using CentralizedDataSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IUserService {
        Task<string> FindUserDataById(string path, string id);
        Task<string> UpdateUserInfo(User user, string path);
        Task<long> CountUsers(string idGroup);
        Task<string> FindUsersByPageAndIdGroup(string idGroup, int page);
        Task<long> CountUsersByName(string keyword);
        Task<string> FindUsersByPageAndName(string keyword, int page);
        Task<string> InsertUser(string data);
    }
}
