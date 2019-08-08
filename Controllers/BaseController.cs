using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Utils;
using System;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class BaseController : Controller {
        private string LoginAuthentication() {
            HttpCookie cookie = Request.Cookies[Keywords.USER];

            if (cookie != null) {
                return string.Empty;
            }

            TempData[Keywords.ERROR] = Messages.LOGIN_TO_CONTINUE;
            return ViewName.LOGIN;
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

        protected string AdminAuthentication() {
            string loginAuthenResult = LoginAuthentication();
            if (!string.Empty.Equals(loginAuthenResult)) {
                return loginAuthenResult;
            }

            User info = GetUser();
            if (info != null && info.IsAdmin) {
                return string.Empty;
            }

            return ViewName.ERROR_403;
        }

        protected string UserAuthentication() {
            string loginAuthenResult = LoginAuthentication();
            if (!loginAuthenResult.Equals(string.Empty)) {
                return loginAuthenResult;
            }

            User info = GetUser();
            if (info != null && !info.IsAdmin) {
                return string.Empty;
            }

            return ViewName.ERROR_403;
        }

        protected User GetUser() {
            HttpCookie cookie = Request.Cookies[Keywords.USER];
            if (cookie == null) {
                return null;
            }

            string infoStr = EncDecUtil.Decrypt(cookie.Value, Configs.CRYPTO_PASSWORD);
            User user = SerializeUtil.DeSerializeAnObject(infoStr, typeof(User)) as User;

            return user;
        }
    }
}