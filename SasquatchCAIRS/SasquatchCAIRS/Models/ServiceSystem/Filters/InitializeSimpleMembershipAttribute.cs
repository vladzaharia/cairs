using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using SasquatchCAIRS.Models;
using System.Web.Security;
using SasquatchCAIRS.Controllers;

namespace SasquatchCAIRS.Filters {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);

            // Add Profile Information to ViewBag for header
            dynamic viewBag = filterContext.Controller.ViewBag;
            UserProfileController profileController = new UserProfileController();
            viewBag.Profile = profileController.getUserProfile(filterContext.HttpContext.User.Identity.Name);
        }

        private class SimpleMembershipInitializer {
            public SimpleMembershipInitializer() {
                Database.SetInitializer<UsersContext>(null);

                try {
                    using (var context = new UsersContext()) {
                        if (!context.Database.Exists()) {
                            // Create the SimpleMembership database without 
                            // Entity Framework migration schema
                            ((IObjectContextAdapter) context)
                                .ObjectContext
                                .CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection(
                        "sasquatchConnectionString", 
                        "UserProfile", 
                        "UserId", 
                        "UserName", 
                        autoCreateTables: true);

                    // Initialize Roles
                    initializeRole("Administrator");
                    initializeRole("ReportGenerator");
                    initializeRole("RequestEditor");
                    initializeRole("Viewer");
                } catch (Exception ex) {
                    throw new InvalidOperationException(
                        "Database could not be initialized!", ex);
                }
            }

            private void initializeRole(string role) {
                if (!Roles.RoleExists(role)) {
                    Roles.CreateRole(role);
                }
            }
        }
    }
}
