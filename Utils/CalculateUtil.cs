using CentralizedDataSystem.Resources;
using System;

namespace CentralizedDataSystem.Utils {
    public class CalculateUtil {
        public static int GetDurationPercent(string start, string expired) {

            DateTime dateStart = DateTime.Parse(start);
            DateTime dateExpired = DateTime.Parse(expired);
            DateTime dateCurrent = DateTime.Now;

            if (DateTime.Compare(dateStart, dateCurrent) > 0) {
                return 0;
            }

            if (DateTime.Compare(dateExpired, dateCurrent) < 0) {
                return 100;
            }

            double diffStartExpired = (dateExpired - dateStart).TotalSeconds;
            double diffStartNow = (dateCurrent - dateStart).TotalSeconds;
            int percent = (int)Math.Round((double)diffStartNow / diffStartExpired * 100);

            return percent;
        }

        public static string GetTypeProgressBar(int percent) {
            if (percent < 25) {
                return Keywords.SUCCESS;
            } else if (percent < 50) {
                return Keywords.PRIMARY;
            } else if (percent < 75) {
                return Keywords.WARNING;
            } else if (percent < 100) {
                return Keywords.INFO;
            }
            return Keywords.DANGER;
        }

        public static bool IsFormPendingOrExpired(string dateCompareStr) {
            DateTime dateCompare = DateTime.Parse(dateCompareStr);
            DateTime dateNow = DateTime.Now;
            return DateTime.Compare(dateCompare, dateNow) >= 0;
        }
    }
}