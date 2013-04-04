using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers {
    [Authorize(Roles = Constants.Roles.ADMINISTRATOR)]
    [ExcludeFromCodeCoverage]
    public class AdminController : Controller {
        private CAIRSDataContext _db = new CAIRSDataContext();
        private DropdownController _dc = new DropdownController();

        //
        // GET: /Admin/Index

        public ActionResult Index() {
            return RedirectToAction("Users");
        }

        #region Users

        //
        // GET: /Admin/User/List

        [HttpGet]
        public ActionResult Users(bool success = false) {
            ViewBag.SuccessMessage = success
                                         ? "Your changes have been saved."
                                         : null;
            return View(_db.UserProfiles.Select(up => up));
        }

        //
        // GET: /Admin/UserEdit

        [HttpGet]
        public ActionResult UserEdit(int id) {
            UserProfile userProfile =
                _db.UserProfiles.FirstOrDefault(up => up.UserId == id);
            var dc = new DropdownController();

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

        

        //
        // POST: /Admin/User/Edit

        [HttpPost]
        public ActionResult UserEdit(string userName,
                                     List<string> userGroup,
                                     List<string> userRole) {
            var uc = new UserController();
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
                    if (!userGroup.Contains(((int) ug.GroupID) + String.Empty)) {
                        _db.UserGroups1.DeleteOnSubmit(ug);
                        _db.SubmitChanges();
                    }
                }
            } else {
                // No boxes checked, clear all groups.
                foreach (
                    UserGroups ug in
                        _db.UserGroups1.Where(newUg => newUg.UserID == up.UserId)
                    ) {
                    _db.UserGroups1.DeleteOnSubmit(ug);
                    _db.SubmitChanges();
                }
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
                // No boxes checked, clear all roles.
                Roles.RemoveUserFromRoles(userName,
                                          Roles.GetRolesForUser(userName));
            }

            return RedirectToAction("Users", new {success = true});
        }

#endregion

        #region Audit

        //
        // GET: /Admin/Audit

        public ActionResult Audit() {
            return View();
        }

        //
        // POST: /Admin/Audit

        [HttpPost]
        public ActionResult Audit(AuditCriteria model, FormCollection form) {

            // Blank Value Sanity Checks
            if (String.IsNullOrEmpty(model.criteriaType)) {
                ModelState.AddModelError("criteriaType",
                                         "Please select a search criteria!");
            }

            if (String.Equals(model.criteriaType, "uCriteria") && String.IsNullOrEmpty(model.userName)) {
                ModelState.AddModelError("userName",
                                         "Please enter username to audit!");
            }

            if (String.Equals(model.criteriaType, "uCriteria") && model.startDate.Equals(null)) {
                ModelState.AddModelError("startDate",
                                         "Please select a start date for the audit!");
            }

            if (String.Equals(model.criteriaType, "uCriteria") && model.endDate.Equals(null)) {
                ModelState.AddModelError("endDate",
                                         "Please select an end date for the audit!");
            }

            if (String.Equals(model.criteriaType, "uCriteria") && model.startDate > model.endDate) {
                ModelState.AddModelError("startDate",
                                         "Start date must be prior to end date.");
            }


            if (String.Equals(model.criteriaType, "rCriteria") && String.IsNullOrEmpty(model.requestID)){
                    ModelState.AddModelError("requestID",
                                             "Please enter request ID(s) to audit!");
                }

            if (!ModelState.IsValid) {
                return View(model);
            }

            // If it passes this point, everything is valid.
            // So clearly, the server fails now.

            // check if user selected search by User ID
            if (model.criteriaType != null &&
                model.criteriaType.Equals("uCriteria")) {
                
                // format ID entered
                string auditUser =
                    model.userName.Trim().ToLower();

                // check if user is in the database
                bool idFound = (from u in _db.UserProfiles
                                 where auditUser == u.UserName.ToLower()
                                 select u.UserName.ToLower()).ToList().Contains(auditUser);

                if (!idFound)
                {
                    ModelState.AddModelError("userName",
                                         "User does not exist.");

                    return View(model);
                }

                List<long> auditIDs = (from u in _db.UserProfiles
                                       where auditUser == u.UserName.ToLower()
                                       select Convert.ToInt64(u.UserId)).ToList();

                // call createReportForUser for all users
                DateTime start = Convert.ToDateTime(model.startDate);
                DateTime end = Convert.ToDateTime(model.endDate);

                AuditLogManagementController almController =
                    new AuditLogManagementController();
                
                // true if report has data, false if not                
                if (almController.createReportForUser(auditIDs,
                                                  start,
                                                  end) == false){
                    ViewBag.FailMessage = ("There is no data in this audit report!");

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
                                      .ToArray();


                // create list of long request IDs that will be audited (deal with ranges)
                List<long> requestsForAudit = new List<long>();

                foreach (string rID in auditRequestsString) {
                    if (rID.Contains("-")) {
                        long rangeStart = Convert.ToInt64(rID.Split('-')
                                            .Select(sValue => sValue.Trim()).First());
                        long rangeEnd = Convert.ToInt64(rID.Split('-')
                                            .Select(sValue => sValue.Trim()).Last());
                        if (!(rangeStart < rangeEnd))
                        {
                            ModelState.AddModelError("requestID",
                                        "An invalid request ID range was entered.");

                            return View(model);
                        }

                        while (rangeStart <= rangeEnd) {
                            List<long> list = (_db.Requests.Where(req => req.RequestID == rangeStart).Select(req => req.RequestID)).ToList();
                            if ((list).Any())
                            {
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
                                            where requestsForAudit.Contains(req.RequestID)
                                            select req.RequestID).ToList();

                // compare requestsForAudit with requestsFound
                List<string> invalidIDs = new List<string>();
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
                           errorMessage = errorMessage + invalidIDs.First() + ", ";
                           invalidIDs.RemoveAt(0);
                       }

                       errorMessage = errorMessage + "and " + invalidIDs.First();

                      ModelState.AddModelError("requestID",
                                           "Requests " + errorMessage + " do not exist.");

                       return View(model);
                   }
                }
                
                // grab Requests for all request IDs
                List<Request> auditRequests = (from req in _db.Requests
                                    where requestsForAudit.Contains(req.RequestID)
                                    select req).ToList();
                                          

                // call createReportForRequest for all requests
                AuditLogManagementController almController =
                    new AuditLogManagementController();

                // true if report has data, false if not                
                if (almController.createReportForRequest(auditRequests) == false) {
                    ViewBag.FailMessage = ("There is no data in this audit report!");

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

        //
        // GET: /Admin/Dropdown/List

        public ActionResult Dropdowns(bool success = false) {
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

        //
        // GET: /Admin/Dropdown/Edit

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

        //
        // POST: /Admin/Dropdown/Edit

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

            // Real Sanity Checks
            if (table != Constants.DropdownTable.Keyword &&
                _dc.getEntries(table).Any(dt => dt.code == code && 
                    dt.id != id)) {
                ModelState.AddModelError("code", "That code is already in use!");
            }
            if (_dc.getEntries(table).Any(dt => dt.value == value && dt.id != id)) {
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
                _dc.editEntry(table, id, code, value, active);
                return RedirectToAction("Dropdowns", new {
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

        //
        // GET: /Admin/Dropdown/Create

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

        //
        // POST: /Admin/Dropdown/Create

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

                return RedirectToAction("Dropdowns", new {
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

        #endregion
    }
}