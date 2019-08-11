using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class LoginService : ILoginService {
        private readonly IHttpUtil _httpUtil;

        public LoginService(IHttpUtil httpUtil) {
            _httpUtil = httpUtil;
        }

        public async Task<HttpResponseMessage> CheckLogin(string email, string password) {
            JObject info = new JObject {
                { Keywords.EMAIL, email },
                { Keywords.PASSWORD, password }
            };

            JObject data = new JObject {
                { Keywords.DATA, info }
            };
            
            return await _httpUtil.PostAsync(APIs.LOGIN_URL, data.ToString());
        }

        public async Task<bool> Logout() {
            HttpResponseMessage response = await _httpUtil.GetAsync(APIs.LOGOUT_URL);
            if (response == null || !response.IsSuccessStatusCode) {
                return false;
            }

            return true;
        }
    }
}