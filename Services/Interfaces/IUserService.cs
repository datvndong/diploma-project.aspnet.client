using CentralizedDataSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IUserService {
        Task<string> FindUserDataById(string token, string path, string id);
        Task<string> UpdateUserInfo(User user, string path);
        Task<long> CountUsers(string token, string idGroup);
        Task<string> FindUsersByPageAndIdGroup(string token, string idGroup, int page);
        Task<long> CountUsersByName(string token, string keyword);
        Task<string> FindUsersByPageAndName(string token, string keyword, int page);
        List<string> GetListUsersFromFile(string pathFile);
        Task<string> InsertUser(string token, string data);
        Task<string> FindAllUsers(string token);
    }
}
