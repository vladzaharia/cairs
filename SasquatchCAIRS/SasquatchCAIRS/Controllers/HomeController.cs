using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers {
    [ExcludeFromCodeCoverage]
    public class HomeController : Controller {
        [Authorize]
        public ActionResult Index(Constants.URLStatus status = Constants.URLStatus.None) {
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
            if (User.IsInRole(Constants.Roles.ADMINISTRATOR)) {
                requests = db.Requests.Select(r => r);
            } else if (User.IsInRole(Constants.Roles.REQUEST_EDITOR)) {
                requests = db.Requests.Select(r => r).Where(
                    r =>
                    (Constants.RequestStatus) r.RequestStatus !=
                    Constants.RequestStatus.Invalid).Where(
                    r => !db.RequestLocks
                        .Any(rl => rl.RequestID == r.RequestID && 
                            rl.UserProfile.UserName != User.Identity.Name));
            } else {
                requests = db.Requests.Select(r => r).Where(
                    r =>
                    (Constants.RequestStatus) r.RequestStatus ==
                    Constants.RequestStatus.Completed).Where(
                    r => !db.RequestLocks
                        .Any(rl => rl.RequestID == r.RequestID && 
                            rl.UserProfile.UserName != User.Identity.Name));
            }

            requests = requests.OrderBy(r => r.RequestStatus)
                .ThenByDescending(r => r.TimeOpened).Take(10);

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
                         select kws.KeywordValue).Distinct()
                            .ToList();
                    keywords.Add(rq.RequestID, kw);
                }
            }

            ViewBag.Requests = requests;
            ViewBag.Keywords = keywords;
            if (status == Constants.URLStatus.Expired) {
                ViewBag.Status = 
                    "Your session has expired due to inactivity. All unsaved changes have been lost.";
                ViewBag.StatusColor = "danger";
            } else if (status == Constants.URLStatus.Unlocked) {
                ViewBag.Status =
                    "The request has now been unlocked and is available for editing by all users.";
                ViewBag.StatusColor = "success";
            } else if (status == Constants.URLStatus.Deleted) {
                ViewBag.Status =
                    "The request has been marked as invalid and cannot be seen by non-Administrators.";
                ViewBag.StatusColor = "success";
            } else if (status == Constants.URLStatus.AccessingLocked) {
                ViewBag.Status =
                    "The request is locked and cannot be edited.";
                ViewBag.StatusColor = "danger";
            } else if (status == Constants.URLStatus.NotLockedToYou) {
                ViewBag.Status =
                    "The request is not locked to you and cannot be edited.";
                ViewBag.StatusColor = "danger";
            } else if (status == Constants.URLStatus.SuccessfulEdit) {
                ViewBag.Status =
                    "The request has been successfully edited.";
                ViewBag.StatusColor = "success";
            } else if (status == Constants.URLStatus.NoRequestEditorRole) {
                ViewBag.Status =
                    "You no longer have permissions to create or edit requests.";
                ViewBag.StatusColor = "danger";
            } else if (status == Constants.URLStatus.SuccessfulCreate) {
                ViewBag.Status =
                    "The request has been successfully created.";
                ViewBag.StatusColor = "success";
            } else if (status == Constants.URLStatus.EditingInvalid) {
                ViewBag.Status =
                    "The request is marked as invalid and cannot be edited.";
                ViewBag.StatusColor = "danger";
            }

            return View();
        }

        //
        // GET: /Index/Error
        public ActionResult Error() {
            return View("Error");
        }
    }
}
