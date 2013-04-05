using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using NUnit.Framework;
using Rhino.Mocks;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.Security;

namespace CAIRSTestProject.Unit {
    [TestFixture]
    public class TestUserController {
        [SetUp]
        public void SetUp() {
            _up = new UserProfile {
                UserName = "TUC-" + _random.Next(10000000)
                                           .ToString(
                                               CultureInfo.InvariantCulture),
                UserFullName = "Test User",
                UserEmail = "test@example.com"
            };

            // Create a Test User in the System
            _db.UserProfiles.InsertOnSubmit(_up);
            _db.SubmitChanges();
        }

        [TearDown]
        public void TearDown() {
            _db.UserProfiles.DeleteOnSubmit(_up);
            _db.SubmitChanges();

            // Clear out the Variable just in case
            _up = null;
        }

        private CAIRSDataContext _db = new CAIRSDataContext();
        private DirectorySearcher _searcher;
        private UserManagementController _uc;
        private UserProfile _up;
        private Random _random = new Random();

        [TestFixtureSetUp]
        public void SetUpTF() {
            _searcher = MockRepository.GenerateStub<DirectorySearcher>();
            _uc = new UserManagementController();
        }

        /// <summary>
        ///     Tests the getUserGroups method
        /// </summary>
        [Test]
        public void TestGetUserGroups() {
            // Precheck that the UP is set
            if (_up == null) {
                Assert.Fail("SetUp has not run yet!");
            }

            // Create a user group and attach it to the UserProfile
            var ug = new UserGroup {
                Code =
                    _random.Next(10000000)
                           .ToString(CultureInfo.InvariantCulture),
                Value =
                    "TUC-" +
                    _random.Next(1000000000)
                           .ToString(CultureInfo.InvariantCulture),
                Active = true
            };
            _db.UserGroups.InsertOnSubmit(ug);
            _db.SubmitChanges();
            var ugs = new UserGroups {
                GroupID = ug.GroupID,
                UserID = _up.UserId
            };
            _db.UserGroups1.InsertOnSubmit(ugs);
            _db.SubmitChanges();

            // Check that there's only one group.
            List<UserGroup> groups = _uc.getUserGroups(_up.UserName);
            Assert.AreEqual(1, groups.Count);

            // Grab the first item from the groups
            UserGroup group = groups[0];

            // Check that the values match the UG above
            Assert.AreEqual(ug.GroupID, group.GroupID);
            StringAssert.Contains(ug.Code, group.Code);
            StringAssert.Contains(ug.Value, group.Value);

            // Clean up the Group
            _db.UserGroups1.DeleteOnSubmit(ugs);
            _db.UserGroups.DeleteOnSubmit(ug);
            _db.SubmitChanges();
        }

        /// <summary>
        ///     Test Get User Groups with Blank Username
        /// </summary>
        [Test]
        public void TestGetUserGroupsBlank() {
            List<UserGroup> ugs = _uc.getUserGroups("");
            Assert.AreEqual(0, ugs.Count); // Should be Empty
        }

        /// <summary>
        ///     Test Get User Groups with Null Username
        /// </summary>
        [Test]
        public void TestGetUserGroupsNull() {
            List<UserGroup> ugs = _uc.getUserGroups(null);
            Assert.AreEqual(0, ugs.Count); // Should be Empty
        }

        /// <summary>
        ///     Tests the getUserProfile method
        /// </summary>
        [Test]
        public void TestGetUserProfile() {
            // Precheck that the UP is set
            if (_up == null) {
                Assert.Fail("SetUp has not run yet!");
            }

            // Grab the user through UserController
            UserProfile generatedUser = _uc.getUserProfile(_up.UserName);

            if (generatedUser == null) {
                Assert.Fail("Cannot find user!");
            }

            // Check that the fields match
            StringAssert.AreEqualIgnoringCase(_up.UserName,
                                              generatedUser.UserName);
            StringAssert.AreEqualIgnoringCase(_up.UserFullName,
                                              generatedUser.UserFullName);
            StringAssert.AreEqualIgnoringCase(_up.UserEmail,
                                              generatedUser.UserEmail);
            Assert.AreEqual(_up.UserId, generatedUser.UserId);
        }
    }
}