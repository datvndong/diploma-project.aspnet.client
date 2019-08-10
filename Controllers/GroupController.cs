using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class GroupController : BaseController {
        private readonly IGroupService _groupService;

        public GroupController(IBaseService baseService, IGroupService groupService) : base(baseService) {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string idParent, int page) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            Group parentGroup = await _groupService.FindGroupParent(token, idParent.Equals(Keywords.ROOT_GROUP) ? "data.idParent=root" : "data.idGroup=" + idParent);

            int sizeListGroups = await _groupService.FindNumberOfChildGroupByIdParent(token, parentGroup.IdGroup);
            int totalPages = (int)Math.Ceiling((float)sizeListGroups / Configs.NUMBER_ROWS_PER_PAGE);

            List<Group> groups = await _groupService.FindListChildGroupByIdParentWithPage(token, parentGroup.IdGroup, parentGroup.Name, page);
            parentGroup.NumberOfChildrenGroup = sizeListGroups;

            ViewBag.List = groups;
            ViewBag.CurrPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Root = parentGroup;
            ViewBag.User = user;
            ViewBag.Title = "Groups management";

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create() {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            ViewBag.Link = APIs.ModifiedForm(Keywords.GROUP);
            ViewBag.User = GetUser();
            ViewBag.Title = "Create new Group";

            return View(ViewName.SEND_REPORT);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            string infoRes = await _groupService.FindGroupDataById(token, id);

            JObject jObject = JObject.Parse(infoRes);
            JObject dataObject = (JObject)jObject.GetValue(Keywords.DATA);
            if (dataObject.GetValue(Keywords.ID_PARENT).ToString().Equals(Keywords.ROOT_GROUP)) {
                return View(ViewName.ERROR_403);
            }

            ViewBag.Link = APIs.ModifiedForm(Keywords.GROUP);
            ViewBag.Id = id;
            ViewBag.Data = JsonConvert.SerializeObject(dataObject);
            ViewBag.User = user;
            ViewBag.Title = "Edit Group";

            return View(ViewName.EDIT_REPORT);
        }

        [HttpGet]
        public async Task<ActionResult> AjaxQuery(string idGroup, string isNextStr) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            bool isNext = bool.Parse(isNextStr);
            string groups = null;

            if (isNext) {
                groups = await _groupService.FindGroupsByIdParentWhenCallAjax(token, idGroup);
            } else {
                Group currentGroup = await _groupService.FindGroupParent(token, "data.idGroup=" + idGroup);
                Group parentGroup = await _groupService.FindGroupParent(token, "data.idGroup=" + currentGroup.IdParent);
                if (parentGroup == null) {
                    return Json(new { success = true, responseText = "[]" }, JsonRequestBehavior.AllowGet);
                }
                groups = await _groupService.FindGroupsByIdParentWhenCallAjax(token, parentGroup.IdParent);
            }

            if (groups == null) {
                return Json(new { success = false, responseText = Messages.COULD_NOT_CONNECT_API_SERVER }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, responseText = groups }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            try {
                if (file.ContentLength > 0) {
                    string fileName = Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(fileName);
                    string res = null;

                    if (extension.Equals("." + Keywords.CSV)) {
                        string path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                        file.SaveAs(path);

                        List<string> groups = _groupService.GetListGroupsFromFile(path);
                        foreach (string group in groups) {
                            res = await _groupService.InsertGroup(token, group);
                            if (JObject.Parse(res).Count == 0) {
                                TempData[Keywords.IMPORT] = Messages.IMPORT_FAILED;
                                return RedirectToAction(Keywords.INDEX, new { idParent = Keywords.ROOT_GROUP, page = 1 });
                            }
                        }

                        TempData[Keywords.IMPORT] = Messages.IMPORT_SUCCESSFUL;
                    } else {
                        TempData[Keywords.IMPORT] = Messages.IMPORT_FAILED;
                    }
                } else {
                    TempData[Keywords.IMPORT] = Messages.IMPORT_FAILED;
                }
            } catch {
                TempData[Keywords.IMPORT] = Messages.IMPORT_FAILED;
            }

            return RedirectToAction(Keywords.INDEX, new { idParent = Keywords.ROOT_GROUP, page = 1 });
        }
    }
}