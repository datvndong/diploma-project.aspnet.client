using System.Configuration;

namespace CentralizedDataSystem.Utils {
    public class ConfigUtil {
        public static string GetKeyConfig(string key) {
            return ConfigurationManager.AppSettings[key].ToString();
        }
    }
}