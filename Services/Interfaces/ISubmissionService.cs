using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface ISubmissionService {
        Task<string> FindSubmissionsByPage(string token, string path, int page);
        Task<long> CountSubmissions(string token, string path);
        Task<string> FindAllSubmissions(string token, string path, bool isGetOnlyData);
    }
}
