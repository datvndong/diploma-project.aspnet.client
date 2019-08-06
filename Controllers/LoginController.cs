using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CentralizedDataSystem.Resources;
using Newtonsoft.Json.Linq;
using CentralizedDataSystem.Models;
using CentralizedDataSystem.Utils;
using CentralizedDataSystem.Services.Interfaces;

namespace CentralizedDataSystem.Controllers {
    public class LoginController : BaseController {
        private readonly ILoginService loginService;

        public LoginController(ILoginService loginService) {
            this.loginService = loginService;
        }

        [HttpGet]
        public ActionResult Index() {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> DoLogin(FormCollection form) {
            string email = form[Keywords.EMAIL];
            string password = form[Keywords.PASSWORD];

            HttpUtils.Instance.RemoveToken();
            HttpResponseMessage response = await loginService.CheckLogin(email, password);

            if (response == null) {
                TempData[Keywords.ERROR] = Messages.SERVER_ERROR;
                return RedirectToAction(Keywords.INDEX);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                TempData[Keywords.ERROR] = Messages.INVALID_ACCOUNT_ERROR;
                return RedirectToAction(Keywords.INDEX);
            }

            string token = response.Headers.TryGetValues(Keywords.TOKEN_KEY, out var values) ? values.FirstOrDefault() : null;
            HttpUtils.Instance.SetToken(token);

            string content = await response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(content);
            JObject dataJSON = (JObject)jObject.GetValue(Keywords.DATA);
            bool isAdmin = !dataJSON.ContainsKey(Keywords.PERMISSION);
            Session[Keywords.IS_ADMIN] = isAdmin;

            if (isAdmin) {
                Session[Keywords.USER] = new User(email, dataJSON.GetValue(Keywords.NAME).ToString(), token);
                return RedirectToAction(Keywords.INDEX, Keywords.DASHBOARD);
            }

            if ((int)dataJSON.GetValue(Keywords.STATUS) == Configs.DEACTIVE_STATUS) {
                // User had been blocked
                TempData[Keywords.ERROR] = Messages.DEACTIVE_USER;
                return RedirectToAction(Keywords.INDEX);
            }

            // Set User Info
            string id = jObject.GetValue(Keywords.ID).ToString();
            string name = dataJSON.GetValue(Keywords.NAME).ToString();
            string idGroup = dataJSON.GetValue(Keywords.ID_GROUP).ToString();
            string gender = dataJSON.GetValue(Keywords.GENDER).ToString();
            string phoneNumber = Keywords.EMPTY_STRING;
            if (dataJSON.ContainsKey(Keywords.PHONE)) {
                phoneNumber = dataJSON.GetValue(Keywords.PHONE).ToString();
            }
            string address = Keywords.EMPTY_STRING;
            if (dataJSON.ContainsKey(Keywords.ADDRESS)) {
                address = dataJSON.GetValue(Keywords.ADDRESS).ToString();
            }

            Session[Keywords.USER] = new User(email, name, token, idGroup, gender, phoneNumber, address, id);

            return RedirectToAction(Keywords.INDEX, Keywords.REPORT, new { page = 1 });
        }

        [HttpGet]
        public async Task<ActionResult> Logout() {
            Session.Remove(Keywords.USER);

            bool isLogout = await loginService.Logout();
            if (!isLogout) {
                TempData[Keywords.ERROR] = Messages.SERVER_ERROR;
            }

            HttpUtils.Instance.RemoveToken();

            return RedirectToAction(Keywords.INDEX);
        }
    }
}