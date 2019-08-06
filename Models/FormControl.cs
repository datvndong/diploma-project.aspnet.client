using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralizedDataSystem.Models {
    public class FormControl {
        public string PathForm { get; set; }
        public string Owner { get; set; }
        public string Assign { get; set; }
        public string Start { get; set; }
        public string Expired { get; set; }
        public FormControl(string pathForm, string owner, string assign, string start, string expired) {
            this.PathForm = pathForm;
            this.Owner = owner;
            this.Assign = assign;
            this.Start = start;
            this.Expired = expired;
        }

        public FormControl() {
        }
    }
}