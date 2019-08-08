using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IWeatherService {
        Task<string> GetWeather(string owmAPIKey, string idCity);
    }
}
