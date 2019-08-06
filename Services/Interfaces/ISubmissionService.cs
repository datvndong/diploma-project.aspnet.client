using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface ISubmissionService {
        Task<string> FindSubmissionsByPage(string path, int page);
        Task<long> CountSubmissions(string path);
        Task<string> FindAllSubmissions(string path, bool isGetOnlyData);
    }
}
