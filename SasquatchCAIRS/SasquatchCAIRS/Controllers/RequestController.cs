using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers {
    public class RequestController : Controller {
        //
        // GET: /Request/Get/{id}

        [Authorize(Roles = "Viewer")]
        public ActionResult Details(long id) {
            var db = new CAIRSDataContext();
            RequestManagementController rmc =
                RequestManagementController.instance;
            RequestLockController rlc = RequestLockController.instance;
            List<string> keywords = new List<string>();

            RequestContent request = rmc.getRequestDetails(id);
            ViewBag.Title = "View Request - Request #" + request.requestID;

            if (!User.IsInRole(Constants.Roles.REQUEST_EDITOR) &&
                request.requestStatus != Constants.RequestStatus.Completed) {
                request = null;
                ViewBag.Title = "View Request - Error";
            }

            if (rlc.isLocked(id) &&
                !User.IsInRole(Constants.Roles.ADMINISTRATOR)) {
                request = null;
                ViewBag.Title = "View Request - Error";
            }

            if (request != null) {
                foreach (Keyword keyword in request.questionResponseList
                            .Select(qr => db.KeywordQuestions
                            .Where(kq => kq.QuestionResponseID == qr.questionResponseID
                                && kq.RequestID == qr.requestID))
                            .SelectMany(kqs => kqs.Select(kq =>
                                db.Keywords.FirstOrDefault(k =>
                                    k.KeywordID == kq.KeywordID))
                            .Where(keyword => keyword != null))) {
                    keywords.Add(keyword.KeywordValue);
                }
            }

            ViewBag.Keywords = keywords;

            return View(request);
        }
    }
}