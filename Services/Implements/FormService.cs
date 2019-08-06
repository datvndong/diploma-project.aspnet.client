using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace CentralizedDataSystem.Services.Implements {
    public class FormService : IFormService {
        private readonly IFormControlService formControlService;
        private readonly ISubmissionService submissionService;

        public FormService(IFormControlService formControlService, ISubmissionService submissionService) {
            this.formControlService = formControlService;
            this.submissionService = submissionService;
        }

        public async Task<List<Form>> FindForms(string email, int page) {
            List<Form> list = new List<Form>();
            string apiURI = APIs.FORM_URL + "?type=form&sort=-created&owner=" + email + "&limit=" + Configs.NUMBER_ROWS_PER_PAGE
                + "&skip=" + (page - 1) * Configs.NUMBER_ROWS_PER_PAGE + "&select=name,title,path,tags";

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return list;

            string content = await response.Content.ReadAsStringAsync();
            JArray jArray = JArray.Parse(content);
            foreach (JObject jObject in jArray) {

                string name = jObject.GetValue(Keywords.NAME).ToString();
                string title = jObject.GetValue(Keywords.TITLE).ToString();
                string path = jObject.GetValue(Keywords.PATH).ToString();
                long amount = await submissionService.CountSubmissions(path);

                FormControl formControl = await formControlService.FindByPathForm(path);
                if (formControl == null) return list;

                string start = formControl.Start;
                string expired = formControl.Expired;
                string assign = formControl.Assign;

                List<string> tags = new List<string>();
                foreach (string tag in JArray.Parse(jObject.GetValue(Keywords.TAGS).ToString())) {
                    tags.Add(tag);
                }

                int durationPercent = CalculateUtils.GetDurationPercent(start, expired);
                string typeProgressBar = CalculateUtils.GetTypeProgressBar(durationPercent);
                list.Add(new Form(name, title, path, amount, start, expired, tags, durationPercent, typeProgressBar,
                        assign.Equals(Keywords.ANONYMOUS)));
            }

            return list;
        }

        public async Task<string> FindFormWithToken(string path) {
            string apiURI = APIs.GetFormByAlias(path);

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> BuildForm(string formJSON, string path) {
            HttpResponseMessage response = null;
            string content = null;

            if (path.Equals("")) {
                // Create
                response = await HttpUtils.Instance.PostAsync(APIs.FORM_URL, formJSON);
            } else {
                // Edit
                response = await HttpUtils.Instance.PutAsync(APIs.ModifiedForm(path), formJSON);
            }

            if (response == null) return "{}";

            content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<bool> DeleteForm(string path) {
            HttpResponseMessage response = await HttpUtils.Instance.DeleteAsync(APIs.ModifiedForm(path));
            if (response == null) return false;

            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<string> FindFormWithNoToken(string path) {
            // Use to get form with Anonymous assign
            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(APIs.GetFormByAlias(path));
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<List<Form>> FindFormsCanStatistics(string email) {
            List<Form> list = new List<Form>();

            string apiURI = APIs.FORM_URL + "?type=form&sort=-created&owner=" + email + "&limit=" + Configs.LIMIT_QUERY
                    + "&select=title,path,components";

            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(apiURI);
            if (response == null) return list;

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