using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers {
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller {
        private CAIRSDataContext _db = new CAIRSDataContext();

        //
        // GEET: /Admin/Index

        public ActionResult Index() {
            return RedirectToAction("Users");
        }

        //
        // GET: /Admin/Users

        public ActionResult Users() {
            return View(_db.UserProfiles.Select(up => up));
        }

        //
        // GET: /Admin/UserDetails
        [HttpGet]
        public ActionResult UserDetails(int id, bool success = false) {
            UserProfile userProfile =
                _db.UserProfiles.FirstOrDefault(up => up.UserId == id);
            var dc = ServiceSystem.DropdownController
                              .instance;

            ViewBag.Groups =
                dc.getActiveEntries(
                    Constants.DropdownTable.UserGroup);
            ViewBag.Roles = Roles.GetAllRoles();

            if (userProfile != null) {
                ViewBag.UserGroups = userProfile.UserGroups.AsQueryable();
                ViewBag.UserRoles =
                    Roles.GetRolesForUser(userProfile.UserName).ToList();
            }

            ViewBag.SuccessMessage = success ? "Your changes have been saved." : null;

            return View(userProfile);
        }

        //
        // POST: /Admin/UserDetails
        [HttpPost]
        public ActionResult UserDetails(string userName,
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

            return RedirectToAction("UserDetails", new {success = true});
        }
    }
}