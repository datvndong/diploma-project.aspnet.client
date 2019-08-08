using CentralizedDataSystem.Models;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IDashboardService {
        Task<City> GetCityInfo();
        Task<long> FindNumberGroups(string token);
        Task<long> FindNumberForms(string email);
        Task<long> FindNumberUsers(string token);
    }
}
