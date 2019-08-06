using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class DashboardController : BaseController {
        private readonly IDashboardService dashboardService;

        public DashboardController(IDashboardService dashboardService) {
            this.dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult> Index() {
            string adminAuthenResult = AdminAuthentication();
            if (!adminAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return View(adminAuthenResult);
            }

            User user = (User)Session[Keywords.USER];

            ViewBag.Title = "Dashboard";
            ViewBag.Groups = await dashboardService.FindNumberGroups();
            ViewBag.Forms = await dashboardService.FindNumberForms(user.Email);
            ViewBag.Users = await dashboardService.FindNumberUsers();
            ViewBag.City = await dashboardService.GetCityInfo();

            return View();
        }
    }
}