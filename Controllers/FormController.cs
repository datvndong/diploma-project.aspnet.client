using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class FormController : BaseController {
        private readonly IFormService _formService;
        private readonly IFormControlService _formControlService;
        private readonly IRoleService _roleService;
        private readonly IGroupService _groupService;

        public FormController(IBaseService baseService, IFormService formService, IFormControlService formControlService, IRoleService roleService, IGroupService groupService) : base(baseService) {
            _formService = formService;
            _formControlService = formControlService;
            _roleService = roleService;
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(int page) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string email = user.Email;
            string token = user.Token;

            List<FormControl> formControls = await _formControlService.FindByOwner(email);
            int sizeListForms = formControls.Count;
            int totalPages = (int)Math.Ceiling((float)sizeListForms / Configs.NUMBER_ROWS_PER_PAGE);

            List<Form> forms = await _formService.FindForms(token, email, page);

            ViewBag.List = forms;
            ViewBag.CurrPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.User = user;
            ViewBag.Title = "Forms management";

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create() {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            ViewBag.Path = string.Empty;
            ViewBag.User = GetUser();
            ViewBag.Title = "Form Builder";

            return View(ViewName.MODIFIED_FORM);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string path) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            ViewBag.Path = path;
            ViewBag.User = GetUser();
            ViewBag.Title = "Form Builder";

            return View(ViewName.MODIFIED_FORM);
        }

        [HttpGet]
        public async Task<ActionResult> Builder(string path) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            List<Role> roles = await _roleService.FindAll(token);
            List<Group> groups = new List<Group>();
            Group groupTemp = null;

            bool isCreate = path == null;
            JObject formJSON = null;

            if (isCreate) {
                // Create form
                formJSON = new JObject {
                    { Keywords.TITLE, string.Empty },
                    { Keywords.PATH, string.Empty },
                    { Keywords.NAME, string.Empty },
                    { Keywords.TAGS, new JArray() },
                    { Keywords.COMPONENTS, new JArray() },
                    { Keywords.OLD_PATH, string.Empty },
                    { Keywords.START_DATE, string.Empty },
                    { Keywords.START_TIME, string.Empty },
                    { Keywords.EXPIRED_DATE, string.Empty },
                    { Keywords.EXPIRED_TIME, string.Empty }
                };

                groupTemp = await _groupService.FindGroupParent(token, "data.idParent=root");
                groups.Add(groupTemp);
            } else {
                // Edit form
                string formRes = await _formService.FindFormWithToken(token, path);
                formJSON = JObject.Parse(formRes);

                FormControl formControl = await _formControlService.FindByPathForm(path);
                if (formControl == null) {
                    return View(ViewName.ERROR_UNKNOWN);
                }
                string[] start = formControl.Start.Split(null);
                string[] expired = formControl.Expired.Split(null);
                string assign = formControl.Assign;
                formJSON.Add(Keywords.OLD_PATH, formControl.PathForm);
                formJSON.Add(Keywords.ASSIGN, assign);
                bool isAssignToGroup = !(assign.Equals(Keywords.ANONYMOUS) || assign.Equals(Keywords.AUTHENTICATED));
                formJSON.Add(Keywords.IS_ASSIGN_TO_GROUP, isAssignToGroup);
                formJSON.Add(Keywords.START_DATE, start[0]);
                formJSON.Add(Keywords.START_TIME, start[1]);
                formJSON.Add(Keywords.EXPIRED_DATE, expired[0]);
                formJSON.Add(Keywords.EXPIRED_TIME, expired[1]);

                if (isAssignToGroup) {
                    Group currentGroup = await _groupService.FindGroupParent(token, "data.idGroup=" + assign);
                    Group parentGroup = await _groupService.FindGroupParent(token, "data.idGroup=" + currentGroup.IdParent);
                    if (parentGroup == null) {
                        groups.Add(currentGroup);
                    } else {
                        groups = await _groupService.FindListChildGroupByIdParentWithPage(token, parentGroup.IdGroup, parentGroup.Name, 0);
                    }
                } else {
                    groupTemp = await _groupService.FindGroupParent(token, "data.idParent=root");
                    groups.Add(groupTemp);
                }
            }

            ViewBag.IsCreate = isCreate;
            ViewBag.Obj = JsonConvert.SerializeObject(formJSON);
            ViewBag.ListGroups = groups;
            ViewBag.ListRoles = roles;
            ViewBag.User = user;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Modified(string formJSON, string oldPath) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string email = user.Email;
            string token = user.Token;

            bool isCreate = oldPath.Equals(string.Empty);

            JObject jObject = JObject.Parse(formJSON);
            if (jObject.Count == 0) {
                return Json(new { success = false, responseText = Messages.NO_COMPONENTS_IN_FORM }, JsonRequestBehavior.AllowGet);
            }

            string[] fields = { Keywords.TITLE, Keywords.PATH, Keywords.NAME, Keywords.ASSIGN, Keywords.START_DATE, Keywords.START_TIME, Keywords.EXPIRED_DATE, Keywords.EXPIRED_TIME };
            foreach (string field in fields) {
                if (jObject.GetValue(field) == null) {
                    return Json(new { success = false, responseText = Messages.FILL(field) }, JsonRequestBehavior.AllowGet);
                }
            }

            string pathForm = jObject.GetValue(Keywords.PATH).ToString();
            string assign = jObject.GetValue(Keywords.ASSIGN).ToString();
            string startDate = jObject.GetValue(Keywords.START_DATE).ToString();
            string expiredDate = jObject.GetValue(Keywords.EXPIRED_DATE).ToString();

            // Compare date, start > expired
            try {
                DateTime date1 = DateTime.Parse(startDate);
                DateTime date2 = DateTime.Parse(expiredDate);
                if (DateTime.Compare(date1, date2) >= 0) {
                    return Json(new { success = false, responseText = Messages.DATE_PICK_ERROR }, JsonRequestBehavior.AllowGet);
                }
            } catch (FormatException) {
                return Json(new { success = false, responseText = Messages.FORMAT_DATE_ERROR }, JsonRequestBehavior.AllowGet);
            }

            // Get start and expired date time to save in database
            string start = startDate + " " + jObject.GetValue(Keywords.START_TIME).ToString();
            string expired = expiredDate + " " + jObject.GetValue(Keywords.EXPIRED_TIME).ToString();

            // Send to form.io server and save to database
            string res = await _formService.BuildForm(token, formJSON, isCreate ? "" : oldPath);

            if (isCreate) {
                bool isInserted = _formControlService.Insert(new FormControl(pathForm, email, assign, start, expired));
                if (!isInserted) {
                    return Json(new { success = false, responseText = Messages.DATABASE_ERROR }, JsonRequestBehavior.AllowGet);
                }

                // Handle this - send email - took a long time
                //                sendEmailService.SendEmail("vandatnguyen1896@gmail.com", jsonObject.GetValue(Keywords.TITLE).ToString());
            } else {
                long rowAffected = await _formControlService.Update(new FormControl(pathForm, email, assign, start, expired), oldPath);
                if (rowAffected < 1) {
                    return Json(new { success = false, responseText = Messages.DATABASE_ERROR }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = true, responseText = res }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string path) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            bool isDeleteFormControlSuccess = await _formControlService.DeleteByPathForm(path);
            if (!isDeleteFormControlSuccess) {
                TempData[Keywords.DELETE] = Messages.DELETE(Keywords.FORM.ToLower(), isDeleteFormControlSuccess);
                TempData[Keywords.DELETE_STATUS] = isDeleteFormControlSuccess;
                return RedirectToAction(Keywords.INDEX, Keywords.FORM, new { page = 1 });
            }

            bool isDeleteFormSuccess = await _formService.DeleteForm(token, path);
            TempData[Keywords.DELETE] = Messages.DELETE(Keywords.FORM.ToLower(), isDeleteFormSuccess);
            TempData[Keywords.DELETE_STATUS] = isDeleteFormSuccess;

            return RedirectToAction(Keywords.INDEX, Keywords.FORM, new { page = 1 });
        }
    }
}