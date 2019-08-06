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
    public class SubmissionService : ISubmissionService {
        public async Task<string> FindSubmissionsByPage(string path, int page) {
            string apiURI = APIs.GetListSubmissionsURL(path) + "?select=data&limit=" + Configs.NUMBER_ROWS_PER_PAGE + "&skip="
                    + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE;

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return "[]";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<long> CountSubmissions(string path) {
            string apiURI = APIs.GetListSubmissionsURL(path) + "?limit=" + Configs.LIMIT_QUERY + "&select=_id";

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return 0;

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);

            return jArray.Count;
        }

        public async Task<string> FindAllSubmissions(string path, bool isGetOnlyData) {
            string apiURI = APIs.GetListSubmissionsURL(path) + "?limit=" + Configs.LIMIT_QUERY;
            if (isGetOnlyData) {
                apiURI += "&select=data";
            }

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}