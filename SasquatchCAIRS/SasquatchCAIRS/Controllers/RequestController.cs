using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Helper;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers
{
    /// <summary>
    /// TODO
    /// </summary>
    public class RequestController : Controller
    {
        //
        // GET: /Request/Create/

        [Authorize(Roles = "RequestEditor")]
        public ActionResult Create() {
            DropdownController dc = new DropdownController();
            var reqContent = new RequestContent {
                timeOpened = DateTime.Now
            };

            ViewBag.RequestorTypes = new SelectList(
                dc.getEntries(Constants.DropdownTable.RequestorType),
                "id", "text");
            ViewBag.Regions = new SelectList(
                dc.getEntries(Constants.DropdownTable.Region),
                "id", "text");

            ViewBag.GenderOptions = new SelectList(Constants.genderOptions);

            return View(reqContent);
        }

        //
        // POST: /Request/Create/

        [HttpPost]
        [Authorize(Roles = "RequestEditor")]
        public ActionResult Create(RequestContent reqContent) {
            RequestManagementController rmc = new RequestManagementController();

            bool valid = ModelState.IsValid;

            if (reqContent.parentRequestID != null &&
                !rmc.requestExists((long) reqContent.parentRequestID)) {

                ModelState.AddModelError("NonexistentParentRequest",
                    "Parent Request ID must correspond to an existing request.");
                valid = false;
            }
            
            if (Request.Form["mark_as_complete"] != null) {
                foreach (var qrContent in reqContent.questionResponseList) {
                    if (String.IsNullOrEmpty(qrContent.question) ||
                        String.IsNullOrEmpty(qrContent.response) || 
                        qrContent.questionTypeID == null ||
                        qrContent.tumourGroupID == null ||
                        qrContent.timeSpent == null ||
                        qrContent.severity == null ||
                        qrContent.consequence == null ||
                        qrContent.keywords.Count < 1) {
                        
                        ModelState.AddModelError("IncompleteQuestion",
                            "Questions must be completed before marking request as complete.");
                        valid = false;
                        break;
                    }

                    foreach (var refContent in qrContent.referenceList) {
                        if (String.IsNullOrEmpty(refContent.referenceString)) {
                            ModelState.AddModelError("IncompleteReference",
                                "References must be completed before marking request as complete.");
                            valid = false;
                            break;
                        }
                    }
                }

                reqContent.timeClosed = DateTime.Now;
                reqContent.requestStatus = Constants.RequestStatus.Completed;
            }

            // Encode HTML in question responses
            // Replace null references with empty string
            foreach (var qrContent in reqContent.questionResponseList) {
                if (!String.IsNullOrEmpty(qrContent.question)) {
                    qrContent.question = HttpUtility.HtmlEncode(
                        removeNewLinesAndTabs(qrContent.question))
                        .Replace("&#39;", "'");
                }
                if (!String.IsNullOrEmpty(qrContent.response)) {
                    qrContent.response = HttpUtility.HtmlEncode(
                        removeNewLinesAndTabs(qrContent.response))
                        .Replace("&#39;", "'");
                }
                if (!String.IsNullOrEmpty(qrContent.specialNotes)) {
                    qrContent.specialNotes = HttpUtility.HtmlEncode(
                        removeNewLinesAndTabs(qrContent.specialNotes))
                        .Replace("&#39;", "'");
                }

                foreach (var refContent in qrContent.referenceList.Where(
                    refContent => refContent.referenceString == null)) {

                    refContent.referenceString = "";
                }
            }

            if (!valid) {
                DropdownController dc = new DropdownController();

                ViewBag.RequestorTypes = new SelectList(
                    dc.getEntries(Constants.DropdownTable.RequestorType),
                    "id", "text");
                ViewBag.Regions = new SelectList(
                    dc.getEntries(Constants.DropdownTable.Region),
                    "id", "text");

                ViewBag.GenderOptions = new SelectList(Constants.genderOptions);

                return View(reqContent);
            }

            long reqId = rmc.create(reqContent);

            // TODO: Audit log


            if (Roles.IsUserInRole(Constants.Roles.VIEWER)) {
                return RedirectToAction("Details", "Request",
                    new {
                        id = reqId
                    });
            }

            return RedirectToAction("Index", "Home",
                new {
                    status = Constants.URLStatus.SuccessfulCreate
                });
        }

        //
        // GET: /Request/Edit/

        [Authorize(Roles = "RequestEditor")]
        public ActionResult Edit(long id) {
            RequestLockController rlc = new RequestLockController();
            UserController uc = new UserController();
            UserProfile up = uc.getUserProfile(User.Identity.Name);

            RequestLock rl = rlc.getRequestLock(id);
            if (rl == null) {
                rlc.addLock(id, up.UserId);
            } else if (rl.UserID != up.UserId) {
                // Locked to someone else, redirect
                return RedirectToAction("Index", "Home", new {
                    status = Constants.URLStatus.AccessingLocked
                });
            }

            var dc = new DropdownController();
            var rmc = new RequestManagementController();

            var reqContent = rmc.getRequestDetails(id);

            ViewBag.RequestorTypes = new SelectList(
                dc.getEntries(Constants.DropdownTable.RequestorType),
                "id", "text");
            ViewBag.Regions = new SelectList(
                dc.getEntries(Constants.DropdownTable.Region),
                "id", "text");

            ViewBag.GenderOptions = new SelectList(Constants.genderOptions);

            // TODO: Audit log

            return View(reqContent);
        }

        //
        // POST: /Request/Create/

        [HttpPost]
        [Authorize(Roles = "RequestEditor")]
        public ActionResult Edit(RequestContent reqContent) {
            RequestManagementController rmc = new RequestManagementController();
            RequestLockController rlc = new RequestLockController();
            UserController uc = new UserController();

            UserProfile up = uc.getUserProfile(User.Identity.Name);

            RequestLock rl = rlc.getRequestLock(reqContent.requestID);
            if (rl == null) {
                return RedirectToAction("Index", "Home", new {
                    status = Constants.URLStatus.NotLockedToYou
                });
            } else if (rl.UserID != up.UserId) {
                return RedirectToAction("Index", "Home", new {
                    status = Constants.URLStatus.AccessingLocked
                });
            }

            if (Request.Form["delete"] != null) {
                rmc.invalidate(reqContent.requestID);

                return RedirectToAction("Index", "Home", new {
                    status = Constants.URLStatus.Deleted
                });
            }

            bool valid = ModelState.IsValid;

            if (reqContent.parentRequestID != null &&
                !rmc.requestExists((long) reqContent.parentRequestID)) {

                ModelState.AddModelError("NonexistentParentRequest",
                    "Parent Request ID must correspond to an existing request.");
                valid = false;
            }
            
            if (Request.Form["mark_as_complete"] != null) {
                foreach (var qrContent in reqContent.questionResponseList) {
                    if (String.IsNullOrEmpty(qrContent.question) ||
                        String.IsNullOrEmpty(qrContent.response) ||
                        qrContent.questionTypeID == null ||
                        qrContent.tumourGroupID == null ||
                        qrContent.timeSpent == null ||
                        qrContent.severity == null ||
                        qrContent.consequence == null ||
                        qrContent.keywords.Count < 1) {

                        ModelState.AddModelError("IncompleteQuestion",
                            "Questions must be completed before marking request as complete.");
                        valid = false;
                        break;
                    }

                    foreach (var refContent in qrContent.referenceList) {
                        if (String.IsNullOrEmpty(refContent.referenceString)) {
                            ModelState.AddModelError("IncompleteReference",
                                "References must be completed before marking request as complete.");
                            valid = false;
                            break;
                        }
                    }
                }

                reqContent.timeClosed = DateTime.Now;
                reqContent.requestStatus = Constants.RequestStatus.Completed;
            }

            // Encode HTML in question responses
            // Replace null references with empty string
            foreach (var qrContent in reqContent.questionResponseList) {
                if (!String.IsNullOrEmpty(qrContent.question)) {
                    qrContent.question = HttpUtility.HtmlEncode(
                        removeNewLinesAndTabs(qrContent.question))
                        .Replace("&#39;", "'");
                }
                if (!String.IsNullOrEmpty(qrContent.response)) {
                    qrContent.response = HttpUtility.HtmlEncode(
                        removeNewLinesAndTabs(qrContent.response))
                        .Replace("&#39;", "'");
                }
                if (!String.IsNullOrEmpty(qrContent.specialNotes)) {
                    qrContent.specialNotes = HttpUtility.HtmlEncode(
                        removeNewLinesAndTabs(qrContent.specialNotes))
                        .Replace("&#39;", "'");
                }

                foreach (var refContent in qrContent.referenceList.Where(
                    refContent => refContent.referenceString == null)) {

                    refContent.referenceString = "";
                }
            }

            if (!valid) {
                DropdownController dc = new DropdownController();

                ViewBag.RequestorTypes = new SelectList(
                    dc.getEntries(Constants.DropdownTable.RequestorType),
                    "id", "text");
                ViewBag.Regions = new SelectList(
                    dc.getEntries(Constants.DropdownTable.Region),
                    "id", "text");

                ViewBag.GenderOptions = new SelectList(Constants.genderOptions);

                return View(reqContent);
            }

            rmc.edit(reqContent);
            rlc.removeLock(reqContent.requestID);

            // TODO: Audit log

            if (Roles.IsUserInRole(Constants.Roles.VIEWER)) {
                return RedirectToAction("Details", "Request",
                    new {id = reqContent.requestID});
            }

            return RedirectToAction("Index", "Home",
                new {status = Constants.URLStatus.SuccessfulEdit});
        }

        [HttpPost]
        public ActionResult NewQuestionResponse(String json) {
            // Don't want to load a partial with a Not Authorized message
            if (!Roles.IsUserInRole(Constants.Roles.REQUEST_EDITOR)) {
                return RedirectToAction("Index", "Home", new {
                    status = Constants.URLStatus.NoRequestEditorRole
                });
            }

            DropdownController dc = new DropdownController();
            QuestionResponseContent qrContent;

            if (String.IsNullOrEmpty(json)) {
                qrContent = new QuestionResponseContent();
            } else {
                qrContent = new JavaScriptSerializer()
                        .Deserialize<QuestionResponseContent>(json);
            }

            // Used to set dynamic model binding index
            ViewBag.Guid = HtmlPrefixScopeExtensions.CreateItemIndex(
                "questionResponseList");

            ViewBag.QuestionTypes = new SelectList(
                dc.getEntries(Constants.DropdownTable.QuestionType),
                "id", "text");
            ViewBag.TumourGroups = new SelectList(
                dc.getEntries(Constants.DropdownTable.TumourGroup),
                "id", "text");
            ViewBag.Severitys = new SelectList(Constants.severityOptions);
            ViewBag.Consequences = new SelectList(Constants.consequenceOptions);

            return View("Partial/NewQuestionResponse", qrContent);
        }

        [HttpPost]
        [Authorize(Roles = "RequestEditor")]
        public ActionResult NewReference(string id, string json) {
            // Don't want to load a partial with a Not Authorized message
            if (!Roles.IsUserInRole(Constants.Roles.REQUEST_EDITOR)) {
                return RedirectToAction("Index", "Home", new {
                    status = Constants.URLStatus.NoRequestEditorRole
                });
            }

            ReferenceContent refContent;

            if (String.IsNullOrEmpty(json)) {
                refContent = new ReferenceContent();
            } else {
                refContent =
                    new JavaScriptSerializer().Deserialize<ReferenceContent>(
                        json);
            }
             
            // Used to set dynamic model binding index
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
            DropdownController dc = new DropdownController();
            return Json(dc.getMatchingKeywords(id),
                JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Request/Details/{id}

        [Authorize]
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
            ViewBag.DataContext = new CAIRSDataContext();

            return View(request);
        }

        //
        // GET: /Request/Unlock/{id}

        [Authorize(Roles = Constants.Roles.ADMINISTRATOR)]
        public ActionResult Unlock(long id) {
            CAIRSDataContext db = new CAIRSDataContext();

            var locks = db.RequestLocks.Where(rl => rl.RequestID == id);

            foreach (RequestLock lck in locks) {
                db.RequestLocks.DeleteOnSubmit(lck);
            }

            db.SubmitChanges();

            return RedirectToAction("Index", "Home", new {
                status = Constants.URLStatus.Unlocked
            });
        }

        //
        // GET: /Request/Delete/{id}

        [Authorize(Roles = Constants.Roles.REQUEST_EDITOR)]
        public ActionResult Delete(long id) {
            CAIRSDataContext db = new CAIRSDataContext();

            var request = db.Requests.FirstOrDefault(r => r.RequestID == id);

            if (request != null) {
                request.RequestStatus = (byte) Constants.RequestStatus.Invalid;
                db.SubmitChanges();
            }

            return RedirectToAction("Index", "Home", new {
                status = Constants.URLStatus.Deleted
            });
        }

        private String removeNewLinesAndTabs(String s) {
            s = s.Replace("\n", String.Empty);
            s = s.Replace("\r", String.Empty);
            s = s.Replace("\t", String.Empty);

            return s;
        }
    }
}
