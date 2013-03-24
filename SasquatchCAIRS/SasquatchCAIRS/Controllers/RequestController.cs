using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers {
    public class RequestController : Controller {
        private CAIRSDataContext _db = new CAIRSDataContext();

        //
        // GET: /Request/Details/{id}

        [Authorize(Roles = Constants.Roles.ADMINISTRATOR)]
        public ActionResult Details(long id) {
            
            RequestManagementController rmc = new RequestManagementController();
            RequestLockController rlc = new RequestLockController();
            UserController upc = new UserController();
            int timeSpent = 0;

            // Set up the Request Object
            RequestContent request = rmc.getRequestDetails(id);
            ViewBag.Title = Constants.UIString.TitleText.VIEW_REQUEST 
                + " - " 
                + Constants.UIString.TitleText.REQUEST_NUM 
                + request.requestID;

            // Show error if not editor/administrator and request isn't complete
            if (!User.IsInRole(Constants.Roles.REQUEST_EDITOR) 
                 && !User.IsInRole(Constants.Roles.ADMINISTRATOR)
                 && request.requestStatus != Constants.RequestStatus.Completed) {
                ViewBag.Title = Constants.UIString.TitleText.VIEW_REQUEST 
                    + " - " 
                    + Constants.UIString.TitleText.ERROR;

                return View((object) null);
            }

            // Show error if not administrator and request is invalid (deleted)
            if (!User.IsInRole(Constants.Roles.ADMINISTRATOR)
                 && request.requestStatus == Constants.RequestStatus.Invalid) {
                ViewBag.Title = Constants.UIString.TitleText.VIEW_REQUEST
                    + " - "
                    + Constants.UIString.TitleText.ERROR;

                return View((object) null);
            }

            // Show error if you can't view due to locked status
            if (rlc.isLocked(id) &&
                !User.IsInRole(Constants.Roles.ADMINISTRATOR)) {

                // Check if it's not locked to you
                if (!User.IsInRole(Constants.Roles.REQUEST_EDITOR) ||
                    rlc.getRequestLock(id).UserID !=
                    upc.getUserProfile(User.Identity.Name).UserId) {
                    request = null;
                    ViewBag.Title = Constants.UIString.TitleText.VIEW_REQUEST
                                    + " - "
                                    + Constants.UIString.TitleText.ERROR;
                }

            }

            // Set up Time Spent (Question-Dependent)
            if (request != null) {
                foreach (QuestionResponseContent qr in request.questionResponseList) {
                    timeSpent += qr.timeSpent.GetValueOrDefault(0);
                }
            }

            ViewBag.TimeSpent = timeSpent;
            ViewBag.DataContext = _db;

            // add AuditLog entry for viewing
            AuditLogManagementController almc = new AuditLogManagementController();
            almc.addEntry(id, upc.getUserProfile(User.Identity.Name).UserId, Constants.AuditType.RequestView);

            return View(request);
        }

        //
        // GET: /Request/Unlock/{id}

        [Authorize(Roles = Constants.Roles.ADMINISTRATOR)]
        public ActionResult Unlock(long id) {
            var locks = _db.RequestLocks.Where(rl => rl.RequestID == id);

            foreach (RequestLock lck in locks) {
                _db.RequestLocks.DeleteOnSubmit(lck);
            }

            _db.SubmitChanges();

            return RedirectToAction("Index", "Home", new {
                status = Constants.URLStatus.Unlocked
            });
        }

        //
        // GET: /Request/Delete/{id}

        [Authorize(Roles = Constants.Roles.REQUEST_EDITOR)]
        public ActionResult Delete(long id) {
            var request = _db.Requests.FirstOrDefault(r => r.RequestID == id);

            if (request != null) {
                request.RequestStatus = (byte) Constants.RequestStatus.Invalid;
                _db.SubmitChanges();
            }

            return RedirectToAction("Index", "Home", new {
                status = Constants.URLStatus.Deleted
            });
        }

        //
        // GET: /Request/Export/{id}
        public ActionResult Export(long id) {
            WordExportController wec = new WordExportController();
            Request request = _db.Requests.FirstOrDefault(r => r.RequestID == id);

            DateTime markDate = new DateTime(2010, 01, 01, 00, 00, 00, 00);
            TimeSpan dateStamp = DateTime.Now.Subtract(markDate);
            string filePath = Server.MapPath(Constants.Export.REPORT_TEMP_PATH + dateStamp.TotalSeconds + ".docx");
            string templatePath = Server.MapPath(Constants.Export.REPORT_TEMPLATE_PATH);

            IEnumerable<string> output = wec.requestToStrings(request);
            wec.generateDocument(output, templatePath, filePath, id);

            ViewBag.export = "This Request should be exported to a .docx file shortly!";

            return View("Details", new RequestContent(request));
        }
    }
}