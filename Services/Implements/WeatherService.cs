using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CentralizedDataSystem.Services.Implements {
    public class WeatherService : IWeatherService {
        public async Task<string> GetWeather(string owmAPIKey, string idCity) {
            HttpResponseMessage response = await HttpUtils.Instance.GetAsync(APIs.GetWeather(owmAPIKey, idCity));
            if (response == null) return "{}";

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}