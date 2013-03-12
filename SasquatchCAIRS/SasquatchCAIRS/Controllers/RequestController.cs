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
            var keywords = new List<string>();
            int timeSpent = 0;

            // Set up the Request Object
            RequestContent request = rmc.getRequestDetails(id);
            ViewBag.Title = "View Request - Request #" + request.requestID;

            // Show error if not editor and request isn't complete
            if (!User.IsInRole(Constants.Roles.REQUEST_EDITOR) &&
                request.requestStatus != Constants.RequestStatus.Completed) {
                request = null;
                ViewBag.Title = "View Request - Error";
            }

            // Show error if not administrator and request is locked
            if (rlc.isLocked(id) &&
                !User.IsInRole(Constants.Roles.ADMINISTRATOR)) {
                request = null;
                ViewBag.Title = "View Request - Error";
            }

            // Set up Keywords and Time Spent (Question-Dependent)
            if (request != null) {
                keywords.AddRange(
                    request.questionResponseList.Select(
                        qr =>
                        db.KeywordQuestions.Where(
                            kq =>
                            kq.QuestionResponseID == qr.questionResponseID &&
                            kq.RequestID == qr.requestID))
                           .SelectMany(
                               kqs =>
                               kqs.Select(
                                   kq =>
                                   db.Keywords.FirstOrDefault(
                                       k => k.KeywordID == kq.KeywordID))
                                  .Where(keyword => keyword != null))
                           .Select(keyword => keyword.KeywordValue));

                foreach (QuestionResponseContent qr in request.questionResponseList) {
                    timeSpent += qr.timeSpent.GetValueOrDefault(0);
                }
            }


            ViewBag.Keywords = keywords;
            ViewBag.TimeSpent = timeSpent; 

            return View(request);
        }
    }
}