using CentralizedDataSystem.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IStatisticsService {
        Task<List<Form>> FindFormsCanStatistics(string token, string email);
        Task<JObject> AnalysisForm(string token, string path);
    }
}
