using System;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Models.Common;
using WebMatrix.WebData;
using System.Web.Security;
using SasquatchCAIRS.Models;

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
            UserManagementController profileController = new UserManagementController();
            viewBag.Profile = profileController.loginAndGetUserProfile(filterContext.HttpContext.User.Identity.Name);
        }

        private class SimpleMembershipInitializer {
            public SimpleMembershipInitializer() {
                try {
                    using (var context = new CAIRSDataContext()) {
                        if (!context.DatabaseExists()) {
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
                    initializeRole(Constants.Roles.ADMINISTRATOR);
                    initializeRole(Constants.Roles.REPORT_GENERATOR);
                    initializeRole(Constants.Roles.REQUEST_EDITOR);
                    initializeRole(Constants.Roles.VIEWER);
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
