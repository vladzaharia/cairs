using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Security;
using SasquatchCAIRS.Models.Service;

namespace SasquatchCAIRS.Controllers.ViewControllers {
    /// <summary>
    ///     Provides all Admin-Related Functionality
    /// </summary>
    [Authorize(Roles = Constants.Roles.ADMINISTRATOR)]
    [ExcludeFromCodeCoverage]
    public class AdminController : Controller {
        /// <summary>Data Context for this Controller.</summary>
        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>Dropdown Management Controller for this Controller.</summary>
        private DropdownManagementController _dc =
            new DropdownManagementController();

        /// <summary>
        ///     The default Index page when visiting /Admin
        /// </summary>
        /// <returns>The User List View</returns>
        /// <request type="GET">/Admin/Index</request>
        public ActionResult Index() {
            return RedirectToAction("UserList");
        }

        #region Users

        /// <summary>
        ///     List all users in the system
        /// </summary>
        /// <returns>The User List View</returns>
        /// <request type="GET">/Admin/User/List</request>
        [HttpGet]
        public ActionResult UserList(bool success = false) {
            ViewBag.SuccessMessage = success
                                         ? "Your changes have been saved."
                                         : null;
            return
                View(_db.UserProfiles.Select(up => up)
                        .OrderBy(up => up.UserName));
        }

        /// <summary>
        ///     Edit a User and add/remove Roles and Groups
        /// </summary>
        /// <returns>The User Edit View</returns>
        /// <request type="GET">/Admin/User/Edit</request>
        [HttpGet]
        public ActionResult UserEdit(int id) {
            UserProfile userProfile =
                _db.UserProfiles.FirstOrDefault(up => up.UserId == id);
            ViewBag.Groups =
                _dc.getEntries(
                    Constants.DropdownTable.UserGroup);
            ViewBag.Roles = Roles.GetAllRoles();

            if (userProfile != null) {
                ViewBag.UserGroups = userProfile.UserGroups.AsQueryable();
                ViewBag.UserRoles =
                    Roles.GetRolesForUser(userProfile.UserName).ToList();
            }

            return View(userProfile);
        }

        /// <summary>
        ///     Process the editing of a User in the system
        /// </summary>
        /// <returns>The User Edit View if there's an error, or User List View otherwise</returns>
        /// <request type="POST">/Admin/User/Edit</request>
        [HttpPost]
        public ActionResult UserEdit(string userName,
                                     List<string> userGroup,
                                     List<string> userRole) {
            var uc = new UserManagementController();
            UserProfile up = uc.getUserProfile(userName);

            // Process Groups to Add
            if (userGroup != null) {
                foreach (string groupId in userGroup) {
                    if (
                        !_db.UserGroups1.Any(
                            ug => ug.GroupID == Convert.ToInt32(groupId)
                                  && ug.UserID == up.UserId)) {
                        _db.UserGroups1.InsertOnSubmit(new UserGroups {
                            GroupID = Convert.ToInt32(groupId),
                            UserID = up.UserId
                        });
                        _db.SubmitChanges();
                    }
                }

                // Process Groups to Remove
                foreach (
                    UserGroups ug in
                        _db.UserGroups1.Where(newUg => newUg.UserID == up.UserId)
                    ) {
                    if (!userGroup.Contains((ug.GroupID) + String.Empty)) {
                        _db.UserGroups1.DeleteOnSubmit(ug);
                        _db.SubmitChanges();
                    }
                }
            } else {
                ModelState.AddModelError("groups",
                                         "A user must have at least one group!");
            }

            if (userRole != null) {
                // Proccess Roles to Remove
                foreach (string role in Roles.GetRolesForUser(up.UserName)) {
                    if (!userRole.Contains(role)) {
                        Roles.RemoveUserFromRole(up.UserName, role);
                    }
                }

                // Process Roles to Add
                foreach (string role in userRole) {
                    if (!Roles.IsUserInRole(up.UserName, role)) {
                        Roles.AddUserToRole(up.UserName, role);
                    }
                }
            } else {
                ModelState.AddModelError("roles",
                                         "A user must have at least one role!");
            }

            if (ModelState.IsValid) {
                return RedirectToAction("UserList", new {
                    success = true
                });
            } else {
                ViewBag.Groups =
                    _dc.getEntries(
                        Constants.DropdownTable.UserGroup);
                ViewBag.Roles = Roles.GetAllRoles();

                if (up != null) {
                    ViewBag.UserGroups = up.UserGroups.AsQueryable();
                    ViewBag.UserRoles =
                        Roles.GetRolesForUser(up.UserName).ToList();
                }
                return View(up);
            }
        }

