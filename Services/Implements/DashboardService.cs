using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class DashboardService : IDashboardService {
        private readonly IWeatherService _weatherService;
        private readonly IFormControlService _formControlService;
        private readonly ISubmissionService _submissionService;

        public DashboardService(IWeatherService weatherService, IFormControlService formControlService, ISubmissionService submissionService) {
            _weatherService = weatherService;
            _formControlService = formControlService;
            _submissionService = submissionService;
        }

        public async Task<City> GetCityInfo() {
            string weatherRes = await _weatherService.GetWeather(Configs.OWM_API_KEY, Configs.ID_CITY);
            JObject weatherObj = JObject.Parse(weatherRes);
            if (weatherObj.Count == 0) return null;

            DateTime currDate = DateTime.Now;

            string weekday = currDate.DayOfWeek.ToString();
            string date = currDate.ToString(Configs.WEATHER_DATE_FORMAT, CultureInfo.InvariantCulture);
            string name = weatherObj.GetValue(Keywords.NAME).ToString();
            string country = ((JObject)weatherObj.GetValue(Keywords.SYS)).GetValue(Keywords.COUNTRY).ToString();

            double temperature = (int)((JObject)weatherObj.GetValue(Keywords.MAIN)).GetValue(Keywords.TEMP) - 273.15; // convert *K to *C
            double temperatureFormat = Double.Parse(temperature.ToString(Configs.DECIMAL_FORMAT));

            JArray jArray = (JArray)weatherObj.GetValue(Keywords.WEATHER);
            string description = ((JObject)jArray[0]).GetValue(Keywords.DESCRIPTION).ToString();

            string descriptionFormat = description.Substring(0, 1).ToUpper() + description.Substring(1);

            return new City(weekday, date, name, country, temperatureFormat, descriptionFormat);
        }

        public async Task<long> FindNumberGroups(string token) {
            return await _submissionService.CountSubmissions(token, Keywords.GROUP);
        }

        public async Task<long> FindNumberForms(string email) {
            List<FormControl> result = await _formControlService.FindByOwner(email);
            return result.Count;
        }

        public async Task<long> FindNumberUsers(string token) {
            return await _submissionService.CountSubmissions(token, Keywords.USER.ToLower());
        }
    }
}