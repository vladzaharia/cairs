using System.Web.Mvc;

namespace SasquatchCAIRS.Controllers {
    [InitializeSimpleMembership]
    public class HomeController : Controller {
        UserProfileController profileController = new UserProfileController();

        [Authorize]
        public ActionResult Index() {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);
            return View();
        }

        [Authorize]
        public ActionResult About() {
            ViewBag.Message = "Your app description page.";
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);
            return View();
        }

        [Authorize (Roles="Administrator")]
        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);
            return View();
        }
    }
}
