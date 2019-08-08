using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using CentralizedDataSystem.Utils.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class BaseController : Controller {
        private readonly IBaseService _baseService;

        public BaseController(IBaseService baseService) {
            _baseService = baseService;
        }

        protected User GetUser() {
            string infoStr = EncDecUtil.Decrypt(Request.Cookies[Keywords.USER].Value, Configs.CRYPTO_PASSWORD);
            User user = SerializeUtil.DeSerializeAnObject(infoStr, typeof(User)) as User;

            return user;
        }

        protected void SetUserInfoToCooke(User user) {
            string encVal = null;
            if (user != null) {
                string serUser = SerializeUtil.SerializeAnObject(user);
                encVal = EncDecUtil.Encrypt(serUser, Configs.CRYPTO_PASSWORD);
            }

            HttpCookie cookie = new HttpCookie(Keywords.USER) {
                Value = encVal,
                Expires = DateTime.Now.AddDays(user != null ? Configs.COOKIE_LIFE_TIME : -1)
            };
            Response.Cookies.Add(cookie);
        }

        protected async Task<string> LoginAuthentication() {
            HttpCookie cookie = Request.Cookies[Keywords.USER];
            if (cookie != null) {
                User user = GetUser();

                bool isValidToken = await _baseService.IsValidToken(user.Token);
                if (isValidToken) {
                    return string.Empty;
                }

                TempData[Keywords.ERROR] = Messages.LOGIN_TO_CONTINUE;
            }

            return ViewName.LOGIN;
        }

        protected async Task<string> AdminAuthentication() {
            string loginAuthenResult = await LoginAuthentication();
            if (!string.Empty.Equals(loginAuthenResult)) {
                return loginAuthenResult;
            }

            User info = GetUser();
            if (info != null && info.IsAdmin) {
                return string.Empty;
            }

            return ViewName.ERROR_403;
        }

        protected async Task<string> UserAuthentication() {
            string loginAuthenResult = await LoginAuthentication();
            if (!loginAuthenResult.Equals(string.Empty)) {
                return loginAuthenResult;
            }

            User info = GetUser();
            if (info != null && !info.IsAdmin) {
                return string.Empty;
            }

            return ViewName.ERROR_403;
        }
    }
}