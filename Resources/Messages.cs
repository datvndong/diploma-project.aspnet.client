using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralizedDataSystem.Resources {
    public class Messages {
        public static readonly string SERVER_ERROR = "Internal Server Error.";
        public static readonly string INVALID_ACCOUNT_ERROR = "Invalid username or password.";
        public static readonly string LOGIN_TO_CONTINUE = "Please login to continue.";
        public static readonly string DEACTIVE_USER = "Sorry, your account has been disabled. Please try another one.";

        public static readonly string DATABASE_ERROR = "Database error.";
        public static readonly string FORMAT_DATE_ERROR = "Format date error.";
        public static readonly string DATE_PICK_ERROR = "Start date must be before expired date.";

        public static readonly string HAS_SUBMITTED_MESSAGE = "Thank you, your submission has been received!";
        public static readonly string HAS_NOT_SUBMITTED_MESSAGE = "Sorry, you have not submitted to this report.";

        public static readonly string NO_COMPONENTS_IN_FORM = "Can't create or modified form because there is no component information.";
        public static readonly string COULD_NOT_CONNECT_API_SERVER = "Could not connect to API server.";

        public static string DELETE(string resource, bool isDeleteSuccess) {
            return isDeleteSuccess ? "Successfully deleted " + resource + "!" : "Error, failed to delete " + resource + ".";
        }

        public static string IMPORT(bool isImportSuccess) {
            return isImportSuccess ? "Successfully imported datas to Database!"
                    : "Error, failed to delete imported datas to Database.";
        }

        public static string FILL(string field) {
            return "Please fill out `" + field + "` field.";
        }

        public static readonly string UNAUTHORIZED_MESSAGE = "Sorry, we couldn't confirm it's you.";
    }
}