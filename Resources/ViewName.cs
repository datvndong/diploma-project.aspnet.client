using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralizedDataSystem.Resources {
    public class ViewName {
        public static readonly string LOGIN = "~/Views/Login/Index.cshtml";

        public static readonly string ERROR_403 = "~/Views/Base/Error403.cshtml";
        public static readonly string ERROR_404 = "~/Views/Base/Error404.cshtml";
        public static readonly string ERROR_500 = "~/Views/Base/Error500.cshtml";
        public static readonly string ERROR_UNKNOWN = "~/Views/Base/ErrorUnknown.cshtml";

        public static readonly string MODIFIED_FORM = "~/Views/Form/Modified.cshtml";

        public static readonly string SEND_REPORT = "~/Views/Report/Send.cshtml";
        public static readonly string EDIT_REPORT = "~/Views/Report/Edit.cshtml";
    }
}