using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers {
    public class HomeController : Controller {
        [Authorize]
        public ActionResult Index() {
            CAIRSDataContext db = new CAIRSDataContext();
            RequestLockController rlc = RequestLockController.instance;
            Dictionary<long, List<string>> keywords = new Dictionary<long, List<string>>();

            // Select all from Requests
            IQueryable<Request> requests = db.Requests.Select(r => r);

            // Remove incomplete (open) requests
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

            // Set the requests to null if there isn't anything on it,
            // as the view doesn't seem to have Any() available.
            if (!requests.Any() || !User.IsInRole(Constants.Roles.VIEWER)) {
                requests = null;
            }

            // Grab keywords for the requests
            if (requests != null) {
                foreach (Request rq in requests) {
                    foreach (Keyword keyword in rq.QuestionResponses
                        .Select(qr => db.KeywordQuestions.Where(kq =>
                            kq.QuestionResponseID == qr.QuestionResponseID &&
                            kq.RequestID == qr.RequestID))
                        .SelectMany(kqs => kqs.Select(kq => 
                            db.Keywords.FirstOrDefault(
                                k => k.KeywordID == kq.KeywordID))
                        .Where(keyword => keyword != null))) {
                        if (keywords.ContainsKey(rq.RequestID)) {
                            keywords[rq.RequestID].Add(
                                keyword.KeywordValue);
                        } else {
                            List<string> kwlist = new List<string> { keyword.KeywordValue };
                            keywords.Add(rq.RequestID, kwlist);
                        }
                    }
                }
            }

            ViewBag.Requests = requests;
            ViewBag.Keywords = keywords;

            return View();
        }
    }
}