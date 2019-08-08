using CentralizedDataSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IRoleService {
        Task<List<Role>> FindAll(string token);
        Task<Role> FindOne(string token, string _id);
    }
}
