using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class StatisticsService : IStatisticsService {
        private readonly IHttpUtil _httpUtil;
        private readonly IFormService _formService;
        private readonly ISubmissionService _submissionService;

        public StatisticsService(IHttpUtil httpUtil, IFormService formService, ISubmissionService submissionService) {
            _httpUtil = httpUtil;
            _formService = formService;
            _submissionService = submissionService;
        }

        private void CountValue(JArray jArray, JObject data, string typeComponent) {
            JToken valueObj = null;
            JArray bluePrintDatasArr = null;
            bool valueObjInBool = false;
            foreach (JObject obj in jArray) {
                string key = obj.GetValue(Keywords.KEY).ToString();
                valueObj = data.GetValue(key);
                if (valueObj == null) continue;

                switch (typeComponent) {
                    case "checkbox":
                        valueObjInBool = valueObj.ToObject<Boolean>();
                        if (valueObjInBool) {
                            int count = (int)obj.GetValue(Keywords.COUNT) + 1;
                            obj[Keywords.COUNT] = count;
                        }
                        break;
                    case "selectboxes":
                        JObject resDataObj = valueObj.ToObject<JObject>();
                        bluePrintDatasArr = (JArray)obj.GetValue(Keywords.DATA);
                        foreach (JObject dataObj in bluePrintDatasArr) {
                            valueObj = resDataObj.GetValue(dataObj.GetValue(Keywords.KEY).ToString());
                            if (valueObj != null) {
                                valueObjInBool = valueObj.ToObject<Boolean>();
                                if (valueObjInBool) {
                                    int count = (int)dataObj.GetValue(Keywords.COUNT) + 1;
                                    dataObj[Keywords.COUNT] = count;
                                }
                            }
                        }
                        break;
                    case "select":
                    case "radio":
                        string resDataValue = valueObj.ToString();
                        bluePrintDatasArr = (JArray)obj.GetValue(Keywords.DATA);
                        foreach (JObject dataObj in bluePrintDatasArr) {
                            if (dataObj.GetValue(Keywords.KEY).ToString().Equals(resDataValue)) {
                                int count = (int)dataObj.GetValue(Keywords.COUNT) + 1;
                                dataObj[Keywords.COUNT] = count;
                                break;
                            }
                        }
                        break;
                }
            }
        }

        public async Task<List<Form>> FindFormsCanStatistics(string token, string email) {
            List<Form> result = await _formService.FindFormsCanStatistics(token, email);
            return result;
        }

        public async Task<JObject> AnalysisForm(string token, string path) {
            JObject analysis = new JObject();

            string formsRes = await _formService.FindFormWithToken(token, path);
            JObject formResObj = JObject.Parse(formsRes);
            JArray components = (JArray)formResObj.GetValue(Keywords.COMPONENTS);
            foreach (JObject compObj in components) {
                string type = compObj.GetValue(Keywords.TYPE).ToString();
                bool isValidType = type.Equals(Keywords.CHECKBOX) || type.Equals(Keywords.SELECTBOXES)
                    || type.Equals(Keywords.SELECT) || type.Equals(Keywords.RADIO);
                if (!isValidType) {
                    continue;
                }

                if (analysis.GetValue(type) == null) {
                    analysis.Add(type, new JArray());
                }
                JArray typesArr = (JArray)analysis.GetValue(type);

                JObject jObject = new JObject();
                if (type.Equals(Keywords.CHECKBOX)) {
                    jObject.Add(Keywords.LABEL, compObj.GetValue(Keywords.LABEL).ToString());
                    jObject.Add(Keywords.KEY, compObj.GetValue(Keywords.KEY).ToString());
                    jObject.Add(Keywords.COUNT, 0);
                } else {
                    // select || selectboxes || radio
                    jObject.Add(Keywords.LABEL, compObj.GetValue(Keywords.LABEL).ToString());
                    jObject.Add(Keywords.KEY, compObj.GetValue(Keywords.KEY).ToString());
                    jObject.Add(Keywords.DATA, new JArray());

                    JArray datas = type.Equals(Keywords.SELECT)
                            ? ((JArray)((JObject)compObj.GetValue(Keywords.DATA)).GetValue(Keywords.VALUES))
                            : (JArray)compObj.GetValue(Keywords.VALUES);
                    foreach (JObject dataObj in datas) {
                        dataObj.Add(Keywords.KEY, dataObj.GetValue(Keywords.VALUE).ToString());
                        dataObj.Remove(Keywords.VALUE);
                        dataObj.Add(Keywords.COUNT, 0);
                        ((JArray)jObject.GetValue(Keywords.DATA)).Add(dataObj);
                    }
                }
                typesArr.Add(jObject);
            }

            string submissionsRes = await _submissionService.FindAllSubmissions(token, path, true);
            JArray submissionsResArr = JArray.Parse(submissionsRes);
            analysis.Add(Keywords.AMOUNT, submissionsResArr.Count);
            JArray jArray = null;
            foreach (JObject obj in submissionsResArr) {
                JObject data = (JObject)(obj.GetValue(Keywords.DATA));

                if (analysis.ContainsKey(Keywords.CHECKBOX)) {
                    jArray = (JArray)analysis.GetValue(Keywords.CHECKBOX);
                    CountValue(jArray, data, Keywords.CHECKBOX);
                }

                if (analysis.ContainsKey(Keywords.SELECTBOXES)) {
                    jArray = (JArray)analysis.GetValue(Keywords.SELECTBOXES);
                    CountValue(jArray, data, Keywords.SELECTBOXES);
                }

                if (analysis.ContainsKey(Keywords.SELECT)) {
                    jArray = (JArray)analysis.GetValue(Keywords.SELECT);
                    CountValue(jArray, data, Keywords.SELECT);
                }

                if (analysis.ContainsKey(Keywords.RADIO)) {
                    jArray = (JArray)analysis.GetValue(Keywords.RADIO);
                    CountValue(jArray, data, Keywords.RADIO);
                }
            }

            return analysis;
        }
    }
}