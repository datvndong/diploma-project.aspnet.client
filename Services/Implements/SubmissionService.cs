using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class SubmissionService : ISubmissionService {
        private readonly IHttpUtil _httpUtil;

        public SubmissionService(IHttpUtil httpUtil) {
            _httpUtil = httpUtil;
        }

        public async Task<string> FindSubmissionsByPage(string token, string path, int page) {
            string apiURI = APIs.GetListSubmissionsURL(path) + "?select=data&limit=" + Configs.NUMBER_ROWS_PER_PAGE + "&skip="
                    + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE;

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null) return "[]";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<long> CountSubmissions(string token, string path) {
            string apiURI = APIs.GetListSubmissionsURL(path) + "?limit=" + Configs.LIMIT_QUERY + "&select=_id";

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null) return 0;

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);

            return jArray.Count;
        }

        public async Task<string> FindAllSubmissions(string token, string path, bool isGetOnlyData) {
            string apiURI = APIs.GetListSubmissionsURL(path) + "?limit=" + Configs.LIMIT_QUERY;
            if (isGetOnlyData) {
                apiURI += "&select=data";
            }

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}