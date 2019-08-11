using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class RoleService : IRoleService {
        private readonly IHttpUtil _httpUtil;

        public RoleService(IHttpUtil httpUtil) {
            _httpUtil = httpUtil;
        }

        public async Task<List<Role>> FindAll(string token) {
            List<Role> list = new List<Role>();

            HttpResponseMessage response = await _httpUtil.GetAsync(token, APIs.ROLE_URL);
            if (response == null || !response.IsSuccessStatusCode) {
                return list;
            }

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);
            foreach (JObject jObject in jArray) {
                if ((bool)jObject.GetValue(Keywords.ADMIN)) {
                    continue;
                }
                string _id = jObject.GetValue(Keywords.ID).ToString();
                string title = jObject.GetValue(Keywords.TITLE).ToString();

                list.Add(new Role(_id, title, null, null));
            }

            return list;
        }

        public async Task<Role> FindOne(string token, string _id) {
            string apiURI = APIs.ROLE_URL + "?_id=" + _id;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return null;
            }

            string content = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(content);
            string title = jObject.GetValue(Keywords.TITLE).ToString();

            return new Role(_id, title, null, null);
        }
    }
}