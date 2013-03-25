using System.Web;
using System.Web.Optimization;

namespace SasquatchCAIRS {
    public class BundleConfig {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Bootstrap
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/bootstrap*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap*"));

            // Bootstrap Datepicker
            bundles.Add(new StyleBundle("~/Content/datepicker").Include("~/Content/datepicker*"));
            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include("~/Scripts/datepicker*"));
            // Flat-UI
            bundles.Add(new StyleBundle("~/Content/flat-ui").Include("~/Content/flat-ui.css"));
            bundles.Add(new ScriptBundle("~/bundles/flat-ui").Include("~/Scripts/flat-ui/*.js"));

            // Font Awesome
            bundles.Add(new StyleBundle("~/Content/fontawesome").Include("~/Content/font-awesome.css"));
            bundles.Add(new StyleBundle("~/Content/fontawesome-ie7").Include("~/Content/font-awesome-ie7.css"));

            // CKEditor
            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                        "~/Scripts/ckeditor/ckeditor.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/ie7").Include("~/Content/ie7.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}