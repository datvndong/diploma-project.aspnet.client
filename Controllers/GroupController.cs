using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class GroupController : BaseController {
        private readonly IGroupService groupService;
        //private readonly IReadSurveyService readSurveyService;

        public GroupController(IGroupService groupService) {
            this.groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string idParent, int page) {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            Group parentGroup = await groupService.FindGroupParent(idParent.Equals(Keywords.ROOT_GROUP) ? "data.idParent=root" : "data.idGroup=" + idParent);

            int sizeListGroups = await groupService.FindNumberOfChildGroupByIdParent(parentGroup.IdGroup);
            int totalPages = (int)Math.Ceiling((float)sizeListGroups / Configs.NUMBER_ROWS_PER_PAGE);

            List<Group> groups = await groupService.FindListChildGroupByIdParentWithPage(parentGroup.IdGroup, parentGroup.Name, page);
            parentGroup.NumberOfChildrenGroup = sizeListGroups;

            ViewBag.List = groups;
            ViewBag.CurrPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Root = parentGroup;
            ViewBag.Title = "Groups management";

            return View();
        }

        [HttpGet]
        public ActionResult Create() {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            ViewBag.Link = APIs.ModifiedForm(Keywords.GROUP);
            ViewBag.Title = "Create new Group";

            return View(ViewName.SEND_REPORT);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id) {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            string infoRes = await groupService.FindGroupDataById(id);

            JObject jObject = JObject.Parse(infoRes);
            JObject dataObject = (JObject)jObject.GetValue(Keywords.DATA);
            if (dataObject.GetValue(Keywords.ID_PARENT).ToString().Equals(Keywords.ROOT_GROUP)) {
                return View(ViewName.ERROR_403);
            }

            ViewBag.Link = APIs.ModifiedForm(Keywords.GROUP);
            ViewBag.Id = id;
            ViewBag.Data = JsonConvert.SerializeObject(dataObject);
            ViewBag.Title = "Edit Group";

            return View(ViewName.EDIT_REPORT);
        }

        [HttpGet]
        public async Task<ActionResult> AjaxQuery(string idGroup, string isNextStr) {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            bool isNext = Boolean.Parse(isNextStr);
            string groups = null;

            if (isNext) {
                groups = await groupService.FindGroupsByIdParentWhenCallAjax(idGroup);
            } else {
                Group currentGroup = await groupService.FindGroupParent("data.idGroup=" + idGroup);
                Group parentGroup = await groupService.FindGroupParent("data.idGroup=" + currentGroup.IdParent);
                if (parentGroup == null) {
                    return Json(new { success = true, responseText = "[]" }, JsonRequestBehavior.AllowGet);
                }
                groups = await groupService.FindGroupsByIdParentWhenCallAjax(parentGroup.IdParent);
            }

            if (groups == null) {
                return Json(new { success = false, responseText = Messages.COULD_NOT_CONNECT_API_SERVER }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, responseText = groups }, JsonRequestBehavior.AllowGet);
        }
    }
}