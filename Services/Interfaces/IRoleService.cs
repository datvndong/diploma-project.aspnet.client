using CentralizedDataSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IRoleService {
        Task<List<Role>> FindAll();
        Task<Role> FindOne(string _id);
    }
}
