using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers {
    public class HomeController : Controller {
        [Authorize]
        public ActionResult Index() {
            var db = new CAIRSDataContext();
            RequestLockController rlc = RequestLockController.instance;

            // Select all from Requests
            IQueryable<Request> requests = db.Requests.Select(r => r);

            // Remove uncompleted Completed
            if (!User.IsInRole(Constants.Roles.REQUEST_EDITOR)) {
                requests = requests.Where(
                    r =>
                    (Constants.RequestStatus) r.RequestStatus !=
                    Constants.RequestStatus.Open);
            }

            // Remove locked+invalid requests
            if (!User.IsInRole(Constants.Roles.ADMINISTRATOR)) {
                requests = requests
                    .Where(r =>
                           (Constants.RequestStatus) r.RequestStatus !=
                           Constants.RequestStatus.Invalid);
                //.Where(r => !rlc.isLocked(r.RequestID)); TODO: Fix this
            }

            requests = requests.OrderByDescending(r => r.TimeOpened).Take(20);

            // TODO: Keywords

            // Set the requests to null if there isn't anything on it,
            // as the view doesn't seem to have Any() available.
            if (!requests.Any() || !User.IsInRole(Constants.Roles.VIEWER)) {
                requests = null;
            }
            ViewBag.Requests = requests;

            return View();
        }

        [Authorize]
        public ActionResult About() {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}