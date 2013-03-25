using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SasquatchCAIRS {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Custom Dropdown Routes
            routes.MapRoute(
                name: "DropdownList",
                url: "Admin/Dropdown/List",
                defaults: new {
                    controller = "Admin",
                    action = "Dropdowns"
                });
            routes.MapRoute(
                name: "DropdownEdit",
                url: "Admin/Dropdown/Edit/{table}/{id}",
                defaults: new {
                    controller = "Admin",
                    action = "DropdownEdit",
                    table = UrlParameter.Optional,
                    id = UrlParameter.Optional
                });
            routes.MapRoute(
                name: "DropdownCreate",
                url: "Admin/Dropdown/Create/{table}",
                defaults: new {
                    controller = "Admin",
                    action = "DropdownCreate",
                    table = UrlParameter.Optional
                });

            // Custom User Routes
            routes.MapRoute(
                name: "UserList",
                url: "Admin/User/List",
                defaults: new {
                    controller = "Admin",
                    action = "Users"
                });
            routes.MapRoute(
                name: "UserEdit",
                url: "Admin/User/Edit/{id}",
                defaults: new {
                    controller = "Admin",
                    action = "UserEdit",
                    id = UrlParameter.Optional
                });

            routes.MapRoute(
                name: "AuditGenerate",
                url: "Admin/Audit/Generate",
                defaults: new {
                    controller = "Admin",
                    action = "GenerateAudit"
                });

            // Custom Report Routes
            routes.MapRoute(
                name: "Report",
                url: "Report",
                defaults: new {
                    controller = "ServiceSystem",
                    action = "Reports"
                });
            routes.MapRoute(
                name: "ReportDownload",
                url: "Report/Generate",
                defaults: new {
                    controller = "ServiceSystem",
                    action = "GeneratingReport"
                });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}