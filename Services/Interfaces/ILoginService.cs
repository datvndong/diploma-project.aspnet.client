using System.Net.Http;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface ILoginService {
        Task<HttpResponseMessage> CheckLogin(string email, string password);
        Task<bool> Logout();
    }
}
