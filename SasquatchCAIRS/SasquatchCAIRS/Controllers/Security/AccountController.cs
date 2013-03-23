using System.Web.Mvc;
using System.Web.Security;

namespace SasquatchCAIRS.Controllers.Security {
    [Authorize]
    public class AccountController : Controller {
        
        //
        // GET: /Account/Auth

        [AllowAnonymous]
        public ActionResult Auth(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;

            if (returnUrl == "/") {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //
        // GET: /Account/Manage
        [Authorize]
        public ActionResult Manage() {
            ViewBag.ReturnUrl = Url.Action("Manage");

            UserController uc = new UserController();

            return View(uc.getUserProfile(User.Identity.Name));
        }
    }
}