        #endregion

        #region Audit

        /// <summary>
        ///     Get the Main Auditing page
        /// </summary>
        /// <returns>The Audit View</returns>
        /// <request type="GET">/Admin/Audit</request>
        public ActionResult Audit() {
            return View();
        }

        /// <summary>
        ///     Generate the Audit Report
        /// </summary>
        /// <returns>A download of the Audit Report</returns>
        /// <request type="POST">/Admin/Audit</request>
        [HttpPost]
        public ActionResult Audit(AuditCriteria model, FormCollection form) {
            // Blank Value Sanity Checks
            if (String.IsNullOrEmpty(model.criteriaType)) {
                ModelState.AddModelError("criteriaType",
                                         "Please select a search criteria!");
            }

            if (String.Equals(model.criteriaType, "uCriteria") &&
                String.IsNullOrEmpty(model.userName)) {
                ModelState.AddModelError("userName",
                                         "Please enter username to audit!");
            }

            if (String.Equals(model.criteriaType, "uCriteria") &&
                model.startDate.Equals(null)) {
                ModelState.AddModelError("startDate",
                                         "Please select a start date for the audit!");
            }

            if (String.Equals(model.criteriaType, "uCriteria") &&
                model.endDate.Equals(null)) {
                ModelState.AddModelError("endDate",
                                         "Please select an end date for the audit!");
            }

            if (String.Equals(model.criteriaType, "uCriteria") &&
                model.startDate > model.endDate) {
                ModelState.AddModelError("startDate",
                                         "Start date must be prior to end date.");
            }

            if (String.Equals(model.criteriaType, "rCriteria") &&
                String.IsNullOrEmpty(model.requestID)) {
                ModelState.AddModelError("requestID",
                                         "Please enter request ID(s) to audit!");
            }

            if (!ModelState.IsValid) {
                return View(model);
            }

            // check if user selected search by User ID
            if (model.criteriaType != null &&
                model.criteriaType.Equals("uCriteria")) {
                // format ID entered
                string auditUser =
                    model.userName.Trim().ToLower();

                // check if user is in the database
                bool idFound = (from u in _db.UserProfiles
                                where auditUser == u.UserName.ToLower()
                                select u.UserName.ToLower()).ToList()
                                                            .Contains(auditUser);

                if (!idFound) {
                    ModelState.AddModelError("userName",
                                             "User does not exist.");

                    return View(model);
                }

                int auditID = (from u in _db.UserProfiles
                               where auditUser == u.UserName.ToLower()
                               select Convert.ToInt32(u.UserId)).FirstOrDefault();

                // call createReportForUser for all users
                DateTime start = Convert.ToDateTime(model.startDate);
                DateTime end = Convert.ToDateTime(model.endDate);

                var almController =
                    new AuditLogManagementController();

                // true if report has data, false if not                
                if (almController.createReportForUser(auditID,
                                                      start,
                                                      end) == false) {
                    ViewBag.FailMessage =
                        ("There is no data in this audit report!");

                    return View(model);
                }

                // now that report has been generated, clear page

                ViewBag.SuccessMessage = ("Audit report successfully created!");

                return View(model);
            }

            // check if user selected search by Request ID
            if (model.criteriaType != null &&
                model.criteriaType.Equals("rCriteria")) {

                // parse comma delimited IDs entered
                string[] auditRequestsString =
                    model.requestID.Split(',')
                         .Select(sValue => sValue.Trim())
                         .Select(sValue => sValue.Replace(" ", string.Empty))
                         .ToArray();

                // create list of long request IDs that will be audited (deal with ranges)
                var requestsForAudit = new List<long>();

                foreach (string rID in auditRequestsString) {
                    long i = 0;

                    if (!(long.TryParse(rID, out i)) && !(long.TryParse(rID.Replace('-', '2'), out i))) {
                        ModelState.AddModelError("requestID",
                                                 "A non-numeric request ID was entered.");

                        return View(model);
                    }                    
                    if (rID.Contains("-")) {
                        long rangeStart = Convert.ToInt64(rID.Split('-')
                                                             .Select(
                                                                 sValue =>
                                                                 sValue.Trim())
                                                             .First());
                        long rangeEnd = Convert.ToInt64(rID.Split('-')
                                                           .Select(
                                                               sValue =>
                                                               sValue.Trim())
                                                           .Last());
                        if (!(rangeStart < rangeEnd)) {
                            ModelState.AddModelError("requestID",
                                                     "An invalid request ID range was entered.");

                            return View(model);
                        }

                        while (rangeStart <= rangeEnd) {
                            List<long> list =
                                (_db.Requests.Where(
                                    req => req.RequestID == rangeStart)
                                    .Select(req => req.RequestID)).ToList();
                            if ((list).Any()) {
                                requestsForAudit.Add(rangeStart);
                            }
                            rangeStart++;
                        }
                    } else {
                        requestsForAudit.Add(Convert.ToInt64(rID));
                    }
                }

                // find valid requests in database
                IEnumerable<long> requestsFound = (from req in _db.Requests
                                                   where
                                                       requestsForAudit.Contains
                                                       (req.RequestID)
                                                   select req.RequestID).ToList();

                // compare requestsForAudit with requestsFound
                var invalidIDs = new List<string>();
                bool idMissing = false;

                foreach (long ID in requestsForAudit) {
                    if (!requestsFound.Contains(ID)) {
                        invalidIDs.Add(ID.ToString());
                        idMissing = true;
                    }
                }

                // if not all requests existed, return error
                if (idMissing) {
                    if (invalidIDs.Count() == 1) {
                        ModelState.AddModelError("requestID",
                                                 "Request " + invalidIDs.First() +
                                                 " does not exist.");

                        return View(model);
                    } else {
                        string errorMessage = null;

                        while (invalidIDs.Count() > 1) {
                            errorMessage = errorMessage + invalidIDs.First() +
                                           ", ";
                            invalidIDs.RemoveAt(0);
                        }

                        errorMessage = errorMessage + "and " +
                                       invalidIDs.First();

                        ModelState.AddModelError("requestID",
                                                 "Requests " + errorMessage +
                                                 " do not exist.");

                        return View(model);
                    }
                }

                // grab Requests for all request IDs
                List<Request> auditRequests = (from req in _db.Requests
                                               where
                                                   requestsForAudit.Contains(
                                                       req.RequestID)
                                               select req).ToList();

                // call createReportForRequest for all requests
                var almController =
                    new AuditLogManagementController();

                // true if report has data, false if not                
                if (almController.createReportForRequest(auditRequests) == false) {
                    ViewBag.FailMessage =
                        ("There is no data in this audit report!");

                    return View(model);
                }

                // now that report has been generated, clear page

                ViewBag.SuccessMessage = ("Audit report successfully created!");

                return View(model);
            }
            return View(model);
        }

