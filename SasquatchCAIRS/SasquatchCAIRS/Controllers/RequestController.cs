using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Helper;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers
{
    public class RequestController : Controller
    {
        //
        // GET: /Request/Create/

        [Authorize(Roles = "RequestEditor")]
        public ActionResult Create() {
            DropdownController dc = DropdownController.instance;
            var reqContent = new RequestContent() {
                timeOpened = DateTime.Now
            };

            ViewBag.CurrentTime = DateTime.Now;
            ViewBag.RequestorTypes = new SelectList(
                dc.getActiveEntries(Constants.DropdownTable.RequestorType),
                "id", "text");
            ViewBag.Regions = new SelectList(
                dc.getActiveEntries(Constants.DropdownTable.Region),
                "id", "text");

            ViewBag.GenderOptions = new SelectList(Constants.genderOptions);

            return View(reqContent);
        }

        //
        // POST: /Request/Create/

        [HttpPost]
        [Authorize(Roles = "RequestEditor")]
        public ActionResult Create(RequestContent reqContent) {
            // TODO: Redirect to request page?
            if (!ModelState.IsValid) {
                return Create();
            } else {
                // Remove empty references
                foreach (var qrCon in reqContent.questionResponseList) {
                    qrCon.referenceList.RemoveAll(r => r.referenceString == null);
                }

                RequestManagementController rmc =
                    RequestManagementController.instance;
                rmc.create(reqContent);

                return Redirect("/Home/Index");
            }
        }

        [Authorize(Roles = "RequestEditor")]
        public ActionResult NewQuestionResponse() {
            DropdownController dc = DropdownController.instance;
            var qrContent = new QuestionResponseContent();

            // Used to set local question response ID
            ViewBag.Guid = HtmlPrefixScopeExtensions.CreateItemIndex(
                "questionResponseList");

            ViewBag.QuestionTypes = new SelectList(
                dc.getActiveEntries(Constants.DropdownTable.QuestionType),
                "id", "text");
            ViewBag.TumourGroups = new SelectList(
                dc.getActiveEntries(Constants.DropdownTable.TumourGroup),
                "id", "text");

            return View("Partial/NewQuestionResponse", qrContent);
        }

        [Authorize(Roles = "RequestEditor")]
        public ActionResult NewReference(string id) {
            var refContent = new ReferenceContent();

            // Used to set local reference ID
            ViewBag.Guid = id;
            ViewBag.RefGuid = HtmlPrefixScopeExtensions.CreateItemIndex(
                "questionResponseList[" + id + "].referenceList");
            ViewBag.HtmlIdPrefix = "questionResponseList_" + id +
                                   "__referenceList_" + ViewBag.RefGuid + "__";

            ViewBag.ReferenceTypes = new SelectList(
                Constants.referenceTypeOptions);

            return View("Partial/NewReference", refContent);
        }

        [Authorize]
        public ActionResult GetMatchingKeywords(string id) {
            DropdownController dc = DropdownController.instance;
            return Json(dc.getMatchingKeywords(id),
                JsonRequestBehavior.AllowGet);
        }
        
        //
        // GET: /Request/Details/{id}

        [Authorize(Roles = "Viewer")]
        public ActionResult Details(long id) {
            RequestManagementController rmc =
                RequestManagementController.instance;
            RequestLockController rlc = RequestLockController.instance;
            UserProfileController upc = UserProfileController.instance;
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

            return View(request);
        }
    }
}
