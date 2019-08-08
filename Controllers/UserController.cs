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
    public class UserController : BaseController {
        private readonly IUserService _userService;
        private readonly ISubmissionService _submissionService;
        private readonly IGroupService _groupService;
        //private readonly IReadSurveyService _readSurveyService;

        public UserController(IBaseService baseService, IUserService userService, ISubmissionService submissionService, IGroupService groupService) : base(baseService) {
            _userService = userService;
            _submissionService = submissionService;
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string idGroup, int page, string keyword) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            bool isRootGroup = idGroup.Equals(Keywords.ROOT_GROUP);
            List<Group> groups = new List<Group>();
            List<User> users = new List<User>();

            long sizeListUsers = 0;
            int totalPages = 0;
            string userResByPage = null;

            if (keyword == null) {
                // Normal case
                sizeListUsers = isRootGroup ? await _submissionService.CountSubmissions(token, Keywords.USER.ToLower())
                        : await _userService.CountUsers(token, idGroup);
                totalPages = (int)Math.Ceiling((float)sizeListUsers / Configs.NUMBER_ROWS_PER_PAGE);

                Group currentGroup = await _groupService.FindGroupParent(token, isRootGroup ? "data.idParent=root" : "data.idGroup=" + idGroup);
                Group parentGroup = await _groupService.FindGroupParent(token, "data.idGroup=" + currentGroup.IdParent);
                if (parentGroup == null) {
                    groups.Add(currentGroup);
                } else {
                    groups = await _groupService.FindListChildGroupByIdParentWithPage(token, parentGroup.IdGroup, parentGroup.Name, 0);
                }

                userResByPage = isRootGroup ? await _submissionService.FindSubmissionsByPage(token, Keywords.USER.ToLower(), page)
                        : await _userService.FindUsersByPageAndIdGroup(token, idGroup, page);

                ViewBag.IdGroup = isRootGroup ? Keywords.ROOT_GROUP : idGroup;
            } else {
                // Search by name
                sizeListUsers = await _userService.CountUsersByName(token, keyword);
                totalPages = (int)Math.Ceiling((float)sizeListUsers / Configs.NUMBER_ROWS_PER_PAGE);
                userResByPage = await _userService.FindUsersByPageAndName(token, keyword, page);

                Group rootGroup = await _groupService.FindGroupParent(token, "data.idParent=root");
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

                string phoneNumber = string.Empty;
                if (dataObject.ContainsKey(Keywords.PHONE)) {
                    phoneNumber = dataObject.GetValue(Keywords.PHONE).ToString();
                }
                string address = string.Empty;
                if (dataObject.ContainsKey(Keywords.ADDRESS)) {
                    address = dataObject.GetValue(Keywords.ADDRESS).ToString();
                }

                string groupName = string.Empty;
                idGroup = dataObject.GetValue(Keywords.ID_GROUP).ToString();
                if (!idGroup.Equals(Keywords.ROOT_GROUP)) {
                    groupName = await _groupService.FindGroupFieldByIdGroup(token, idGroup, Keywords.NAME);
                }

                users.Add(new User(id, dataObject.GetValue(Keywords.EMAIL).ToString(), dataObject.GetValue(Keywords.NAME).ToString(),
                    groupName, dataObject.GetValue(Keywords.GENDER).ToString(), phoneNumber, address));
            }

            ViewBag.ListUsers = users;
            ViewBag.ListGroups = groups;
            ViewBag.CurrPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.User = user;
            ViewBag.Title = "Users management";

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create() {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            ViewBag.Link = APIs.ModifiedForm(Keywords.USER.ToLower());
            ViewBag.User = GetUser();
            ViewBag.Title = "Create new User";

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

            string infoRes = await _userService.FindUserDataById(token, Keywords.USER.ToLower(), id);

            JObject jObject = JObject.Parse(infoRes);
            JObject dataObject = (JObject)jObject.GetValue(Keywords.DATA);

            ViewBag.Link = APIs.ModifiedForm(Keywords.USER.ToLower());
            ViewBag.Id = id;
            ViewBag.Data = JsonConvert.SerializeObject(dataObject);
            ViewBag.User = user;
            ViewBag.Title = "Edit Group";

            return View(ViewName.EDIT_REPORT);
        }

        [HttpGet]
        public async Task<ActionResult> ProfileInfo() {
            string userAuthenResult = await UserAuthentication();
            if (!userAuthenResult.Equals(string.Empty)) {
                return View(userAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            string groupName = string.Empty;
            string idGroup = user.IdGroup;
            if (!idGroup.Equals(Keywords.ROOT_GROUP)) {
                groupName = await _groupService.FindGroupFieldByIdGroup(token, idGroup, Keywords.NAME);
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
            string userAuthenResult = await UserAuthentication();
            if (!userAuthenResult.Equals(string.Empty)) {
                return View(userAuthenResult);
            }

            User user = GetUser();

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
            if (name == null || name.Equals(string.Empty)) {
                return Json(new { success = false, responseText = Messages.FILL(Keywords.NAME) }, JsonRequestBehavior.AllowGet);
            }
            if (email == null || email.Equals(string.Empty)) {
                return Json(new { success = false, responseText = Messages.FILL(Keywords.EMAIL) }, JsonRequestBehavior.AllowGet);
            }

            User newUser = new User(email, name, token, idGroup, gender, phoneNumber, address, id, false);

            string res = await _userService.UpdateUserInfo(newUser, Keywords.USER.ToLower());

            SetUserInfoToCooke(newUser);

            return Json(new { success = true, responseText = res }, JsonRequestBehavior.AllowGet);
        }
    }
}