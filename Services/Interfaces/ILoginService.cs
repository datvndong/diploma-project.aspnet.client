using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface ILoginService {
        Task<HttpResponseMessage> CheckLogin(string email, string password);
        Task<bool> Logout();
    }
}
