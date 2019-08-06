using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralizedDataSystem.Resources {
    public class APIs {
        private static readonly string SERVER_URL = "http://localhost:3001";
        public static readonly string LOGIN_URL = SERVER_URL + "/user/login";
        public static readonly string LOGOUT_URL = SERVER_URL + "/logout";
        public static readonly string FORM_URL = SERVER_URL + "/form";
        public static readonly string ROLE_URL = SERVER_URL + "/role";

        public static string GetListSubmissionsURL(string path) {
            return SERVER_URL + "/" + path + "/submission";
        }

        public static string GetFormByAlias(string path) {
            return SERVER_URL + "/" + path;
        }

        public static string ModifiedForm(string path) {
            return SERVER_URL + "/" + path;
        }

        public static string GetWeather(string owmAPIKey, string idCity) {
            return "https://api.openweathermap.org/data/2.5/weather?id=" + idCity + "&appid=" + owmAPIKey;
        }
    }
}