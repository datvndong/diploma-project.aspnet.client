using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class BaseController : Controller {
        private string LoginAuthentication() {
            User user = (User)Session[Keywords.USER];

            if (user != null && user.Token.GetType() == typeof(string)) {
                return Keywords.EMPTY_STRING;
            }

            TempData[Keywords.ERROR] = Messages.LOGIN_TO_CONTINUE;
            return ViewName.LOGIN;
        }

        protected string AdminAuthentication() {
            string loginAuthenResult = LoginAuthentication();
            if (!loginAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return loginAuthenResult;
            }

            object isAdmin = Session[Keywords.IS_ADMIN];
            if (isAdmin != null && (bool)isAdmin) {
                return Keywords.EMPTY_STRING;
            }
            return ViewName.ERROR_403;
        }

        protected string UserAuthentication() {
            string loginAuthenResult = LoginAuthentication();
            if (!loginAuthenResult.Equals(Keywords.EMPTY_STRING)) {
                return loginAuthenResult;
            }

            object isAdmin = Session[Keywords.IS_ADMIN];
            if (isAdmin != null && !((bool)isAdmin)) {
                return Keywords.EMPTY_STRING;
            }
            return ViewName.ERROR_403;
        }
    }
}