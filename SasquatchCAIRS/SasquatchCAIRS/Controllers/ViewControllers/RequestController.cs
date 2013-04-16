using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using SasquatchCAIRS.Controllers.Export;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Helper;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Service;

namespace SasquatchCAIRS.Controllers.ViewControllers {
    /// <summary>
    ///     Provides the Request Views for the System.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RequestController : Controller {
        /// <summary>
        ///     Create a new Request page
        /// </summary>
        /// <returns>The Request Create View</returns>
        /// <request type="GET">/Request/Create</request>
        [Authorize(Roles = Constants.Roles.REQUEST_EDITOR)]
        public ActionResult Create() {
            var dc = new DropdownManagementController();
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

        /// <summary>
        ///     Proccess the Create Request Page.
        /// </summary>
        /// <param name="reqContent">The RequestContent to Add</param>
        /// <returns>The Request Create View if errors exist, and Request View/Dashboard otherwise.</returns>
        /// <request type="POST">/Request/Create</request>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.REQUEST_EDITOR)]
        public ActionResult Create(RequestContent reqContent) {
            var rmc = new RequestManagementController();

            bool valid = ModelState.IsValid;

            if (reqContent.parentRequestID != null &&
                !rmc.requestExists((long) reqContent.parentRequestID)) {
                ModelState.AddModelError("NonexistentParentRequest",
                                         "Parent Request ID must correspond to an existing request.");
                valid = false;
            }

            if (Request.Form["mark_as_complete"] != null) {
                foreach (
                    QuestionResponseContent qrContent in
                        reqContent.questionResponseList) {
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

                    if (qrContent.keywords.Any(keyword => keyword.Length > 128)) {
                        ModelState.AddModelError("KeywordTooLong",
                                                 "Keywords must be less than 128 characters.");
                        valid = false;
                    }

                    if (qrContent.referenceList.Any(refContent => String.IsNullOrEmpty(refContent.referenceString))) {
                        ModelState.AddModelError("IncompleteReference",
                                                 "References must be completed before marking request as complete.");
                        valid = false;
                    }
                }

                reqContent.timeClosed = DateTime.Now;
                reqContent.requestStatus = Constants.RequestStatus.Completed;
            }

            // Encode HTML in question responses
            // Replace null references with empty string
            foreach (
                QuestionResponseContent qrContent in
                    reqContent.questionResponseList) {
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

                foreach (
                    ReferenceContent refContent in
                        qrContent.referenceList) {

                    if (refContent.referenceString == null) {
                        refContent.referenceString = "";
                    } else {
                        refContent.referenceString = refContent.referenceString.Replace("\\", "\\\\");
                    }
                }
            }

            if (!valid) {
                var dc = new DropdownManagementController();

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

            var uc = new UserManagementController();
            UserProfile up = uc.getUserProfile(User.Identity.Name);
            var almc = new AuditLogManagementController();
            almc.addEntry(reqId, up.UserId,
                          Constants.AuditType.RequestCreation,
                          reqContent.timeOpened);

            if (reqContent.requestStatus == Constants.RequestStatus.Completed &&
                reqContent.timeClosed != null) {
                almc.addEntry(reqId, up.UserId,
                              Constants.AuditType.RequestCompletion,
                              (DateTime) reqContent.timeClosed);
            }

            if (Roles.IsUserInRole(Constants.Roles.VIEWER)) {
                return RedirectToAction("Details", "Request",
                                        new {
                                            id = reqId
                                        });
            }

            return RedirectToAction("Index", "Home",
                                    new {
                                        status =
                                        Constants.URLStatus.SuccessfulCreate
                                    });
        }

        /// <summary>
        ///     Page for editing an existing request
        /// </summary>
        /// <param name="id">The Request ID</param>
        /// <returns>The Request Edit View</returns>
        /// <request type="GET">/Request/Edit</request>
        [Authorize(Roles = Constants.Roles.REQUEST_EDITOR)]
        public ActionResult Edit(long id) {
            var rlc = new RequestLockManagementController();
            var uc = new UserManagementController();
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

            var dc = new DropdownManagementController();
            var rmc = new RequestManagementController();

            RequestContent reqContent = rmc.getRequestDetails(id);

            if (reqContent.requestStatus == Constants.RequestStatus.Invalid) {
                // Invalid request, cannot edit
                return RedirectToAction("Index", "Home", new {
                    status = Constants.URLStatus.EditingInvalid
                });
            }

            ViewBag.RequestorTypes = new SelectList(
                dc.getEntries(Constants.DropdownTable.RequestorType),
                "id", "text");
            ViewBag.Regions = new SelectList(
                dc.getEntries(Constants.DropdownTable.Region),
                "id", "text");

            ViewBag.GenderOptions = new SelectList(Constants.genderOptions);

            return View(reqContent);
        }

        /// <summary>
        ///     Proccess the request from a Request Edit page
        /// </summary>
        /// <param name="reqContent">The RequestContent to edit.</param>
        /// <returns>The Request Edit View if there were errors, the Request View or Dashboard page otherwise.</returns>
        /// <request type="POST">/Request/Edit</request>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.REQUEST_EDITOR)]
        public ActionResult Edit(RequestContent reqContent) {
            var rmc = new RequestManagementController();
            var rlc = new RequestLockManagementController();
            var uc = new UserManagementController();
            var almc = new AuditLogManagementController();

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
                rlc.removeLock(reqContent.requestID);

                almc.addEntry(reqContent.requestID, up.UserId,
                              Constants.AuditType.RequestDeletion);

                return RedirectToAction("Index", "Home", new {
                    status = Constants.URLStatus.Deleted
                });
            }

            if (Request.Form["cancel"] != null) {
                rlc.removeLock(reqContent.requestID);

                if (Roles.IsUserInRole(Constants.Roles.VIEWER)) {
                    return RedirectToAction(
                        "Details", "Request",
                        new {
                            id = reqContent.requestID
                        });
                }

                return RedirectToAction(
                    "Index", "Home",
                    new {
                        status =
                            Constants.URLStatus.SuccessfulEdit
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
                foreach (
                    QuestionResponseContent qrContent in
                        reqContent.questionResponseList) {
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

                    if (qrContent.keywords.Any(keyword => keyword.Length > 128)) {
                        ModelState.AddModelError("KeywordTooLong",
                                                 "Keywords must be less than 128 characters.");
                        valid = false;
                    }

                    if (qrContent.referenceList.Any(refContent => String.IsNullOrEmpty(refContent.referenceString))) {
                        ModelState.AddModelError("IncompleteReference",
                                                 "References must be completed before marking request as complete.");
                        valid = false;
                    }
                }

                reqContent.timeClosed = DateTime.Now;
                reqContent.requestStatus = Constants.RequestStatus.Completed;
            }

            // Encode HTML in question responses
            // Replace null references with empty string
            foreach (
                QuestionResponseContent qrContent in
                    reqContent.questionResponseList) {
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

                foreach (
                    ReferenceContent refContent in
                        qrContent.referenceList) {

                    if (refContent.referenceString == null) {
                        refContent.referenceString = "";
                    } else {
                        refContent.referenceString = refContent.referenceString.Replace("\\", "\\\\");
                    }
                }
            }

            if (!valid) {
                var dc = new DropdownManagementController();

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

            almc.addEntry(reqContent.requestID, up.UserId,
                          Constants.AuditType.RequestModification);

            if (Request.Form["mark_as_complete"] != null) {
                almc.addEntry(reqContent.requestID, up.UserId,
                              Constants.AuditType.RequestCompletion,
                              (DateTime) reqContent.timeClosed);
            }

            if (Roles.IsUserInRole(Constants.Roles.VIEWER)) {
                return RedirectToAction("Details", "Request",
                                        new {id = reqContent.requestID});
            }

            return RedirectToAction("Index", "Home",
                                    new {
                                        status =
                                        Constants.URLStatus.SuccessfulEdit
                                    });
        }

        /// <summary>
        ///     Creates the partial for a new QuestionResponse
        /// </summary>
        /// <param name="json">The JSON Data for the QR</param>
        /// <returns>A partial for the QuestionResponse</returns>
        /// <request type="POST">/Request/NewQuestionResponse</request>
        [HttpPost]
        public ActionResult NewQuestionResponse(String json) {
            // Don't want to load a partial with a Not Authorized message
            if (!Roles.IsUserInRole(Constants.Roles.REQUEST_EDITOR)) {
                return RedirectToAction("Index", "Home", new {
                    status = Constants.URLStatus.NoRequestEditorRole
                });
            }

            var dc = new DropdownManagementController();
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

        /// <summary>
        ///     Creates the Partial for a Reference
        /// </summary>
        /// <param name="id">The ID for the QR</param>
        /// <param name="json">The JSON Data to use</param>
        /// <returns>The Reference Partial</returns>
        /// <request type="GET">/Request/NewReference</request>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.REQUEST_EDITOR)]
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

        /// <summary>
        ///     Get the matching keywords for a partial
        /// </summary>
        /// <param name="id">The partial to use</param>
        /// <returns>The List of matching keywords, in JSON.</returns>
        /// <request type="any">/Request/GetMatchingKeywords</request>
        [Authorize]
        public ActionResult GetMatchingKeywords(string id) {
            var dc = new DropdownManagementController();
            return Json(dc.getMatchingKeywords(id),
                        JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     View a request by ID
        /// </summary>
        /// <param name="id">The ID of the Request</param>
        /// <returns>The Request Details View</returns>
        /// <request type="GET">/Request/Details</request>
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Details(long id) {
            var rmc = new RequestManagementController();
            var rlc = new RequestLockManagementController();
            var upc = new UserManagementController();
            var db = new CAIRSDataContext();
            int timeSpent = 0;

            // Set up the Request Object
            RequestContent request = rmc.getRequestDetails(id);
            if (request == null) {
                ViewBag.Title = Constants.UIString.TitleText.VIEW_REQUEST
                                + " - "
                                + Constants.UIString.TitleText.ERROR;
                ViewBag.Error =
                    "The Request ID provided does not exist in the database.";

                return View((object) null);
            }

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
                ViewBag.Error =
                    "You do not have the necessary permissions to view this request.";

                return View((object) null);
            }

            // Show error if not administrator and request is invalid (deleted)
            if (!User.IsInRole(Constants.Roles.ADMINISTRATOR)
                && request.requestStatus == Constants.RequestStatus.Invalid) {
                ViewBag.Title = Constants.UIString.TitleText.VIEW_REQUEST
                                + " - "
                                + Constants.UIString.TitleText.ERROR;
                ViewBag.Error =
                    "You do not have the necessary permissions to view this request.";

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
                    ViewBag.Error =
                        "This request has been locked to another person and cannot be viewed until unlocked.";

                    return View((object) null);
                }
            }

            // Set up Time Spent (Question-Dependent)
            foreach (QuestionResponseContent qr in request.questionResponseList) {
                timeSpent += qr.timeSpent.GetValueOrDefault(0);
            }

            ViewBag.TimeSpent = timeSpent;
            ViewBag.DataContext = new CAIRSDataContext();

            // Created By
            AuditLog auditLog = (from al in db.AuditLogs
                                 where
                                     (int) al.AuditType ==
                                     (int) Constants.AuditType.RequestCreation &&
                                     al.RequestID == request.requestID
                                 select al).FirstOrDefault();
            if (auditLog != null && auditLog.UserProfile != null) {
                ViewBag.CreatedBy = auditLog.UserProfile.UserFullName;
            } else {
                ViewBag.CreatedBy = "";
            }

            // Closed By
            auditLog = (from al in db.AuditLogs
                        where
                            (int) al.AuditType ==
                            (int) Constants.AuditType.RequestCompletion &&
                            al.RequestID == request.requestID
                        select al).FirstOrDefault();
            if (auditLog != null && auditLog.UserProfile != null) {
                ViewBag.CompletedBy = auditLog.UserProfile.UserFullName;
            } else {
                ViewBag.CompletedBy = "";
            }

            // add AuditLog entry for viewing
            var almc = new AuditLogManagementController();
            almc.addEntry(id, upc.getUserProfile(User.Identity.Name).UserId,
                          Constants.AuditType.RequestView);

            ViewBag.IsLocked = rlc.isLocked(id);

            if (ViewBag.IsLocked) {
                ViewBag.IsLockedToMe = rlc.getRequestLock(id).UserID ==
                                       upc.getUserProfile(User.Identity.Name)
                                          .UserId;
            } else {
                ViewBag.IsLockedToMe = false;
            }

            return View(request);
        }

        /// <summary>
        ///     Ability to Unlock a locked Request
        /// </summary>
        /// <param name="id">The Request ID to Unlock</param>
        /// <returns>The Request Unlock View</returns>
        /// <request type="GET">/Request/Unlock</request>
        [Authorize]
        public ActionResult Unlock(long id) {
            var rlc = new RequestLockManagementController();
            // add AuditLog entry for lock remove
            var upc = new UserManagementController();
            var almc = new AuditLogManagementController();
            almc.addEntry(id, upc.getUserProfile(User.Identity.Name).UserId,
                          Constants.AuditType.RequestUnlock);

            rlc.removeLock(id);

            return RedirectToAction("Index", "Home", new {
                status = Constants.URLStatus.Unlocked
            });
        }

        /// <summary>
        ///     Ability to Delete a locked Request
        /// </summary>
        /// <param name="id">The Request ID to Delete</param>
        /// <returns>The Dashboard View</returns>
        /// <request type="GET">/Request/Delete</request>
        [Authorize(Roles = Constants.Roles.REQUEST_EDITOR)]
        public ActionResult Delete(long id) {
            var rmc = new RequestManagementController();

            rmc.invalidate(id);

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

        /// <summary>
        ///     Ability to Export a locked Request as a DOCX
        /// </summary>
        /// <param name="id">The Request ID to Export</param>
        /// <returns>A DOCX file.</returns>
        /// <request type="GET">/Request/Export</request>
        public ActionResult Export(long id) {
            var wec = new WordExportController();
            var db = new CAIRSDataContext();
            Request request = db.Requests.FirstOrDefault(r => r.RequestID == id);

            var markDate = new DateTime(2010, 01, 01, 00, 00, 00, 00);
            TimeSpan dateStamp = DateTime.Now.Subtract(markDate);
            string filePath =
                Server.MapPath(Constants.Export.REPORT_TEMP_PATH +
                               dateStamp.TotalSeconds + ".docx");
            string templatePath =
                Server.MapPath(Constants.Export.REPORT_TEMPLATE_PATH);

            IEnumerable<string> output = wec.requestToStrings(request);
            wec.generateDocument(output, templatePath, filePath, id);

            // add AuditLog entry for exporting
            var upc = new UserManagementController();
            var almc = new AuditLogManagementController();
            almc.addEntry(id, upc.getUserProfile(User.Identity.Name).UserId,
                          Constants.AuditType.RequestExport);

            return View("Details", new RequestContent(request));
        }
    }
}