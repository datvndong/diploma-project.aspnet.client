using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class WeatherService : IWeatherService {
        private readonly IHttpUtil _httpUtil;

        public WeatherService(IHttpUtil httpUtil) {
            _httpUtil = httpUtil;
        }

        public async Task<string> GetWeather(string owmAPIKey, string idCity) {
            HttpResponseMessage response = await _httpUtil.GetAsync(APIs.GetWeather(owmAPIKey, idCity));
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}