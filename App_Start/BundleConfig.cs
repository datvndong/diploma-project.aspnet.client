using System.Web.Optimization;

namespace CentralizedDataSystem {
    public class BundleConfig {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            // Projects bundles
            bundles.Add(new StyleBundle("~/bundles/css/common").Include(
                      "~/Content/style.css",
                      "~/vendors/iconfonts/mdi/css/materialdesignicons.min.css",
                      "~/vendors/css/vendor.bundle.base.css",
                      "~/vendors/css/vendor.bundle.addons.css",
                      "~/images/favicon.png"));

            bundles.Add(new ScriptBundle("~/bundles/js/common").Include(
                        "~/vendors/js/vendor.bundle.base.js",
                        "~/vendors/js/vendor.bundle.addons.js",
                        "~/Scripts/off-canvas.js",
                        "~/Scripts/misc.js"));

            bundles.Add(new StyleBundle("~/bundles/css/builder").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/formio.full.min.css",
                      "~/fonts/font-awesome.min.css",
                      "~/fonts/fontawesome-webfont.ttf",
                      "~/fonts/fontawesome-webfont.woff",
                      "~/fonts/fontawesome-webfont.woff2",
                      "~/fonts/glyphicons-halflings-regular.ttf",
                      "~/fonts/glyphicons-halflings-regular.woff",
                      "~/fonts/glyphicons-halflings-regular.woff2"));

            bundles.Add(new ScriptBundle("~/bundles/js/builder").Include(
                        "~/Scripts/formio.full.min.js",
                        "~/Scripts/jquery.min.js"));

            bundles.Add(new StyleBundle("~/bundles/css/report").Include(
                      "~/Content/formio.full.min.css",
                      "~/Content/style.css",
                      "~/Content/material-kit.css",
                      "~/images/favicon.png",
                      "~/vendors/iconfonts/mdi/css/materialdesignicons.min.css",
                      "~/vendors/css/vendor.bundle.base.css",
                      "~/vendors/css/vendor.bundle.addons.css",
                      "~/fonts/bootstrap-glyphicons.css"));

            bundles.Add(new StyleBundle("~/bundles/css/profile").Include(
                      "~/vendors/nucleo/css/nucleo.css",
                      "~/vendors/@fortawesome/fontawesome-free/css/all.min.css",
                      "~/Content/argon.css"));
        }
    }
}
