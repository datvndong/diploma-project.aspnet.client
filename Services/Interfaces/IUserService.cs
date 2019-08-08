using CentralizedDataSystem.Models;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IUserService {
        Task<string> FindUserDataById(string token, string path, string id);
        Task<string> UpdateUserInfo(User user, string path);
        Task<long> CountUsers(string token, string idGroup);
        Task<string> FindUsersByPageAndIdGroup(string token, string idGroup, int page);
        Task<long> CountUsersByName(string token, string keyword);
        Task<string> FindUsersByPageAndName(string token, string keyword, int page);
        Task<string> InsertUser(string token, string data);
    }
}
