using CentralizedDataSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IFormService {
        Task<List<Form>> FindForms(string email, int page);
        Task<string> FindFormWithToken(string path);
        Task<string> BuildForm(string formJSON, string path);
        Task<bool> DeleteForm(string path);
        Task<string> FindFormWithNoToken(string path);
        Task<List<Form>> FindFormsCanStatistics(string email);
    }
}
