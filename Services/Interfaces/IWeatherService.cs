using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IWeatherService {
        Task<string> GetWeather(string owmAPIKey, string idCity);
    }
}
