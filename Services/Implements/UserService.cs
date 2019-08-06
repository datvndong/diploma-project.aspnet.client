using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CentralizedDataSystem.Services.Implements {
    public class UserService : IUserService {
        public async Task<string> FindUserDataById(string path, string id) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "/" + id;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> UpdateUserInfo(User user, string path) {
            JObject info = new JObject {
                { Keywords.EMAIL, user.Email },
                { Keywords.NAME, user.Name },
                { Keywords.ID_GROUP, user.IdGroup },
                { Keywords.PERMISSION, Keywords.USER.ToLower() },
                { Keywords.GENDER, user.Gender },
                { Keywords.PHONE, user.PhoneNumber },
                { Keywords.ADDRESS, user.Address },
                { Keywords.STATUS, 1 },
                { Keywords.SUBMIT, true }
            };

            JObject data = new JObject {
                { Keywords.DATA, info }
            };

            string apiURI = APIs.GetListSubmissionsURL(path) + "/" + user.Id;

            HttpResponseMessage response = await HttpUtils.Instance.PutAsync(apiURI, data.ToString());
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<long> CountUsers(string idGroup) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "?limit=" + Configs.LIMIT_QUERY + "&data.idGroup="
                    + idGroup + "&select=_id";

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return 0;

            string content = await response.Content.ReadAsStringAsync();
            return JArray.Parse(content).Count;
        }

        public async Task<string> FindUsersByPageAndIdGroup(string idGroup, int page) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "?select=data&limit=" + Configs.NUMBER_ROWS_PER_PAGE
                    + "&skip=" + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE + "&data.idGroup=" + idGroup;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<long> CountUsersByName(string keyword) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "?limit=" + Configs.LIMIT_QUERY + "&data.name__regex=/"
                    + keyword + "/&select=_id";

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return 0;

            string content = await response.Content.ReadAsStringAsync();
            return JArray.Parse(content).Count;
        }

        public async Task<string> FindUsersByPageAndName(string keyword, int page) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "?select=data&limit=" + Configs.NUMBER_ROWS_PER_PAGE
                    + "&skip=" + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE + "&data.name__regex=/" + keyword + "/";

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> InsertUser(string data) {
            HttpResponseMessage response = await HttpUtils.Instance.PostAsync(APIs.GetFormByAlias(Keywords.USER.ToLower()), data);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}