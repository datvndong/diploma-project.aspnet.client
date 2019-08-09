using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class ExportService : IExportService {
        private readonly ISubmissionService _submissionService;

        public ExportService(ISubmissionService submissionService) {
            _submissionService = submissionService;
        }

        public async Task<string> ExportSubmissionsDataToString(string token, string path, string type) {
            StringBuilder stringBuilder = new StringBuilder();

            string submissionRes = await _submissionService.FindAllSubmissions(token, path, false);
            JArray jArray = JArray.Parse(submissionRes);

            if (type.Equals(Keywords.JSON)) {
                // Write JSON file
                foreach (JObject jObject in jArray) {
                    stringBuilder.Append(jObject.ToString());
                    stringBuilder.Append(Environment.NewLine);
                }
            } else {
                // Write CSV file
                List<string> line = null;
                List<string[]> dataLines = new List<string[]>();
                JObject dataObject = null;
                bool isFirstWrite = true;

                foreach (JObject jObject in jArray) {
                    // Add row header
                    if (isFirstWrite) {
                        line = new List<string> {
                            Keywords.ID, Keywords.CREATED, Keywords.MODIFIED
                        };

                        dataObject = (JObject)jObject.GetValue(Keywords.DATA);
                        foreach (JProperty property in dataObject.Properties()) {
                            line.Add(property.Name);
                        }

                        dataLines.Add(line.ToArray());

                        isFirstWrite = false;
                    }

                    // Add row data
                    line = new List<string> {
                        jObject.GetValue(Keywords.ID).ToString(),
                        jObject.GetValue(Keywords.CREATED).ToString(),
                        jObject.GetValue(Keywords.MODIFIED).ToString()
                    };

                    dataObject = (JObject)jObject.GetValue(Keywords.DATA);
                    foreach (JProperty property in dataObject.Properties()) {
                        line.Add(property.Value.ToString());
                    }

                    dataLines.Add(line.ToArray());
                }

                foreach (string[] data in dataLines) {
                    stringBuilder.Append(string.Join(",", data));
                    stringBuilder.Append(Environment.NewLine);
                }
            }

            return stringBuilder.ToString();
        }
    }
}