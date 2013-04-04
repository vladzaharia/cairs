using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Controllers;
using SasquatchCAIRS.Models.ServiceSystem;
using SasquatchCAIRS.Models;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CAIRSTestProject.Unit {
    [TestFixture]
    public class TestAuditLogManagementController {

        private CAIRSDataContext _dc = new CAIRSDataContext();
        private Request rq;
        private UserProfile up;
        private Request rq2;
        Random _random = new Random();

        [TestFixtureSetUp]
        public void SetUp() {
            long randomRequestInt = _random.Next(1, 100000000);
            long randomUserInt = _random.Next(1, 100000000);
            RequestManagementController rmc = new RequestManagementController();

            // Create new Request Directly in DB
            rmc.create(new RequestContent {
                patientLName =
                    "TALM" + randomRequestInt.ToString(CultureInfo.InvariantCulture)
            });

            // Create new UserProfile Directly in DB
            _dc.UserProfiles.InsertOnSubmit(new UserProfile {
                UserName = "TALM" + randomUserInt.ToString(CultureInfo.InvariantCulture)
            });

            _dc.SubmitChanges();

            rq =
                _dc.Requests.FirstOrDefault(
                    request => 
                    request.PatientLName == 
                    ("TALM" + randomRequestInt.ToString(CultureInfo.InvariantCulture)));
            up =
                _dc.UserProfiles.FirstOrDefault(
                    userProfile =>
                    userProfile.UserName ==
                    ("TALM" + randomUserInt.ToString(CultureInfo.InvariantCulture)));
        }

        [TestFixtureTearDown]
        public void TearDown() {
            _dc.AuditLogs.DeleteAllOnSubmit(_dc.AuditLogs.Where(au => au.RequestID == rq.RequestID));
            _dc.Requests.DeleteOnSubmit(rq);
            _dc.AuditLogs.DeleteAllOnSubmit(_dc.AuditLogs.Where(au => au.RequestID == rq2.RequestID));
            _dc.Requests.DeleteOnSubmit(rq2);
            _dc.UserProfiles.DeleteOnSubmit(up);
            _dc.SubmitChanges();
        }

        #region AddEntryCorrect

        [Test]
        public void Test_addEntryWithSpecifiedCompletion() {
            DateTime randomizedDateTime = DateTime.Now.AddMonths(_random.Next(0, 12));
            addEntryWithSpecifiedDateTimeHelper(Constants.AuditType.RequestCompletion, randomizedDateTime);    
        }

        [Test]
        public void Test_addEntryWithSpecifiedCreation() {
            DateTime randomizedDateTime = DateTime.Now.AddMonths(_random.Next(0, 12));
            addEntryWithSpecifiedDateTimeHelper(Constants.AuditType.RequestCreation, randomizedDateTime);
        }

        [Test]
        public void Test_addEntryWithSpecifiedDeletion() {
            DateTime randomizedDateTime = DateTime.Now.AddMonths(_random.Next(0, 12));
            addEntryWithSpecifiedDateTimeHelper(Constants.AuditType.RequestDeletion, randomizedDateTime);
        }

        [Test]
        public void Test_addEntryWithSpecifiedExport() {
            DateTime randomizedDateTime = DateTime.Now.AddMonths(_random.Next(0, 12));
            addEntryWithSpecifiedDateTimeHelper(Constants.AuditType.RequestExport, randomizedDateTime);
        }

        [Test]
        public void Test_addEntryWithSpecifiedModification() {
            DateTime randomizedDateTime = DateTime.Now.AddMonths(_random.Next(0, 12));
            addEntryWithSpecifiedDateTimeHelper(Constants.AuditType.RequestModification, randomizedDateTime);
        }

        [Test]
        public void Test_addEntryWithSpecifiedUnlock() {
            DateTime randomizedDateTime = DateTime.Now.AddMonths(_random.Next(0, 12));
            addEntryWithSpecifiedDateTimeHelper(Constants.AuditType.RequestUnlock, randomizedDateTime);
        }

        [Test]
        public void Test_addEntryWithSpecifiedView() {
            DateTime randomizedDateTime = DateTime.Now.AddMonths(_random.Next(0, 12));
            addEntryWithSpecifiedDateTimeHelper(Constants.AuditType.RequestView, randomizedDateTime);
        }

        [Test]
        public void Test_addEntryCompletion() {
            addEntryHelper(Constants.AuditType.RequestCompletion);
        }

        [Test]
        public void Test_addEntryCreation() {
            addEntryHelper(Constants.AuditType.RequestCreation);
        }

        [Test]
        public void Test_addEntryDeletion() {
            addEntryHelper(Constants.AuditType.RequestDeletion);
        }

        [Test]
        public void Test_addEntryExport() {
            addEntryHelper(Constants.AuditType.RequestExport);
        }

        [Test]
        public void Test_addEntryModification() {
            addEntryHelper(Constants.AuditType.RequestModification);
        }

        [Test]
        public void Test_addEntryUnlock() {
            addEntryHelper(Constants.AuditType.RequestUnlock);
        }

        [Test]
        public void Test_addEntryView() {
            addEntryHelper(Constants.AuditType.RequestView);
        }

        #endregion

        # region AddEntryIncorrect
        
        [Test]
        [NUnit.Framework.ExpectedException(typeof(UserDoesNotExistException))]
        public void Test_addEntryIncorrectUser() {
            AuditLogManagementController almc = new AuditLogManagementController();

            // run method
            almc.addEntry(rq.RequestID, 0, Constants.AuditType.RequestView);

            //// checks
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);

            Assert.IsNull(alCreated, "Audit log created despite invalid user ID.");
        }

        [Test]
        [NUnit.Framework.ExpectedException(typeof(RequestDoesNotExistException))]
        public void Test_addEntryIncorrectRequest() {
            AuditLogManagementController almc = new AuditLogManagementController();

            // run method
            almc.addEntry(0, up.UserId, Constants.AuditType.RequestView);

            //// checks
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);

            Assert.IsNull(alCreated, "Audit log created despite invalid request ID.");
        }

        [Test]
        [NUnit.Framework.ExpectedException(typeof(UserDoesNotExistException))]
        public void Test_addEntryWithSpecifiedIncorrectUser() {
            AuditLogManagementController almc = new AuditLogManagementController();

            // run method
            almc.addEntry(rq.RequestID, 0, Constants.AuditType.RequestView, DateTime.Now);

            //// checks
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);

            Assert.IsNull(alCreated, "Audit log created despite invalid user ID.");
        }

        [Test]
        [NUnit.Framework.ExpectedException(typeof(RequestDoesNotExistException))]
        public void Test_addEntryWithSpecifiedIncorrectRequest() {
            AuditLogManagementController almc = new AuditLogManagementController();

            // run method
            almc.addEntry(0, up.UserId, Constants.AuditType.RequestView, DateTime.Now);

            //// checks
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);

            Assert.IsNull(alCreated, "Audit log created despite invalid request ID.");
        }

        [Test]
        [NUnit.Framework.ExpectedException(typeof(SqlTypeException))]
        public void Test_addEntryWithSpecifiedIncorrectDateTime() {
            AuditLogManagementController almc = new AuditLogManagementController();

            // run method
            almc.addEntry(rq.RequestID, up.UserId, Constants.AuditType.RequestView, new DateTime (1600, 11, 1));

            //// checks
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);

            Assert.IsNull(alCreated, "Audit log created despite invalid datetime.");
        }


        #endregion

        #region createReportForRequestCorrect

        // Pass createReportForRequest null
        [Test]
        [NUnit.Framework.ExpectedException(typeof(AuditRequestsDoNotExistException))]
        public void Test_createReportForRequestNull() {
            AuditLogManagementController almc = new AuditLogManagementController();
            bool returnValue = almc.createReportForRequest(null);
        }

        // Pass createReportForRequest an empty list
        [Test]
        public void Test_createReportForRequestEmpty() {
            AuditLogManagementController almc = new AuditLogManagementController();
            System.Collections.Generic.List<Request> auditRequests = new List<Request>();
            bool returnValue = almc.createReportForRequest(auditRequests);

            Assert.IsFalse(returnValue, "Data in audit report for no requests.");
        }

        // Pass createReportForRequest an list with no audit logs
        [Test]
        public void Test_createReportForRequestNoData() {
            AuditLogManagementController almc = new AuditLogManagementController();
            System.Collections.Generic.List<Request> auditRequests = new List<Request> {
                rq
            };

            bool returnValue = almc.createReportForRequest(auditRequests);

            Assert.IsFalse(returnValue, "Data in audit report for requests with no AuditLogs.");
        }

        // Pass createReportForRequest a list with one request with audit data
        [Test]
        [NUnit.Framework.ExpectedException(typeof(System.ArgumentNullException), ExpectedMessage = "path", MatchType = MessageMatch.Contains)]
        public void Test_createReportForRequestWithSingleRequest() {
            
            AuditLogManagementController almc = new AuditLogManagementController();

            // create AuditLogs for rq
            _dc.AuditLogs.InsertOnSubmit(new AuditLog {
                RequestID = rq.RequestID,
                UserID = up.UserId,
                AuditType = (byte) Constants.AuditType.RequestView,
                AuditDate = DateTime.Now
            });
            _dc.SubmitChanges();

            System.Collections.Generic.List<Request> auditRequests = new List<Request> {
                rq
            };

            bool returnValue = almc.createReportForRequest(auditRequests);

            // delete AuditLog created
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);
            _dc.AuditLogs.DeleteOnSubmit(alCreated);
            _dc.SubmitChanges();
        }


        // Pass createReportForRequest a list with more than one request with audit data
        [Test]
        [NUnit.Framework.ExpectedException(typeof(System.ArgumentNullException), ExpectedMessage = "path", MatchType = MessageMatch.Contains)]
        public void Test_createReportForRequestWithMultipleRequests() {
            
            // Create an extra test request to test list with multiple
            long randomRequestInt = _random.Next(1, 100000000);
            RequestManagementController rmc = new RequestManagementController();

            // Create new Request Directly in DB
            rmc.create(new RequestContent {
                patientLName =
                    "TALM" + randomRequestInt.ToString(CultureInfo.InvariantCulture)
            });

            _dc.SubmitChanges();

            rq2 = _dc.Requests.FirstOrDefault(
                request =>
                request.PatientLName ==
                ("TALM" + randomRequestInt.ToString(CultureInfo.InvariantCulture)));

            AuditLogManagementController almc = new AuditLogManagementController();

            // create AuditLogs for rq
            _dc.AuditLogs.InsertOnSubmit(new AuditLog {
                RequestID = rq.RequestID,
                UserID = up.UserId,
                AuditType = (byte) Constants.AuditType.RequestView,
                AuditDate = DateTime.Now
            });
            _dc.SubmitChanges();

            // create AuditLogs for rq2
            _dc.AuditLogs.InsertOnSubmit(new AuditLog {
                RequestID = rq2.RequestID,
                UserID = up.UserId,
                AuditType = (byte) Constants.AuditType.RequestView,
                AuditDate = DateTime.Now
            });
            _dc.SubmitChanges();

            System.Collections.Generic.List<Request> auditRequests = new List<Request> {
                rq, rq2
            };

            bool returnValue = almc.createReportForRequest(auditRequests);

            // delete both AuditLogs created
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);
            _dc.AuditLogs.DeleteOnSubmit(alCreated);
            AuditLog alCreated2 = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq2.RequestID);
            _dc.AuditLogs.DeleteOnSubmit(alCreated2);
            _dc.SubmitChanges();
        }


        #endregion

        #region createReportForUser

        // Pass createReportForUser an invalid userID
        [Test]
        [NUnit.Framework.ExpectedException(typeof(UserDoesNotExistException))]
        public void Test_createReportForUserInvalid() {
            AuditLogManagementController almc = new AuditLogManagementController();
            bool returnValue = almc.createReportForUser(0, DateTime.MinValue, DateTime.MaxValue);

        }

        // Pass createReportForUser a user with no audit logs
        [Test]
        public void Test_createReportForUserNoData() {
            AuditLogManagementController almc = new AuditLogManagementController();
            int randomUserIntNew = _random.Next(1, 100000000);

            _dc.UserProfiles.InsertOnSubmit(new UserProfile {
                UserName = "TALM" + randomUserIntNew.ToString(CultureInfo.InvariantCulture)
            });

            _dc.SubmitChanges();

            UserProfile upNew =
                _dc.UserProfiles.FirstOrDefault(
                    userProfile =>
                    userProfile.UserName ==
                    ("TALM" + randomUserIntNew.ToString(CultureInfo.InvariantCulture)));

            bool returnValue = almc.createReportForUser(upNew.UserId, DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now);

            Assert.IsFalse(returnValue, "Data in audit report for requests with no AuditLogs.");

            _dc.UserProfiles.DeleteOnSubmit(upNew);
            _dc.SubmitChanges();
        }
        
        // Pass createReportForUser invalid date range
        [Test]
        [NUnit.Framework.ExpectedException(typeof(DateRangeInvalidException))]
        public void Test_createReportForUserInvalidDate() {
            AuditLogManagementController almc = new AuditLogManagementController();

            bool returnValue = almc.createReportForUser(up.UserId, DateTime.Now, DateTime.Now.Subtract(TimeSpan.FromHours(1)));

        }

        // Pass createReportForUser with one audit record
        [Test]
        [NUnit.Framework.ExpectedException(typeof(System.ArgumentNullException), ExpectedMessage = "path", MatchType = MessageMatch.Contains)]
        public void Test_createReportForUserWithSingleAuditLog() {

            AuditLogManagementController almc = new AuditLogManagementController();

            // create AuditLog by up
            _dc.AuditLogs.InsertOnSubmit(new AuditLog {
                RequestID = rq.RequestID,
                UserID = up.UserId,
                AuditType = (byte) Constants.AuditType.RequestView,
                AuditDate = DateTime.Now
            });
            _dc.SubmitChanges();

            bool returnValue = almc.createReportForUser(up.UserId, DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now);

        }

        #endregion

        #region Helpers
        private void addEntryHelper(Constants.AuditType at) {
            AuditLogManagementController almc = new AuditLogManagementController();
                
            // run method
            almc.addEntry(rq.RequestID, up.UserId, at);

            //// checks
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);

            Assert.IsNotNull(alCreated, "No audit log created.");
           
                Assert.IsTrue(alCreated.UserID == up.UserId,
                           "Audit Log created with wrong user ID.");
                Assert.IsTrue(Enum.GetName(typeof(Constants.AuditType), alCreated.AuditType)
                    .Equals(Enum.GetName(typeof(Constants.AuditType), at)),
                              "Audit Log created with wrong audit type.");
                Assert.IsTrue(((DateTime.Now.Subtract(TimeSpan.FromSeconds(5))) < alCreated.AuditDate),
                              "Audit Log created with incorrect date and time (too early).");
                Assert.IsTrue(DateTime.Now > alCreated.AuditDate,
                                 "Audit Log created with incorrect date and time (too late).");
            
            _dc.AuditLogs.DeleteOnSubmit(alCreated);
            _dc.SubmitChanges();
        }

        private void addEntryWithSpecifiedDateTimeHelper(Constants.AuditType at, DateTime randomizedDateTime) {
            AuditLogManagementController almc = new AuditLogManagementController();

            // run method
            
            almc.addEntry(rq.RequestID, up.UserId, at, randomizedDateTime);

            // checks
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);

            Assert.IsNotNull(alCreated, "No audit log created.");
            Assert.IsTrue(alCreated.UserID == up.UserId,
                          "Audit Log created with wrong user ID.");
            Assert.IsTrue(Enum.GetName(typeof(Constants.AuditType), alCreated.AuditType)
                .Equals(Enum.GetName(typeof(Constants.AuditType), at)),
                          "Audit Log created with wrong audit type.");
            Assert.That(alCreated.AuditDate, Is.EqualTo(randomizedDateTime).Within(1).Seconds,
                          "Audit Log created with wrong date.");

            _dc.AuditLogs.DeleteOnSubmit(alCreated);
            _dc.SubmitChanges();
        }
        #endregion

        public string AppDomainAppPath {
            get;
            set;
        }
    }
}
