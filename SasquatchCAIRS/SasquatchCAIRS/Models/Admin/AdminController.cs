using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Security;

namespace SasquatchCAIRS.Controllers {
    [Authorize]
    public class AuditLogController : Controller {
        
        //
        // GET: /Admin/Audit

        [AllowAnonymous]
        public ActionResult Audit (string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;

            if (returnUrl == "/") {
                return RedirectToLocal(returnUrl);
            }

            return View();
        }

        //
        // GET: /Admin/LookupFields
        [Authorize]
        public ActionResult LookupFields() {
            ViewBag.ReturnUrl = Url.Action("LookupFields");

            return View();
        }

        //
        // GET: /Admin/Users
        [Authorize]
        public ActionResult Users(string roleAction, string role) {
            if (roleAction.Equals("add")) {
                Roles.AddUserToRole(User.Identity.Name, role);
            } else if (roleAction.Equals("remove")) {
                Roles.RemoveUserFromRole(User.Identity.Name, role);
            }

            return RedirectToLocal("/Account/Manage");
        }

          public class AuditReportGenerator
    {

        [DataType(DataType.Text)]
        public long requestId { get; set; }
        
        [DataType(DataType.Text)]
        public int userId { get; set; }

        [DataType(DataType.Date)]
        public DateTime startTime
        {
            get;
            set;
        }

        [DataType(DataType.Date)]
        public DateTime completionTime
        {
            get;
            set;
        }
    }

    public class SearchCriteriaContext : DbContext
    {
        public SearchCriteriaContext()
            : base("sasquatchConnectionString")
        {
        }

        public DbSet<SearchCriteria> SearchCriterias
        {
            get;
            set;
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
