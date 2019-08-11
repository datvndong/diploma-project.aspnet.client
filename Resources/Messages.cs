namespace CentralizedDataSystem.Resources {
    public class Messages {
        public static readonly string SERVER_ERROR = "Internal Server Error.";
        public static readonly string INVALID_ACCOUNT_ERROR = "Invalid username or password.";
        public static readonly string LOGIN_TO_CONTINUE = "Sorry, your session has expired. Please login again.";
        public static readonly string DEACTIVE_USER = "Sorry, your account has been disabled. Please try another one.";

        public static readonly string DATABASE_ERROR = "Database error.";
        public static readonly string FORMAT_DATE_ERROR = "Format date error.";
        public static readonly string DATE_PICK_ERROR = "Start date must be before expired date.";

        public static readonly string HAS_SUBMITTED_MESSAGE = "Thank you, your submission has been received!";
        public static readonly string HAS_NOT_SUBMITTED_MESSAGE = "Sorry, you have not submitted to this report.";

        public static readonly string NO_COMPONENTS_IN_FORM = "Can't create or modified form because there is no component information.";
        public static readonly string COULD_NOT_CONNECT_API_SERVER = "Could not connect to API server.";

        public static readonly string DELETE_SUCCESSFUL = "Successfully deleted datas!";
        public static readonly string DELETE_FAILED = "Error, failed to delete datas.";
        public static readonly string IMPORT_SUCCESSFUL = "Successfully imported datas!";
        public static readonly string IMPORT_FAILED = "Error, failed to import datas.";

        public static string FILL(string field) {
            return "Please fill out `" + field + "` field.";
        }

        public static readonly string UNAUTHORIZED_MESSAGE = "Sorry, we couldn't confirm it's you.";

        public static readonly string MAIL_TITLE = "Notification from C.D System.";
        public static string MAIL_SUBJECT(string form) {
            return "You've got new report named \"" + form + "\"";
        }
        public static readonly string SEND_MAIL_SUCCESSFUL = "Successfully send email to users!";
    }
}