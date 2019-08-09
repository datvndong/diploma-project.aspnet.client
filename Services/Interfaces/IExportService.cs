using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IExportService {
        Task<string> ExportSubmissionsDataToString(string token, string path, string type);
    }
}
