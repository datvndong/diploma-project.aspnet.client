using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CentralizedDataSystem.Services.Implements {
    public class DashboardService : IDashboardService {
        private readonly IWeatherService weatherService;
        private readonly IFormControlService formControlService;
        private readonly ISubmissionService submissionService;

        public DashboardService(IWeatherService weatherService, IFormControlService formControlService, ISubmissionService submissionService) {
            this.weatherService = weatherService;
            this.formControlService = formControlService;
            this.submissionService = submissionService;
        }

        public async Task<City> GetCityInfo() {
            string owmAPIKey = "cf76b373a6c28e3253b49e1a8f04beb7";
            string idCity = "1580541";

            string weatherRes = await weatherService.GetWeather(owmAPIKey, idCity);
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

        public async Task<long> FindNumberGroups() {
            return await submissionService.CountSubmissions(Keywords.GROUP);
        }

        public async Task<long> FindNumberForms(string email) {
            List<FormControl> result = await formControlService.FindByOwner(email);
            return result.Count;
        }

        public async Task<long> FindNumberUsers() {
            return await submissionService.CountSubmissions(Keywords.USER.ToLower());
        }
    }
}