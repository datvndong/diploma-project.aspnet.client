using CentralizedDataSystem.Models;
using CentralizedDataSystem.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class StatisticsController : BaseController {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IBaseService baseService, IStatisticsService statisticsService) : base(baseService) {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public async Task<ActionResult> Index() {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string email = user.Email;
            string token = user.Token;

            List<Form> formsCanStatistics = await _statisticsService.FindFormsCanStatistics(token, email);

            ViewBag.List = formsCanStatistics;
            ViewBag.User = user;
            ViewBag.Title = "Statistics";

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Analysis(string path) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            JObject result = await _statisticsService.AnalysisForm(token, path);

            return Json(new { success = true, responseText = JsonConvert.SerializeObject(result) }, JsonRequestBehavior.AllowGet);
        }
    }
}