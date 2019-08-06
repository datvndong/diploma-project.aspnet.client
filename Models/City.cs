using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralizedDataSystem.Models {
    public class City {
        public string Weekday { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public double Temperature { get; set; }
        public string Description { get; set; }
        public City(string weekday, string date, string name, string country, double temperature, string description) {
            this.Weekday = weekday;
            this.Date = date;
            this.Name = name;
            this.Country = country;
            this.Temperature = temperature;
            this.Description = description;
        }

        public City() {
        }
    }
}