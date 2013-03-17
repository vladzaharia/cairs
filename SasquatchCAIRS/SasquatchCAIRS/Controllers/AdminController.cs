using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers {
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller {
        private CAIRSDataContext _db = new CAIRSDataContext();
        private DropdownController _dc = new DropdownController();

        //
        // GET: /Admin/Index

        public ActionResult Index() {
            return RedirectToAction("Users");
        }

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
                            ug => ug.GroupID == Convert.ToByte(groupId)
                                  && ug.UserID == up.UserId)) {
                        _db.UserGroups1.InsertOnSubmit(new UserGroups {
                            GroupID = Convert.ToByte(groupId),
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
                                         string active) {
            // Blank Value Sanity Checks
            if (table != Constants.DropdownTable.Keyword && code == "") {
                ModelState.AddModelError("value", "Value cannot be empty!");
            }
            if (value == "") {
                ModelState.AddModelError("code", "Value cannot be empty!");
            }
            if (active == "") {
                ModelState.AddModelError("active", "Status cannot be empty!");
            }

            // Real Sanity Checks
            if (active != "true" && active != "false") {
                ModelState.AddModelError("active",
                                         "Status must be true or false!");
            }
            if (_dc.getEntries(table).Any(dt => dt.code == code)) {
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
                View(new DropdownEntry(id, code, value,
                                       Convert.ToBoolean(active)));
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
                                           string active) {
            // Blank Value Sanity Checks
            if (table != Constants.DropdownTable.Keyword && code == "") {
                ModelState.AddModelError("value", "Value cannot be empty!");
            }
            if (value == "") {
                ModelState.AddModelError("code", "Value cannot be empty!");
            }
            if (active == "") {
                ModelState.AddModelError("active", "Status cannot be empty!");
            }

            // Real Sanity Checks
            if (active != "true" && active != "false") {
                ModelState.AddModelError("active",
                                         "Status must be true or false!");
            }
            if (_dc.getEntries(table).Any(dt => dt.code == code)) {
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

            ViewBag.SelectList = new SelectList(slis, "Value", "Text",
                                                Convert.ToBoolean(active));
            ViewBag.Table = table;
            return
                View(new DropdownEntry(0, code, value, Convert.ToBoolean(active)));
        }
    }
}