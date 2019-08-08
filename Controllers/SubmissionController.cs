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

        public SubmissionController(IFormService formService, ISubmissionService submissionService) {
            _formService = formService;
            _submissionService = submissionService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string path, int page) {
            // This controller can used by Admin and User -> so can't authen by normal way in BaseController
            User user = GetUser();

            if (user == null) {
                TempData[Keywords.ERROR] = Messages.LOGIN_TO_CONTINUE;
                return RedirectToAction(Keywords.INDEX, Keywords.LOGIN);
            };

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