        #endregion

        #region Dropdowns

        /// <summary>
        ///     Get a List of all the Dropdowns in the System
        /// </summary>
        /// <returns>The Dropdown List View</returns>
        /// <request type="GET">/Admin/Dropdown/List</request>
        public ActionResult DropdownList(bool success = false) {
            Dictionary<Constants.DropdownTable, List<DropdownEntry>> model =
                Constants.DROPDOWN_TABLES.ToDictionary(dropdown => dropdown,
                                                       dropdown =>
                                                       _dc.getEntries(dropdown,
                                                                      false));
            ViewBag.SuccessMessage = success
                                         ? "Your changes have been saved."
                                         : null;

            return View(model);
        }

        /// <summary>
        ///     Edit a Dropdown's Code, Value, and Status
        /// </summary>
        /// <returns>The Dropdown Edit View</returns>
        /// <request type="GET">/Admin/Dropdown/Edit</request>
        [HttpGet]
        public ActionResult DropdownEdit(Constants.DropdownTable table, int id) {
            DropdownEntry model =
                _dc.getEntries(table, false).FirstOrDefault(dd => dd.id == id);

            var slis = new List<SelectListItem> {
                new SelectListItem {
                    Text = Constants.UIString.GeneralText.ACTIVE,
                    Value = "true",
                    Selected = true
                },
                new SelectListItem {
                    Text = Constants.UIString.GeneralText.DISABLED,
                    Value = "false",
                    Selected = false
                }
            };

            ViewBag.SelectList = new SelectList(slis, "Value", "Text",
                                                model != null && model.active);
            ViewBag.Table = table;

            return View(model);
        }

