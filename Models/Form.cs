using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralizedDataSystem.Models {
    public class Form {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public long Amount { get; set; }
        public string Start { get; set; }
        public string Expired { get; set; }
        public List<string> Tags { get; set; }
        public int DurationPercent { get; set; }
        public string TypeProgressBar { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsPending { get; set; }
        public bool IsAnonymousForm { get; set; }
        public Form(string name, string title, string path, long amount, string start, string expired,
            List<string> tags, int durationPercent, string typeProgressBar, bool isAnonymousForm) {
            this.Name = name;
            this.Title = title;
            this.Path = path;
            this.Amount = amount;
            this.Start = start;
            this.Expired = expired;
            this.Tags = tags;
            this.DurationPercent = durationPercent;
            this.TypeProgressBar = typeProgressBar;
            this.IsAnonymousForm = isAnonymousForm;
        }

        public Form(string title, string path, string start, string expired, List<string> tags, int durationPercent,
                string typeProgressBar, bool isSubmitted, bool isPending) {
            this.Title = title;
            this.Path = path;
            this.Start = start;
            this.Expired = expired;
            this.Tags = tags;
            this.DurationPercent = durationPercent;
            this.TypeProgressBar = typeProgressBar;
            this.IsSubmitted = isSubmitted;
            this.IsPending = isPending;
        }

        public Form(string title, string path) {
            this.Title = title;
            this.Path = path;
        }

        public Form() {
        }
    }
}