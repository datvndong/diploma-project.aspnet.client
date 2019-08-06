using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class UserController : BaseController {
        private readonly IUserService userService;
        private readonly ISubmissionService submissionService;
        private readonly IGroupService groupService;
        //private readonly IReadSurveyService readSurveyService;

        public UserController(IUserService userService, ISubmissionService submissionService, IGroupService groupService) {
            this.userService = userService;
            this.submissionService = submissionService;
            this.groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string idGroup, int page, string keyword) {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            bool isRootGroup = idGroup.Equals(Keywords.ROOT_GROUP);
            List<Group> groups = new List<Group>();
            List<User> users = new List<User>();

            long sizeListUsers = 0;
            int totalPages = 0;
            string userResByPage = null;

            if (keyword == null) {
                // Normal case
                sizeListUsers = isRootGroup ? await submissionService.CountSubmissions(Keywords.USER.ToLower())
                        : await userService.CountUsers(idGroup);
                totalPages = (int)Math.Ceiling((float)sizeListUsers / Configs.NUMBER_ROWS_PER_PAGE);

                Group currentGroup = await groupService.FindGroupParent(isRootGroup ? "data.idParent=root" : "data.idGroup=" + idGroup);
                Group parentGroup = await groupService.FindGroupParent("data.idGroup=" + currentGroup.IdParent);
                if (parentGroup == null) {
                    groups.Add(currentGroup);
                } else {
                    groups = await groupService.FindListChildGroupByIdParentWithPage(parentGroup.IdGroup, parentGroup.Name, 0);
                }

                userResByPage = isRootGroup ? await submissionService.FindSubmissionsByPage(Keywords.USER.ToLower(), page)
                        : await userService.FindUsersByPageAndIdGroup(idGroup, page);

                ViewBag.IdGroup = isRootGroup ? Keywords.ROOT_GROUP : idGroup;
            } else {
                // Search by name
                sizeListUsers = await userService.CountUsersByName(keyword);
                totalPages = (int)Math.Ceiling((float)sizeListUsers / Configs.NUMBER_ROWS_PER_PAGE);
                userResByPage = await userService.FindUsersByPageAndName(keyword, page);

                Group rootGroup = await groupService.FindGroupParent("data.idParent=root");
                groups.Add(rootGroup);

                ViewBag.IdGroup = null;
                ViewBag.Keyword = keyword;
            }

            JArray jArray = JArray.Parse(userResByPage);
            JObject dataObject = null;
            foreach (JObject jsonObject in jArray) {
                string id = jsonObject.GetValue(Keywords.ID).ToString();
                dataObject = (JObject)jsonObject.GetValue(Keywords.DATA);

                if ((int)dataObject.GetValue(Keywords.STATUS) == Configs.DEACTIVE_STATUS) {
                    continue;
                }

                string phoneNumber = Keywords.EMPTY_STRING;
                if (dataObject.ContainsKey(Keywords.PHONE)) {
                    phoneNumber = dataObject.GetValue(Keywords.PHONE).ToString();
                }
                string address = Keywords.EMPTY_STRING;
                if (dataObject.ContainsKey(Keywords.ADDRESS)) {
                    address = dataObject.GetValue(Keywords.ADDRESS).ToString();
                }

                string groupName = Keywords.EMPTY_STRING;
                idGroup = dataObject.GetValue(Keywords.ID_GROUP).ToString();
                if (!idGroup.Equals(Keywords.ROOT_GROUP)) {
                    groupName = await groupService.FindGroupFieldByIdGroup(idGroup, Keywords.NAME);
                }

                users.Add(new User(id, dataObject.GetValue(Keywords.EMAIL).ToString(), dataObject.GetValue(Keywords.NAME).ToString(),
                    groupName, dataObject.GetValue(Keywords.GENDER).ToString(), phoneNumber, address));
            }

            ViewBag.ListUsers = users;
            ViewBag.ListGroups = groups;
            ViewBag.CurrPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Title = "Users management";

            return View();
        }

        [HttpGet]
        public ActionResult Create() {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            ViewBag.Link = APIs.ModifiedForm(Keywords.USER.ToLower());
            ViewBag.Title = "Create new User";

            return View(ViewName.SEND_REPORT);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id) {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            string infoRes = await userService.FindUserDataById(Keywords.USER.ToLower(), id);

            JObject jObject = JObject.Parse(infoRes);
            JObject dataObject = (JObject)jObject.GetValue(Keywords.DATA);

            ViewBag.Link = APIs.ModifiedForm(Keywords.USER.ToLower());
            ViewBag.Id = id;
            ViewBag.Data = JsonConvert.SerializeObject(dataObject);
            ViewBag.Title = "Edit Group";

            return View(ViewName.EDIT_REPORT);
        }

        [HttpGet]
        public async Task<ActionResult> ProfileInfo() {
            string userAuthenResult = UserAuthentication();
            if (!userAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(userAuthenResult);
            }

            User user = (User)Session[Keywords.USER];

            string groupName = Keywords.EMPTY_STRING;
            string idGroup = user.IdGroup;
            if (!idGroup.Equals(Keywords.ROOT_GROUP)) {
                groupName = await groupService.FindGroupFieldByIdGroup(idGroup, Keywords.NAME);
            }
            user.NameGroup = groupName;

            int reportsNumber = user.ReportsNumber;
            int submittedNumber = user.SubmittedNumber;

            ViewBag.ReportsNumber = reportsNumber;
            ViewBag.SubmittedNumber = submittedNumber;
            ViewBag.NotSubmittedNumber = reportsNumber - submittedNumber;
            ViewBag.User = user;
            ViewBag.Title = "Profile";

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateProfile(FormCollection form) {
            string userAuthenResult = UserAuthentication();
            if (!userAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(userAuthenResult);
            }

            User user = (User)Session[Keywords.USER];
            string name = form[Keywords.NAME];
            string email = form[Keywords.EMAIL];
            string gender = form[Keywords.GENDER]; 
            string address = form[Keywords.ADDRESS]; 
            string phoneNumber = form[Keywords.PHONE];
            string token = form[Keywords.TOKEN];
            string id = form[Keywords.ID]; 
            string idGroup = form[Keywords.ID_GROUP]; 

            if (!id.Equals(user.Id) || !idGroup.Equals(user.IdGroup)) {
                return Json(new { success = false, responseText = Messages.UNAUTHORIZED_MESSAGE }, JsonRequestBehavior.AllowGet);
            }
            if (name == null || name.Equals(Keywords.EMPTY_STRING)) {
                return Json(new { success = false, responseText = Messages.FILL(Keywords.NAME) }, JsonRequestBehavior.AllowGet);
            }
            if (email == null|| email.Equals(Keywords.EMPTY_STRING)) {
                return Json(new { success = false, responseText = Messages.FILL(Keywords.EMAIL) }, JsonRequestBehavior.AllowGet);
            }

            User newUser = new User(email, name, token, idGroup, gender, phoneNumber, address, id);

            string res = await userService.UpdateUserInfo(newUser, Keywords.USER.ToLower());

            Session[Keywords.USER] = newUser;

            return Json(new { success = true, responseText = res }, JsonRequestBehavior.AllowGet);
        }
    }
}