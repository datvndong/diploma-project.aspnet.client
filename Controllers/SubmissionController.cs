using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services;
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
    public class SubmissionController : BaseController {
        private readonly IFormService formService;
        private readonly ISubmissionService submissionService;

        public SubmissionController(IFormService formService, ISubmissionService submissionService) {
            this.formService = formService;
            this.submissionService = submissionService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string path, int page) {
            User user = (User)Session[Keywords.USER];

            if (user == null) {
                TempData[Keywords.ERROR] = Messages.LOGIN_TO_CONTINUE;
                return RedirectToAction(Keywords.INDEX, Keywords.LOGIN);
            };

            long sizeListSubs = await submissionService.CountSubmissions(path);
            int totalPages = (int)Math.Ceiling((float)sizeListSubs / Configs.NUMBER_ROWS_PER_PAGE);

            string submissionRes = await submissionService.FindSubmissionsByPage(path, page);
            string formRes = await formService.FindFormWithToken(path);

            ViewBag.CurrPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Path = path;
            ViewBag.SubmissionData = submissionRes;
            ViewBag.FormData = formRes;
            ViewBag.Title = "Submissions";

            return View();
        }
    }
}