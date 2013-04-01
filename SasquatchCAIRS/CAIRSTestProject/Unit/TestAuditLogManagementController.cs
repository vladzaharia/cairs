using System;
using System.Globalization;
using System.Linq;
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

        [TestFixtureSetUp]
        public void SetUp() {
            Random random = new Random();
            long randomRequestInt = random.Next(1, 100000000);
            long randomUserInt = random.Next(1, 100000000);
            RequestManagementController rmc = new RequestManagementController();

            // Create new Request Directly in DB
            rmc.create(new RequestContent {
                patientLName =
                    randomRequestInt.ToString(CultureInfo.InvariantCulture)
            });

            _dc.UserProfiles.InsertOnSubmit(new UserProfile {
                UserName = randomUserInt.ToString(CultureInfo.InvariantCulture)
            });

            _dc.SubmitChanges();

            rq =
                _dc.Requests.FirstOrDefault(
                    request => 
                    request.PatientLName == 
                    randomRequestInt.ToString(CultureInfo.InvariantCulture));
            up =
                _dc.UserProfiles.FirstOrDefault(
                    userProfile =>
                    userProfile.UserName ==
                    randomUserInt.ToString(CultureInfo.InvariantCulture));
        }

        [TestFixtureTearDown]
        public void TearDown() {
            _dc.AuditLogs.DeleteOnSubmit(_dc.AuditLogs.FirstOrDefault(au => au.RequestID == rq.RequestID));
            _dc.Requests.DeleteOnSubmit(rq);
            _dc.UserProfiles.DeleteOnSubmit(up);
            _dc.SubmitChanges();
        }

        [Test]
        public void Test_addEntry() {
            AuditLogManagementController almc = new AuditLogManagementController();
                
            // run method
            almc.addEntry(rq.RequestID, up.UserId, Constants.AuditType.RequestView);

            //// checks
            AuditLog alCreated = _dc.AuditLogs.FirstOrDefault(r => r.RequestID == rq.RequestID);

            Assert.IsNotNull(alCreated, "No audit log created.");
            Assert.IsTrue(alCreated.UserID == up.UserId,
                          "Audit Log created with wrong user ID.");
            Assert.IsTrue(Enum.GetName(typeof(Constants.AuditType), alCreated.AuditType)
                .Equals(Enum.GetName(typeof(Constants.AuditType), Constants.AuditType.RequestView)),
                          "Audit Log created with wrong user ID.");
        }
    }
}