        /// <summary>
        ///     Proccess the Dorpdown Editing
        /// </summary>
        /// <returns>The Dropdown Edit View if an error occured, or Dropdown List otherwise</returns>
        /// <request type="POST">/Admin/Dropdown/Edit</request>
        [HttpPost]
        public ActionResult DropdownEdit(Constants.DropdownTable table, int id,
                                         string code, string value,
                                         bool active) {
            // Blank Value Sanity Checks
            if (table != Constants.DropdownTable.Keyword && code == "") {
                ModelState.AddModelError("code", "Code cannot be empty!");
            }
            if (value == "") {
                ModelState.AddModelError("value", "Value cannot be empty!");
            }

            // Length Sanity Checks
            if (table != Constants.DropdownTable.Keyword && code.Length > 10) {
                ModelState.AddModelError("code", "Code cannot be over 10 characters!");
            }

            if (value.Length > 64) {
                ModelState.AddModelError("value", "Value cannot be over 64 characters!");
            }

            // Real Sanity Checks
            if (table != Constants.DropdownTable.Keyword &&
                _dc.getEntries(table).Any(dt => dt.code == code &&
                                                dt.id != id)) {
                ModelState.AddModelError("code", "That code is already in use!");
            }
            if (_dc.getEntries(table)
                   .Any(dt => dt.value == value && dt.id != id)) {
                ModelState.AddModelError("value",
                                         "That " + (table ==
                                                    Constants.DropdownTable
                                                             .Keyword
                                                        ? "keyword"
                                                        : "value") +
                                         " is already in use!");
            }

            // Check if last one in respective table
            if (_dc.getEntries(table).Count == 1) {
                ModelState.AddModelError("active",
                                         "Cannot disable last active dropdown in list!");
            }

            // Route to List page if valid, or back to create with errors
            if (ModelState.IsValid) {
                _dc.editEntry(table, id, code, value, active);
                return RedirectToAction("DropdownList", new {
                    success = true
                });
            }

            var slis = new List<SelectListItem> {
                new SelectListItem {
                    Text = Constants.UIString.GeneralText.ACTIVE,
                    Value = "true",
                    Selected = true
                },
                new SelectListItem {
                    Text = Constants.UIString.GeneralText.DISABLED,
                    Value = "false",
                    Selected = false
                }
            };

            ViewBag.SelectList = new SelectList(slis, "Value", "Text",
                                                Convert.ToBoolean(active));
            ViewBag.Table = table;
            return
                View(new DropdownEntry(id, code, value, active));
        }

