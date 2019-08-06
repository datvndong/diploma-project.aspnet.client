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
using CentralizedDataSystem.Utils;
using Newtonsoft.Json.Linq;
using CentralizedDataSystem.Services.Interfaces;

namespace CentralizedDataSystem.Services.Implements {
    public class LoginService : ILoginService {
        public async Task<HttpResponseMessage> CheckLogin(string email, string password) {
            JObject info = new JObject {
                { Keywords.EMAIL, email },
                { Keywords.PASSWORD, password }
            };

            JObject data = new JObject {
                { Keywords.DATA, info }
            };
            
            return await HttpUtils.Instance.PostAsync(APIs.LOGIN_URL, data.ToString());
        }

        public async Task<bool> Logout() {
            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(APIs.LOGOUT_URL);
            if (response == null) return false;

            return true;
        }
    }
}