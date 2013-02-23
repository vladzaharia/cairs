using SasquatchCAIRS.Filters;
using System.Web.Mvc;

namespace SasquatchCAIRS.Controllers {
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller {
        UserProfileManager profileManager = new UserProfileManager();
        
        //
        // GET: /Account/Auth

        [AllowAnonymous]
        public ActionResult Auth(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Profile = profileManager.getUserProfile(User.Identity.Name);

            if (returnUrl == "/") {
                return RedirectToLocal(returnUrl);
            } else {
                return View();
            }
        }

        //
        // GET: /Account/Manage
        [Authorize]
        public ActionResult Manage() {
            ViewBag.ReturnUrl = Url.Action("Manage");
            ViewBag.Profile = profileManager.getUserProfile(User.Identity.Name);

            return View();
        }
        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}
