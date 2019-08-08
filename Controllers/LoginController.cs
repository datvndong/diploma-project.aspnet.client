using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class LoginController : BaseController {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService) {
            _loginService = loginService;
        }

        [HttpGet]
        public ActionResult Index() {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> DoLogin(FormCollection form) {
            string email = form[Keywords.EMAIL];
            string password = form[Keywords.PASSWORD];

            HttpResponseMessage response = await _loginService.CheckLogin(email, password);

            if (response == null) {
                TempData[Keywords.ERROR] = Messages.SERVER_ERROR;
                return RedirectToAction(Keywords.INDEX);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                TempData[Keywords.ERROR] = Messages.INVALID_ACCOUNT_ERROR;
                return RedirectToAction(Keywords.INDEX);
            }

            string token = response.Headers.TryGetValues(Keywords.TOKEN_KEY, out var values) ? values.FirstOrDefault() : null;
            string content = await response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(content);
            JObject dataJSON = (JObject)jObject.GetValue(Keywords.DATA);

            string name = dataJSON.GetValue(Keywords.NAME).ToString();
            bool isAdmin = !dataJSON.ContainsKey(Keywords.PERMISSION);
            User user = null;

            if (isAdmin) {
                user = new User(email, name, token, isAdmin);
                SetUserInfoToCooke(user);

                return RedirectToAction(Keywords.INDEX, Keywords.DASHBOARD);
            }

            if ((int)dataJSON.GetValue(Keywords.STATUS) == Configs.DEACTIVE_STATUS) {
                // User had been blocked
                TempData[Keywords.ERROR] = Messages.DEACTIVE_USER;
                return RedirectToAction(Keywords.INDEX);
            }

            // Set User Info
            string id = jObject.GetValue(Keywords.ID).ToString();
            string idGroup = dataJSON.GetValue(Keywords.ID_GROUP).ToString();
            string gender = dataJSON.GetValue(Keywords.GENDER).ToString();
            string phoneNumber = string.Empty;
            if (dataJSON.ContainsKey(Keywords.PHONE)) {
                phoneNumber = dataJSON.GetValue(Keywords.PHONE).ToString();
            }
            string address = string.Empty;
            if (dataJSON.ContainsKey(Keywords.ADDRESS)) {
                address = dataJSON.GetValue(Keywords.ADDRESS).ToString();
            }

            user = new User(email, name, token, idGroup, gender, phoneNumber, address, id, isAdmin);
            SetUserInfoToCooke(user);

            return RedirectToAction(Keywords.INDEX, Keywords.REPORT, new { page = 1 });
        }

        [HttpGet]
        public async Task<ActionResult> Logout() {
            SetUserInfoToCooke(null);

            bool isLogout = await _loginService.Logout();
            if (!isLogout) {
                TempData[Keywords.ERROR] = Messages.SERVER_ERROR;
            }

            return RedirectToAction(Keywords.INDEX);
        }
    }
}