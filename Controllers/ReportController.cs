using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class ReportController : BaseController {
        private readonly IFormService formService;
        private readonly IFormControlService formControlService;
        private readonly ISubmissionService submissionService;
        private readonly IGroupService groupService;

        public ReportController(IFormService formService, IFormControlService formControlService, ISubmissionService submissionService, IGroupService groupService) {
            this.formService = formService;
            this.formControlService = formControlService;
            this.submissionService = submissionService;
            this.groupService = groupService;
        }

        private async Task<List<Form>> AddFormToList(List<Form> listForm, List<FormControl> listFormControl) {
            foreach (FormControl formControl in listFormControl) {
                string path = formControl.PathForm;
                string start = formControl.Start;
                string expired = formControl.Expired;

                int durationPercent = CalculateUtils.GetDurationPercent(start, expired);
                string typeProgressBar = CalculateUtils.GetTypeProgressBar(durationPercent);

                string formRes = await formService.FindFormWithToken(path);
                JObject formResJSON = JObject.Parse(formRes);
                if (formResJSON.Count == 0) return listForm;
                string title = formResJSON.GetValue(Keywords.TITLE).ToString();
                List<string> tags = new List<string>();
                JArray tagsArray = (JArray)formResJSON.GetValue(Keywords.TAGS);
                foreach (JObject tag in tagsArray) {
                    tags.Add(tag.ToString());
                }

                string submissionsRes = await submissionService.FindSubmissionsByPage(path, 1);
                JArray submissionResJSON = JArray.Parse(submissionsRes);
                bool isSubmitted = submissionResJSON.Count != 0;

                bool isPending = CalculateUtils.IsFormPendingOrExpired(start);

                listForm.Add(new Form(title, path, start, expired, tags, durationPercent, typeProgressBar, isSubmitted, isPending));
            }

            return listForm;
        }

        private async Task<List<Form>> GetListFormByIdGroupRecursive(List<Form> listForm, string id) {
            List<FormControl> listFormsGroup = await formControlService.FindByAssign(id);
            await AddFormToList(listForm, listFormsGroup);

            // Check if idGroup have idParent
            string nextIdParent = await groupService.FindGroupFieldByIdGroup(id, Keywords.ID_PARENT);

            if (!nextIdParent.Equals(Keywords.ROOT_GROUP)) {
                await GetListFormByIdGroupRecursive(listForm, nextIdParent);
            }

            return listForm;
        }

        private async Task<bool> IsFormAssignToUser(string assignIdGroup, string formIdGroup) {
            if (assignIdGroup.Equals(formIdGroup)) {
                return true;
            }

            string nextIdParent = await groupService.FindGroupFieldByIdGroup(formIdGroup, Keywords.ID_PARENT);
            if (!nextIdParent.Equals(Keywords.ROOT_GROUP)) {
                return await IsFormAssignToUser(assignIdGroup, nextIdParent);
            } else {
                return false;
            }
        }

        [HttpGet]
        public async Task<ActionResult> Index(int page) {
            string userAuthenResult = UserAuthentication();
            if (!userAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(userAuthenResult);
            }

            User user = (User)Session[Keywords.USER];

            List<Form> listAllForms = await GetListFormByIdGroupRecursive(new List<Form>(), user.IdGroup);
            List<Form> listFormsByPage = new List<Form>();

            List<FormControl> listFormsAuth = await formControlService.FindByAssign(Keywords.AUTHENTICATED);
            await AddFormToList(listAllForms, listFormsAuth);

            int numberRowsPerPage = Configs.NUMBER_ROWS_PER_PAGE;
            int sizeListReports = listAllForms.Count;

            // Process for Profile page
            user.ReportsNumber = sizeListReports;
            int submittedNumber = 0;
            foreach (Form form in listAllForms) {
                if (form.IsSubmitted) {
                    submittedNumber++;
                }
            }
            user.SubmittedNumber = submittedNumber;

            int totalPages = (int)Math.Ceiling((float)sizeListReports / numberRowsPerPage);

            int start = (page - 1) * numberRowsPerPage;
            int end = page * numberRowsPerPage;
            for (int i = start; i < end; i++) {
                if (i == sizeListReports) {
                    break;
                }
                listFormsByPage.Add(listAllForms[i]);
            }

            ViewBag.List = listFormsByPage;
            ViewBag.CurrPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Title = "Reports";

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Auth(string path) {
            string userAuthenResult = UserAuthentication();
            if (!userAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(userAuthenResult);
            }

            User user = (User)Session[Keywords.USER];

            FormControl formControl = await formControlService.FindByPathForm(path);
            if (formControl == null) {
                return View(ViewName.ERROR_404);
            }
            string assign = formControl.Assign;

            bool isFormPending = CalculateUtils.IsFormPendingOrExpired(formControl.Start);
            bool isFormExpired = !CalculateUtils.IsFormPendingOrExpired(formControl.Expired);
            if (isFormPending || isFormExpired) {
                return View(ViewName.ERROR_403);
            }

            if (assign.Equals(Keywords.AUTHENTICATED) || await IsFormAssignToUser(assign, user.IdGroup)) {
                string res1 = await formService.FindFormWithToken(path);
                JObject resJSON = JObject.Parse(res1);

                string res2 = await submissionService.FindSubmissionsByPage(path, 1);
                bool isNotSubmitted = JArray.Parse(res2).Count == 0;

                ViewBag.Link = isNotSubmitted ? APIs.ModifiedForm(path) : Keywords.EMPTY_STRING;
                ViewBag.Title = isNotSubmitted ? resJSON.GetValue(Keywords.TITLE).ToString() : Messages.HAS_SUBMITTED_MESSAGE;

                return View(ViewName.SEND_REPORT);
            }

            return View(ViewName.ERROR_404);
        }

        [HttpGet]
        public async Task<ActionResult> Anon(string path) {
            JObject formJSON = JObject.Parse(await formService.FindFormWithNoToken(path));
            if (formJSON.Count == 0) {
                return View(ViewName.ERROR_403);
            }

            JArray submissionAccessJSON = (JArray)formJSON.GetValue(Keywords.SUBMISSION_ACCESS);
            JArray roles = (JArray)((JObject)submissionAccessJSON[4]).GetValue(Keywords.ROLES);
            if (roles.Count == 0 || !((string)roles[0]).Equals(Keywords.ANONYMOUS)) {
                return View(ViewName.ERROR_404);
            }

            ViewBag.Link = APIs.ModifiedForm(path);
            ViewBag.Title = formJSON.GetValue(Keywords.TITLE).ToString();

            return View(ViewName.SEND_REPORT);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string path) {
            string userAuthenResult = UserAuthentication();
            if (!userAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(userAuthenResult);
            }

            User user = (User)Session[Keywords.USER];

            FormControl formControl = await formControlService.FindByPathForm(path);
            if (formControl == null) {
                return View(ViewName.ERROR_404);
            }
            string assign = formControl.Assign;

            bool isFormPending = CalculateUtils.IsFormPendingOrExpired(formControl.Start);
            bool isFormExpired = !CalculateUtils.IsFormPendingOrExpired(formControl.Expired);
            if (isFormPending || isFormExpired) {
                return View(ViewName.ERROR_403);
            }

            if (assign.Equals(Keywords.AUTHENTICATED) || await IsFormAssignToUser(assign, user.IdGroup)) {
                string res1 = await formService.FindFormWithToken(path);
                JObject resJSON = JObject.Parse(res1);

                string res2 = await submissionService.FindSubmissionsByPage(path, 1);
                JArray jsonArray = JArray.Parse(res2);
                bool isNotSubmitted = jsonArray.Count == 0;
                if (isNotSubmitted) {
                    ViewBag.Link = Keywords.EMPTY_STRING;
                    ViewBag.Title = Messages.HAS_NOT_SUBMITTED_MESSAGE;
                } else {
                    ViewBag.Link = APIs.ModifiedForm(path);
                    ViewBag.Title = resJSON.GetValue(Keywords.TITLE).ToString();
                    ViewBag.Id = ((JObject)jsonArray[0]).GetValue(Keywords.ID).ToString();
                    ViewBag.Data = ((JObject)jsonArray[0]).GetValue(Keywords.DATA).ToString();
                }

                return View(ViewName.EDIT_REPORT);
            }

            return View(ViewName.ERROR_404);
        }
    }
}