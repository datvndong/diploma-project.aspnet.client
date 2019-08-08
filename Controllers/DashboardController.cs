using CentralizedDataSystem.Models;
using CentralizedDataSystem.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class DashboardController : BaseController {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IBaseService baseService, IDashboardService dashboardService) : base(baseService) {
            _dashboardService = dashboardService;
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

            ViewBag.Groups = await _dashboardService.FindNumberGroups(token);
            ViewBag.Forms = await _dashboardService.FindNumberForms(email);
            ViewBag.Users = await _dashboardService.FindNumberUsers(token);
            ViewBag.City = await _dashboardService.GetCityInfo();
            ViewBag.User = user;
            ViewBag.Title = "Dashboard";

            return View();
        }
    }
}