using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Security;

namespace SasquatchCAIRS.Controllers.ViewControllers {
    /// <summary>
    ///     Provides Account-based Views.
    /// </summary>
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class AccountController : Controller {
        /// <summary>
        ///     Provides the Not Authenticated Page
        /// </summary>
        /// <param name="returnUrl">URL where request came from</param>
        /// <returns>The Not Authenticated View</returns>
        /// <request type="GET">/Account/Auth</request>
        [AllowAnonymous]
        public ActionResult Auth(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;

            if (returnUrl == "/") {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        ///     Provides the Manage Account Page
        /// </summary>
        /// <returns>The Manage Account View</returns>
        /// <request type="GET">/Account/Manage</request>
        [Authorize]
        public ActionResult Manage() {
            ViewBag.ReturnUrl = Url.Action("Manage");

            var uc = new UserManagementController();

            return View(uc.getUserProfile(User.Identity.Name));
        }
    }
}