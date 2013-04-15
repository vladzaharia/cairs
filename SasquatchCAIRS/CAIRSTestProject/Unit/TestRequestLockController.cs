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
        private Request _lrq, _urq;
        private UserProfile _up1, _up2;
        private RequestLock _rl;
        private const string PREPENDOR = "TRLC";

        [TestFixtureSetUp]
        public void setUp() {
            Random random = new Random();
            long randomRequestInt1 = random.Next(10000000, 100000000);
            long randomRequestInt2 = random.Next(10000000, 100000000);
            long randomUserInt1 = random.Next(10000000, 100000000);
            long randomUserInt2 = random.Next(10000000, 100000000);

            // Create new Request Directly in DB
            _rmc.create(new RequestContent {
                patientLName =
                    randomRequestInt1.ToString()});

            // Create new Request Directly in DB
            _rmc.create(new RequestContent {
                patientLName =
                    randomRequestInt2.ToString()});

            // Create new UserProfile Directly in DB
            _dc.UserProfiles.InsertOnSubmit(new UserProfile {
                UserName = PREPENDOR + randomUserInt1.ToString()});

            _dc.UserProfiles.InsertOnSubmit(new UserProfile {
                UserName = PREPENDOR + randomUserInt2.ToString()});

            _dc.SubmitChanges();

            _urq =
                _dc.Requests.FirstOrDefault(
                    request => request.PatientLName == randomRequestInt1.ToString());
            _lrq =
                _dc.Requests.FirstOrDefault(
                    request => request.PatientLName == randomRequestInt2.ToString());
            _up1 =
                _dc.UserProfiles.FirstOrDefault(
                    userProfile => userProfile.UserName == (PREPENDOR + randomUserInt1.ToString()));
            _up2 =
                _dc.UserProfiles.FirstOrDefault(
                    userProfile => userProfile.UserName == (PREPENDOR + randomUserInt2.ToString()));
           
            _rl = new RequestLock {
                RequestID = _lrq.RequestID,
                UserID = _up2.UserId, 
                StartTime = DateTime.Now
            };
            _dc.RequestLocks.InsertOnSubmit(_rl);
            _dc.SubmitChanges();

        }

        [TestFixtureTearDown]
        public void TearDown() {
            _dc.RequestLocks.DeleteAllOnSubmit(_dc.RequestLocks.Where(lk => lk.RequestID == _urq.RequestID || lk.RequestID == _lrq.RequestID));
            _dc.Requests.DeleteOnSubmit(_urq);
            _dc.Requests.DeleteOnSubmit(_lrq);
            _dc.UserProfiles.DeleteOnSubmit(_up1);
            _dc.UserProfiles.DeleteOnSubmit(_up2);
            _dc.SubmitChanges();
        }

        [Test]
        public void TestLockRequest() {
            _rlc.addLock(_urq.RequestID, _up1.UserId);
            var toCheck = (from lck in _dc.RequestLocks
                               where lck.RequestID == _urq.RequestID
                               select lck).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.UserID, _up1.UserId);
            Assert.AreEqual(toCheck.RequestID, _urq.RequestID);
        }

        [Test]
        public void TestIsLocked() {
            Assert.IsTrue(_rlc.isLocked(_lrq.RequestID));
            Assert.IsFalse(_rlc.isLocked(_urq.RequestID));
        }

        [Test]
        public void TestUnlockRequest() {
            _rlc.removeLock(_lrq.RequestID);
            var toCheck = (from lck in _dc.RequestLocks
                           where lck.RequestID == _lrq.RequestID
                           select lck).FirstOrDefault();
            Assert.IsNull(toCheck);
        }

        [Test]
        public void TestUnlockUnlockedRequest() {
            _rlc.removeLock(_urq.RequestID);
        }

        [Test]
        [NUnit.Framework.ExpectedException]
        public void TestLockLockedRequest() {
            _rlc.addLock(_lrq.RequestID, _up1.UserId);
        }

        [Test]
        public void TestGetRequestLock() {
            RequestLock rlk = _rlc.getRequestLock(_lrq.RequestID);
            Assert.AreEqual(rlk.RequestID, _rl.RequestID);
            Assert.AreEqual(rlk.UserID, _rl.UserID);
            Assert.That(rlk.StartTime, Is.EqualTo(_rl.StartTime).Within(1).Seconds);
        }
    }
}
