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
    public class StatisticsController : BaseController {
        private readonly IStatisticsService statisticsService;

        public StatisticsController(IStatisticsService statisticsService) {
            this.statisticsService = statisticsService;
        }

        [HttpGet]
        public async Task<ActionResult> Index() {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            User user = (User)Session[Keywords.USER];
            string email = user.Email;

            List<Form> formsCanStatistics = await statisticsService.FindFormsCanStatistics(email);

            ViewBag.List = formsCanStatistics;
            ViewBag.Title = "Statistics";

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Analysis(string path) {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            JObject result = await statisticsService.AnalysisForm(path);

            return Json(new { success = true, responseText = JsonConvert.SerializeObject(result) }, JsonRequestBehavior.AllowGet);
        }
    }
}