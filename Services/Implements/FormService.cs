using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using CentralizedDataSystem.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class FormService : IFormService {
        private readonly IHttpUtil _httpUtil;
        private readonly IFormControlService _formControlService;
        private readonly ISubmissionService _submissionService;

        public FormService(IHttpUtil httpUtil, IFormControlService formControlService, ISubmissionService submissionService) {
            _httpUtil = httpUtil;
            _formControlService = formControlService;
            _submissionService = submissionService;
        }

        public async Task<List<Form>> FindForms(string token, string email, int page) {
            List<Form> list = new List<Form>();
            string apiURI = APIs.FORM_URL + "?type=form&sort=-created&owner=" + email + "&limit=" + Configs.NUMBER_ROWS_PER_PAGE
                + "&skip=" + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE + "&select=name,title,path,tags";

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return list;
            }

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);
            foreach (JObject jObject in jArray) {

                string name = jObject.GetValue(Keywords.NAME).ToString();
                string title = jObject.GetValue(Keywords.TITLE).ToString();
                string path = jObject.GetValue(Keywords.PATH).ToString();
                long amount = await _submissionService.CountSubmissions(token, path);

                FormControl formControl = await _formControlService.FindByPathForm(path);
                if (formControl == null) return list;

                string start = formControl.Start;
                string expired = formControl.Expired;
                string assign = formControl.Assign;

                List<string> tags = new List<string>();
                foreach (string tag in JArray.Parse(jObject.GetValue(Keywords.TAGS).ToString())) {
                    tags.Add(tag);
                }

                int durationPercent = CalculateUtil.GetDurationPercent(start, expired);
                string typeProgressBar = CalculateUtil.GetTypeProgressBar(durationPercent);
                list.Add(new Form(name, title, path, amount, start, expired, tags, durationPercent, typeProgressBar,
                        assign.Equals(Keywords.ANONYMOUS)));
            }

            return list;
        }

        public async Task<string> FindFormWithToken(string token, string path) {
            string apiURI = APIs.GetFormByAlias(path);

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return "{}";
            }

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> BuildForm(string token, string formJSON, string path) {
            HttpResponseMessage response = null;
            string content = null;

            if (string.Empty.Equals(path)) {
                // Create
                response = await _httpUtil.PostAsync(token, APIs.FORM_URL, formJSON);
            } else {
                // Edit
                response = await _httpUtil.PutAsync(token, APIs.ModifiedForm(path), formJSON);
            }

            if (response == null || !response.IsSuccessStatusCode) {
                return "{}";
            }

            content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<bool> DeleteForm(string token, string path) {
            HttpResponseMessage response = await _httpUtil.DeleteAsync(token, APIs.ModifiedForm(path));
            if (response == null) return false;

            return response.IsSuccessStatusCode;
        }

        public async Task<string> FindFormWithNoToken(string path) {
            // Use to get form with Anonymous assign
            HttpResponseMessage response = await _httpUtil.GetAsync(APIs.GetFormByAlias(path));
            if (response == null || !response.IsSuccessStatusCode) {
                return "{}";
            }

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<List<Form>> FindFormsCanStatistics(string token, string email) {
            List<Form> list = new List<Form>();

            string apiURI = APIs.FORM_URL + "?type=form&sort=-created&owner=" + email + "&limit=" + Configs.LIMIT_QUERY
                    + "&select=title,path,components";

            HttpResponseMessage response = await _httpUtil.GetAsync(token, apiURI);
            if (response == null || !response.IsSuccessStatusCode) {
                return list;
            }

            string content = await response.Content.ReadAsStringAsync();

            JArray jArray = JArray.Parse(content);
            foreach (JObject jObject in jArray) {
                JArray components = (JArray)jObject.GetValue(Keywords.COMPONENTS);
                foreach (JObject compObj in components) {
                    string type = compObj.GetValue(Keywords.TYPE).ToString();
                    if (type.Equals(Keywords.CHECKBOX) || type.Equals(Keywords.SELECTBOXES)
                        || type.Equals(Keywords.SELECT) || type.Equals(Keywords.RADIO)) {
                        string title = jObject.GetValue(Keywords.TITLE).ToString();
                        string path = jObject.GetValue(Keywords.PATH).ToString();
                        list.Add(new Form(title, path));
                        break;
                    }
                }
            }

            return list;
        }
    }
}