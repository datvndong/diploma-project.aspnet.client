using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class UserService : IUserService {
        private readonly IHttpUtil _httpUtil;

        public UserService(IHttpUtil httpUtil) {
            _httpUtil = httpUtil;
        }

        public async Task<string> FindUserDataById(string token, string path, string id) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "/" + id;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
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

            HttpResponseMessage response = await _httpUtil.PutAsync(user.Token, apiURI, JsonConvert.SerializeObject(data));
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<long> CountUsers(string token, string idGroup) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "?limit=" + Configs.LIMIT_QUERY + "&data.idGroup="
                    + idGroup + "&select=_id";

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null) return 0;

            string content = await response.Content.ReadAsStringAsync();
            return JArray.Parse(content).Count;
        }

        public async Task<string> FindUsersByPageAndIdGroup(string token, string idGroup, int page) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "?select=data";
            if (page == 0) {
                // If page = 0 => get full data
                apiURI += "&limit=" + Configs.LIMIT_QUERY;
            } else {
                apiURI += "&limit=" + Configs.NUMBER_ROWS_PER_PAGE + "&skip=" + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE;
            }
            apiURI += "&data.idGroup=" + idGroup;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null) return "[]";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<long> CountUsersByName(string token, string keyword) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "?limit=" + Configs.LIMIT_QUERY + "&data.name__regex=/"
                    + keyword + "/&select=_id";

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null) return 0;

            string content = await response.Content.ReadAsStringAsync();
            return JArray.Parse(content).Count;
        }

        public async Task<string> FindUsersByPageAndName(string token, string keyword, int page) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "?select=data&limit=" + Configs.NUMBER_ROWS_PER_PAGE
                    + "&skip=" + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE + "&data.name__regex=/" + keyword + "/";

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public List<string> GetListUsersFromFile(string pathFile) {
            using (StreamReader reader = new StreamReader(pathFile, Encoding.UTF8)) {
                List<string> list = new List<string>();
                bool isFirstRow = true;

                string[] labels = {
                    Keywords.EMAIL, Keywords.NAME,
                    Keywords.ID_GROUP, Keywords.PERMISSION,
                    Keywords.GENDER, Keywords.PHONE,
                    Keywords.ADDRESS, Keywords.STATUS
                };
                int size = labels.Length;

                JObject data = null;
                JObject info = null;

                while (!reader.EndOfStream) {
                    if (isFirstRow) {
                        reader.ReadLine();
                        isFirstRow = false;
                        continue;
                    }

                    data = new JObject();
                    info = new JObject();

                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    for (int i = 0; i < size; i++) {
                        info.Add(labels[i], values[i]);
                    }
                    data.Add(Keywords.DATA, info);

                    list.Add(JsonConvert.SerializeObject(data));
                }

                return list;
            }
        }

        public async Task<string> InsertUser(string token, string data) {
            HttpResponseMessage response = await _httpUtil.PostAsync(token, APIs.GetFormByAlias(Keywords.USER.ToLower()), data);
            if (response == null || response.StatusCode != HttpStatusCode.Created) {
                return "{}";
            }

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> FindAllUsers(string token) {
            List<User> users = new List<User>();

            string apiURI = APIs.GetListSubmissionsURL(Keywords.USER.ToLower()) + "?limit=" + Configs.LIMIT_QUERY + "&select=data";

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null) return "[]";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}