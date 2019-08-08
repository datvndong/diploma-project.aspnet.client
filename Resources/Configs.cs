namespace CentralizedDataSystem.Resources {
    public class Configs {
        public static readonly string MONGO_CONNECTION_STRING = "mongodb://localhost:27017";
        public static readonly string DATABASE_NAME = "formioapp";

        public static readonly string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public static readonly string DATE_FORMAT = "yyyy-MM-dd";
        public static readonly string WEATHER_DATE_FORMAT = "dd MMM, yyyy";
        public static readonly string WEEKDAY_FORMAT = "EEEE";

        public static readonly string DECIMAL_FORMAT = "##.##";

        public static readonly int NUMBER_ROWS_PER_PAGE = 10;
        public static readonly string DEFAULT_FILE_NAME = "export.";

        public static readonly int DEACTIVE_STATUS = 0;
        public static readonly int ACTIVE_STATUS = 1;

        public static readonly string LIMIT_QUERY = "1000000000";

        public static readonly int HTTP_LIFE_TIME = 5;
        public static readonly int COOKIE_LIFE_TIME = 2;

        public static readonly string CRYPTO_PASSWORD = "XtremeMemory";

        public static readonly string OWM_API_KEY = "cf76b373a6c28e3253b49e1a8f04beb7";
        public static readonly string ID_CITY = "1580541";
    }
}