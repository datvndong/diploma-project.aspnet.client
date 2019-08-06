using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CentralizedDataSystem {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{page}",
                defaults: new { controller = "Login", action = "Index", page = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Submission",
                url: "Submission/Index/{path}/{page}",
                defaults: new { controller = "Submission", action = "Index", path = UrlParameter.Optional, page = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Group",
                url: "group/Index/{idParent}/{page}",
                defaults: new { controller = "group", action = "Index", idParent = UrlParameter.Optional, page = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "User",
                url: "User/Index/{idGroup}/{page}",
                defaults: new { controller = "User", action = "Index", idGroup = UrlParameter.Optional, page = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Auth",
                url: "Send/Auth/Report/{path}",
                defaults: new { controller = "Report", action = "Auth", path = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Anon",
                url: "Send/Anon/Report/{path}",
                defaults: new { controller = "Report", action = "Anon", path = UrlParameter.Optional }
            );
        }
    }
}
