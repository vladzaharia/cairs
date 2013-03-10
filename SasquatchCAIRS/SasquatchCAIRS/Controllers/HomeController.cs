using System.Web.Mvc;

namespace SasquatchCAIRS.Controllers {
    public class HomeController : Controller {
        [Authorize]
        public ActionResult Index() {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            return View();
        }

        [Authorize]
        public ActionResult About() {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        [Authorize (Roles="Administrator")]
        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
