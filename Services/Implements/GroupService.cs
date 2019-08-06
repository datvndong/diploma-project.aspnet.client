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
    public class GroupService : IGroupService {
        public async Task<string> FindGroupFieldByIdGroup(string idGroup, string field) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "?select=data&data.idGroup=" + idGroup;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return Keywords.EMPTY_STRING;

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);
            if (jArray.Count == 0) return Keywords.EMPTY_STRING;
            JObject jObject = jArray.Children<JObject>().FirstOrDefault();
            JObject dataObject = (JObject)jObject.GetValue(Keywords.DATA);

            return dataObject.GetValue(field).ToString();
        }

        public async Task<string> FindGroupDataById(string id) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "/" + id;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<Group> FindGroupParent(string condition) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "?select=data&" + condition;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return null;

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);
            if (jArray.Count == 0) return null;
            JObject jObject = jArray.Children<JObject>().FirstOrDefault();
            JObject dataObject = (JObject)jObject.GetValue(Keywords.DATA);

            string id = jObject.GetValue(Keywords.ID).ToString();
            string idGroup = dataObject.GetValue(Keywords.ID_GROUP).ToString();
            string name = dataObject.GetValue(Keywords.NAME).ToString();
            string idParent = dataObject.GetValue(Keywords.ID_PARENT).ToString();
            string nameParent = Keywords.EMPTY_STRING;

            return new Group(id, idGroup, name, idParent, nameParent);
        }

        public async Task<List<Group>> FindListChildGroupByIdParentWithPage(string idParent, string nameParent, int page) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP);
            if (page > 0) {
                apiURI += "?limit=" + Configs.NUMBER_ROWS_PER_PAGE + "&skip=" + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE;
            } else {
                // If page = 0 => get full data
                apiURI += "?limit=" + Configs.LIMIT_QUERY;
            }
            apiURI += "&sort=-create&select=data&data.status=" + Configs.ACTIVE_STATUS + "&data.idParent=" + idParent;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return null;

            List<Group> groups = new List<Group>();

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);
            JObject dataObject = null;

            foreach (JObject jObject in jArray) {
                dataObject = (JObject)jObject.GetValue(Keywords.DATA);

                string id = jObject.GetValue(Keywords.ID).ToString();
                string idGroup = dataObject.GetValue(Keywords.ID_GROUP).ToString();
                string name = dataObject.GetValue(Keywords.NAME).ToString();
                int childSize = await FindNumberOfChildGroupByIdParent(idGroup);
                groups.Add(new Group(id, idGroup, name, idParent, nameParent, childSize));
            }

            return groups;
        }

        public async Task<int> FindNumberOfChildGroupByIdParent(string idParent) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "?limit=" + Configs.LIMIT_QUERY
                    + "&select=_id&data.status=" + Configs.ACTIVE_STATUS + "&data.idParent=" + idParent;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return 0;

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);

            return jArray.Count;
        }

        public async Task<string> FindGroupsByIdParentWhenCallAjax(string idParent) {
            string apiURI = APIs.GetListSubmissionsURL(Keywords.GROUP) + "?limit=" + Configs.LIMIT_QUERY
                    + "&sort=-create&select=data&data.status=" + Configs.ACTIVE_STATUS + "&data.idParent=" + idParent;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return null;

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> InsertGroup(string data) {
            HttpResponseMessage response = await HttpUtils.Instance.PostAsync(APIs.GetFormByAlias(Keywords.GROUP), data);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}