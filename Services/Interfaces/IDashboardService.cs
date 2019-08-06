using CentralizedDataSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IDashboardService {
        Task<City> GetCityInfo();
        Task<long> FindNumberGroups();
        Task<long> FindNumberForms(string email);
        Task<long> FindNumberUsers();
    }
}
