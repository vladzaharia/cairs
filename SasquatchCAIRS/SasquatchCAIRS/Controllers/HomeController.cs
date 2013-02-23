using System.Web.Mvc;

namespace SasquatchCAIRS.Controllers {
    public class HomeController : Controller {
        UserProfileManager profileManager = new UserProfileManager();

        public ActionResult Index() {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            ViewBag.Profile = profileManager.getUserProfile(User.Identity.Name);
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your app description page.";
            ViewBag.Profile = profileManager.getUserProfile(User.Identity.Name);
            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";
            ViewBag.Profile = profileManager.getUserProfile(User.Identity.Name);
            return View();
        }
    }
}
