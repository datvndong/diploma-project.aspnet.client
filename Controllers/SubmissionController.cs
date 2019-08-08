using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class SubmissionController : BaseController {
        private readonly IFormService _formService;
        private readonly ISubmissionService _submissionService;

        public SubmissionController(IBaseService baseService, IFormService formService, ISubmissionService submissionService) : base(baseService) {
            _formService = formService;
            _submissionService = submissionService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string path, int page) {
            string loginAuthenResult = await LoginAuthentication();
            if (!string.Empty.Equals(loginAuthenResult)) {
                TempData[Keywords.ERROR] = Messages.LOGIN_TO_CONTINUE;
                return RedirectToAction(Keywords.INDEX, Keywords.LOGIN);
            }

            User user = GetUser();
            string token = user.Token;

            long sizeListSubs = await _submissionService.CountSubmissions(token, path);
            int totalPages = (int)Math.Ceiling((float)sizeListSubs / Configs.NUMBER_ROWS_PER_PAGE);

            string submissionRes = await _submissionService.FindSubmissionsByPage(token, path, page);
            string formRes = await _formService.FindFormWithToken(token, path);

            ViewBag.CurrPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Path = path;
            ViewBag.SubmissionData = submissionRes;
            ViewBag.FormData = formRes;
            ViewBag.User = user;
            ViewBag.Title = "Submissions";

            return View();
        }
    }
}