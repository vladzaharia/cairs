using System.Web.Mvc;
using System.Web.Security;

namespace SasquatchCAIRS.Controllers {
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller {
        UserProfileController profileController = new UserProfileController();
        
        //
        // GET: /Account/Auth

        [AllowAnonymous]
        public ActionResult Auth(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);

            if (returnUrl == "/") {
                return RedirectToLocal(returnUrl);
            }

            return View();
        }

        //
        // GET: /Account/Manage
        [Authorize]
        public ActionResult Manage() {
            ViewBag.ReturnUrl = Url.Action("Manage");
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);

            return View();
        }

        //
        // GET: /Account/RoleManager
        // ONLY HERE UNTIL ADMIN WORKS
        [Authorize]
        public ActionResult RoleManager(string roleAction, string role) {
            if (roleAction.Equals("add")) {
                Roles.AddUserToRole(User.Identity.Name, role);
            } else if (roleAction.Equals("remove")) {
                Roles.RemoveUserFromRole(User.Identity.Name, role);
            }

            return RedirectToLocal("/Account/Manage");
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
