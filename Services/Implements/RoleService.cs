using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CentralizedDataSystem.Services.Implements {
    public class RoleService : IRoleService {
        public async Task<List<Role>> FindAll() {
            List<Role> list = new List<Role>();

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(APIs.ROLE_URL);
            if (response == null) return list;

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

        public async Task<Role> FindOne(string _id) {
            string apiURI = APIs.ROLE_URL + "?_id=" + _id;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return null;

            string content = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(content);
            string title = jObject.GetValue(Keywords.TITLE).ToString();

            return new Role(_id, title, null, null);
        }
    }
}