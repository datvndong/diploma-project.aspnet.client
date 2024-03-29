﻿using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class GroupService : IGroupService {
        private readonly IHttpUtil _httpUtil;

        public GroupService(IHttpUtil httpUtil) {
            _httpUtil = httpUtil;
        }

        public async Task<string> FindGroupFieldByIdGroup(string token, string idGroup, string field) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "?select=data&data.idGroup=" + idGroup;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return string.Empty;
            }

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);
            if (jArray.Count == 0) return string.Empty;
            JObject jObject = jArray.Children<JObject>().FirstOrDefault();
            JObject dataObject = (JObject)jObject.GetValue(Keywords.DATA);

            return dataObject.GetValue(field).ToString();
        }

        public async Task<string> FindGroupDataById(string token, string id) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "/" + id;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return "{}";
            }

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<Group> FindGroupParent(string token, string condition) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "?select=data&" + condition;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return null;
            }

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);
            if (jArray.Count == 0) return null;
            JObject jObject = jArray.Children<JObject>().FirstOrDefault();
            JObject dataObject = (JObject)jObject.GetValue(Keywords.DATA);

            string id = jObject.GetValue(Keywords.ID).ToString();
            string idGroup = dataObject.GetValue(Keywords.ID_GROUP).ToString();
            string name = dataObject.GetValue(Keywords.NAME).ToString();
            string idParent = dataObject.GetValue(Keywords.ID_PARENT).ToString();
            string nameParent = string.Empty;

            return new Group(id, idGroup, name, idParent, nameParent);
        }

        public async Task<List<Group>> FindListChildGroupByIdParentWithPage(string token, string idParent, string nameParent, int page) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP);
            if (page == 0) {
                // If page = 0 => get full data
                apiURI += "?limit=" + Configs.LIMIT_QUERY;
            } else {
                apiURI += "?limit=" + Configs.NUMBER_ROWS_PER_PAGE + "&skip=" + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE;
            }
            apiURI += "&sort=-create&select=data&data.status=" + Configs.ACTIVE_STATUS + "&data.idParent=" + idParent;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return null;
            }

            List<Group> groups = new List<Group>();

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);
            JObject dataObject = null;

            foreach (JObject jObject in jArray) {
                dataObject = (JObject)jObject.GetValue(Keywords.DATA);

                string id = jObject.GetValue(Keywords.ID).ToString();
                string idGroup = dataObject.GetValue(Keywords.ID_GROUP).ToString();
                string name = dataObject.GetValue(Keywords.NAME).ToString();
                int childSize = await FindNumberOfChildGroupByIdParent(token, idGroup);
                groups.Add(new Group(id, idGroup, name, idParent, nameParent, childSize));
            }

            return groups;
        }

        public async Task<int> FindNumberOfChildGroupByIdParent(string token, string idParent) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "?limit=" + Configs.LIMIT_QUERY
                    + "&select=_id&data.status=" + Configs.ACTIVE_STATUS + "&data.idParent=" + idParent;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return 0;
            }

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);

            return jArray.Count;
        }

        public async Task<string> FindAllGroupsByIdParent(string token, string idParent) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "?limit=" + Configs.LIMIT_QUERY
                    + "&sort=-create&select=data&data.status=" + Configs.ACTIVE_STATUS + "&data.idParent=" + idParent;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return "[]";
            }

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public List<string> GetListGroupsFromFile(string pathFile) {
            using (StreamReader reader = new StreamReader(pathFile, Encoding.UTF8)) {
                List<string> list = new List<string>();
                bool isFirstRow = true;

                string[] labels = { Keywords.ID_GROUP, Keywords.NAME, Keywords.ID_PARENT, Keywords.STATUS };
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

        public async Task<string> InsertGroup(string token, string data) {
            HttpResponseMessage response = await _httpUtil.PostAsync(token, APIs.GetFormByAlias(Keywords.GROUP), data);
            if (response == null || response.StatusCode != HttpStatusCode.Created) {
                return "{}";
            }

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}