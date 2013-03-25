using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SasquatchCAIRS
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Session_Start(object sender, EventArgs e) {}

        protected void Session_End(object sender, EventArgs e) {
            var db = new CAIRSDataContext();

            List<RequestLock> locks =
                db.RequestLocks.Where(
                    l => l.StartTime.AddMinutes(30) > DateTime.Now).ToList();

            db.RequestLocks.DeleteAllOnSubmit(locks);
            db.SubmitChanges();
        }
    }
}