using System;
using System.Linq;
using NUnit.Framework;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Service;
using Assert = NUnit.Framework.Assert;

namespace CAIRSTestProject.Unit {
    [TestFixture]
    public class TestRequestLockController {
        private CAIRSDataContext _dc = new CAIRSDataContext();
        private RequestLockManagementController _rlc = new RequestLockManagementController();
        private RequestManagementController _rmc = new RequestManagementController();
        private Request lrq, urq;
        private UserProfile up1, up2;
        private RequestLock rl;
        private const string PREPENDOR = "TRLC";

        [TestFixtureSetUp]
        public void setUp() {
            Random _random = new Random();
            long randomRequestInt1 = _random.Next(10000000, 100000000);
            long randomRequestInt2 = _random.Next(10000000, 100000000);
            long randomUserInt1 = _random.Next(10000000, 100000000);
            long randomUserInt2 = _random.Next(10000000, 100000000);

            // Create new Request Directly in DB
            _rmc.create(new RequestContent {
                patientLName =
                    randomRequestInt1.ToString()
            });

            // Create new Request Directly in DB
            _rmc.create(new RequestContent {
                patientLName =
                    randomRequestInt2.ToString()
            });

            // Create new UserProfile Directly in DB
            _dc.UserProfiles.InsertOnSubmit(new UserProfile {
                UserName = PREPENDOR + randomUserInt1.ToString()
            });

            _dc.UserProfiles.InsertOnSubmit(new UserProfile {
                UserName = PREPENDOR + randomUserInt2.ToString()
            });

            _dc.SubmitChanges();

            urq =
                _dc.Requests.FirstOrDefault(
                    request => request.PatientLName == randomRequestInt1.ToString());
            lrq =
                _dc.Requests.FirstOrDefault(
                    request => request.PatientLName == randomRequestInt2.ToString());
            up1 =
                _dc.UserProfiles.FirstOrDefault(
                    userProfile => userProfile.UserName == (PREPENDOR + randomUserInt1.ToString()));
            up2 =
                _dc.UserProfiles.FirstOrDefault(
                    userProfile => userProfile.UserName == (PREPENDOR + randomUserInt2.ToString()));
           
            rl = new RequestLock {
                RequestID = lrq.RequestID,
                UserID = up2.UserId, 
                StartTime = DateTime.Now
            };
            _dc.RequestLocks.InsertOnSubmit(rl);
            _dc.SubmitChanges();

        }

        [TestFixtureTearDown]
        public void TearDown() {
            _dc.RequestLocks.DeleteAllOnSubmit(_dc.RequestLocks.Where(lk => lk.RequestID == urq.RequestID || lk.RequestID == lrq.RequestID));
            _dc.Requests.DeleteOnSubmit(urq);
            _dc.Requests.DeleteOnSubmit(lrq);
            _dc.UserProfiles.DeleteOnSubmit(up1);
            _dc.UserProfiles.DeleteOnSubmit(up2);
            _dc.SubmitChanges();
        }

        [Test]
        public void TestLockRequest() {
            _rlc.addLock(urq.RequestID, up1.UserId);
            var toCheck = (from lck in _dc.RequestLocks
                               where lck.RequestID == urq.RequestID
                               select lck).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.UserID, up1.UserId);
            Assert.AreEqual(toCheck.RequestID, urq.RequestID);
        }

        [Test]
        public void TestIsLocked() {
            Assert.IsTrue(_rlc.isLocked(lrq.RequestID));
            Assert.IsFalse(_rlc.isLocked(urq.RequestID));
        }

        [Test]
        public void TestUnlockRequest() {
            _rlc.removeLock(lrq.RequestID);
            var toCheck = (from lck in _dc.RequestLocks
                           where lck.RequestID == lrq.RequestID
                           select lck).FirstOrDefault();
            Assert.IsNull(toCheck);
        }

        [Test]
        public void TestUnlockUnlockedRequest() {
            _rlc.removeLock(urq.RequestID);
        }

        [Test]
        [NUnit.Framework.ExpectedException]
        public void TestLockLockedRequest() {
            _rlc.addLock(lrq.RequestID, up1.UserId);
        }

        [Test]
        public void TestGetRequestLock() {
            RequestLock rlk = _rlc.getRequestLock(lrq.RequestID);
            Assert.AreEqual(rlk.RequestID, rl.RequestID);
            Assert.AreEqual(rlk.UserID, rl.UserID);
            Assert.That(rlk.StartTime, Is.EqualTo(rl.StartTime).Within(1).Seconds);
        }
    }
}
