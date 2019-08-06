using CentralizedDataSystem.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IStatisticsService {
        Task<List<Form>> FindFormsCanStatistics(string email);
        Task<JObject> AnalysisForm(string path);
    }
}