        /// <summary>
        ///     Create a new Dropdown Item
        /// </summary>
        /// <returns>The Dropdown Create View</returns>
        /// <request type="GET">/Admin/Dropdown/Create</request>
        [HttpGet]
        public ActionResult DropdownCreate(Constants.DropdownTable? table) {
            var slis = new List<SelectListItem> {
                new SelectListItem {
                    Text = Constants.UIString.GeneralText.ACTIVE,
                    Value = "true",
                    Selected = true
                },
                new SelectListItem {
                    Text = Constants.UIString.GeneralText.DISABLED,
                    Value = "false",
                    Selected = false
                }
            };

            ViewBag.SelectList = new SelectList(slis, "Value", "Text", true);
            ViewBag.Table = table;

            return View();
        }

        /// <summary>
        ///     Proccess the Dropdown Creation request
        /// </summary>
        /// <returns>The Dropdown Edit View if an error occured, the Dropdown List view Otherwise</returns>
        /// <request type="POST">/Admin/Dropdown/Create</request>
        [HttpPost]
        public ActionResult DropdownCreate(Constants.DropdownTable table,
                                           string code, string value,
                                           bool active) {
            // Blank Value Sanity Checks
            if (table != Constants.DropdownTable.Keyword && code == "") {
                ModelState.AddModelError("code", "Code cannot be empty!");
            }
            if (value == "") {
                ModelState.AddModelError("value", "Value cannot be empty!");
            }

            // Length Sanity Checks
            if (table != Constants.DropdownTable.Keyword && code.Length > 10) {
                ModelState.AddModelError("code", "Code cannot be over 10 characters!");
            }

            if (value.Length > 64) {
                ModelState.AddModelError("value", "Value cannot be over 64 characters!");
            }

            // Real Sanity Checks
            if (table != Constants.DropdownTable.Keyword &&
                _dc.getEntries(table).Any(dt => dt.code == code)) {
                ModelState.AddModelError("code", "That code is already in use!");
            }
            if (_dc.getEntries(table).Any(dt => dt.value == value)) {
                ModelState.AddModelError("value",
                                         "That " + (table ==
                                                    Constants.DropdownTable
                                                             .Keyword
                                                        ? "keyword"
                                                        : "value") +
                                         " is already in use!");
            }

            // Route to List page if valid, or back to create with errors
            if (ModelState.IsValid) {
                _dc.createEntry(table, code, value, active);

                return RedirectToAction("DropdownList", new {
                    success = true
                });
            }

            var slis = new List<SelectListItem> {
                new SelectListItem {
                    Text = Constants.UIString.GeneralText.ACTIVE,
                    Value = "true",
                    Selected = true
                },
                new SelectListItem {
                    Text = Constants.UIString.GeneralText.DISABLED,
                    Value = "false",
                    Selected = false
                }
            };

            ViewBag.SelectList = new SelectList(slis, "Value", "Text", active);
            ViewBag.Table = table;
            return
                View(new DropdownEntry(0, code, value, active));
        }

        /// <summary>
        ///     Import keywords from a given CSV file.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ImportKeywords() {
            return View();
        }

        [HttpPost]
        public ActionResult ImportKeywords(int id = 0) {
            var file = Request.Files["keywords-file"];

            // Check file was submitted
            if (file == null || file.ContentLength == 0 ||
                !file.ContentType.Equals("application/vnd.ms-excel") ||
                !file.FileName.EndsWith("csv")) {
                ModelState.AddModelError("InvalidFile",
                                         "Keyword import only accepts CSV files.");
                return View();
            }

            var dmc = new DropdownManagementController();

            var reader = new StreamReader(file.InputStream);
            while (!reader.EndOfStream) {
                var str = reader.ReadLine();
                if (String.IsNullOrEmpty(str)) {
                    continue;
                }

                var keyword = str.Split(',')[0];
                Keyword kw = (from kws in _db.Keywords
                              where kws.KeywordValue == keyword
                              select kws)
                    .FirstOrDefault();

                if (kw == null) {
                    dmc.addEntry(Constants.DropdownTable.Keyword,
                                 new DropdownEntry(0, null, keyword, true));
                } else {
                    kw.Active = true;
                    _db.SubmitChanges();
                }
            }

            return RedirectToAction("DropdownList", new {
                success = true
            });
        } 

        #endregion
    }
}