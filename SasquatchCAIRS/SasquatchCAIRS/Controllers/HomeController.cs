using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers {
    public class HomeController : Controller {
        [Authorize]
        public ActionResult Index() {
            var db = new CAIRSDataContext();
            RequestLockController rlc = new RequestLockController();
            var keywords = new Dictionary<long, List<string>>();

            if (!User.IsInRole(Constants.Roles.VIEWER)) {
                ViewBag.Requests = null;
                return View();
            }

            // Select all from Requests
            IQueryable<Request> requests;

            // Create request list based on roles
            if (User.IsInRole(Constants.Roles.REQUEST_EDITOR)) {
                requests = db.Requests.Select(r => r).Where(
                    r =>
                    (Constants.RequestStatus) r.RequestStatus !=
                    Constants.RequestStatus.Invalid);
                //.Where(r => !rlc.isLocked(r.RequestID)); TODO: Fix this
            } else if (User.IsInRole(Constants.Roles.ADMINISTRATOR)) {
                requests = db.Requests.Select(r => r);
            } else {
                requests = db.Requests.Select(r => r).Where(
                    r =>
                    (Constants.RequestStatus) r.RequestStatus ==
                    Constants.RequestStatus.Completed);
                //.Where(r => !rlc.isLocked(r.RequestID)); TODO: Fix this
            }

            requests = requests.OrderBy(r => r.RequestStatus)
                .ThenByDescending(r => r.TimeOpened).Take(20);

            // Set the requests to null if there isn't anything on it,
            // as the view doesn't seem to have Any() available.
            if (!requests.Any()) {
                requests = null;
            }

            // Grab keywords for the requests
            if (requests != null) {
                foreach (Request rq in requests) {
                    List<string> kw =
                        (from kws in db.Keywords
                         join kqs in db.KeywordQuestions on kws.KeywordID equals
                             kqs.KeywordID
                         where kqs.RequestID == rq.RequestID
                         select kws.KeywordValue)
                            .ToList();
                    keywords.Add(rq.RequestID, kw);
                }
            }

            ViewBag.Requests = requests;
            ViewBag.Keywords = keywords;

            return View();
        }
    }
}