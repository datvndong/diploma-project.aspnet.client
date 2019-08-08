using CentralizedDataSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IFormService {
        Task<List<Form>> FindForms(string token, string email, int page);
        Task<string> FindFormWithToken(string token, string path);
        Task<string> BuildForm(string token, string formJSON, string path);
        Task<bool> DeleteForm(string token, string path);
        Task<string> FindFormWithNoToken(string path);
        Task<List<Form>> FindFormsCanStatistics(string token, string email);
    }
}
