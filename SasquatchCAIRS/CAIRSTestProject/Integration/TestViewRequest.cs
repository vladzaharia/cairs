using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Service;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class TestViewRequest {
        private CommonTestingMethods _ctm = new CommonTestingMethods();
        private IWebDriver _driver;
        private Random _random = new Random();
        private CAIRSDataContext _cdc = new CAIRSDataContext();

        [TestFixtureSetUp]
        public void Setup() {
            _driver = _ctm.getDriver();
            _ctm.addAllRoles();
        }

        [TestFixtureTearDown]
        public void Teardown() {
            _driver.Quit();
            _ctm.getAdminDriver().Quit();
        }

        /// <summary>
        ///     Test View Request with Invalid status and no Administrator Role
        /// </summary>
        [Test]
        public void TestViewRequestInvalid() {
            // Create a test request in the DB
            var rc = new RequestContent {
                patientFName = "VRInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Invalid
            };
            var rmc = new RequestManagementController();
            long rid = rmc.create(rc);

            // Remove the Viewer Role from the User
            _ctm.removeRole(Constants.Roles.ADMINISTRATOR);

            // Attempt to go to the appropriate View Request Page Directly
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _driver.Navigate()
                   .GoToUrl(CommonTestingMethods.getURL() + "/Request/Details/" +
                            rid.ToString(CultureInfo.InvariantCulture));

            // Assert that we're redirected to the not authorized page
            StringAssert.Contains("/Request/Details", _driver.Url);
            _driver.FindElement(By.Id("error-header"));
            IWebElement msg = _driver.FindElement(By.Id("error-message"));
            StringAssert.AreEqualIgnoringCase(
                "You do not have the necessary permissions to view this request.",
                msg.Text);

            // Cleanup
            Request rq = _cdc.Requests.FirstOrDefault(r => r.RequestID == rid);
            if (rq == null) {
                Assert.Fail("Request is null");
            }

            _cdc.Requests.DeleteOnSubmit(rq);
            _cdc.SubmitChanges();

            _ctm.addRole(Constants.Roles.ADMINISTRATOR);
        }

        /// <summary>
        ///     Test View Request that is Locked to Another User
        /// </summary>
        [Test]
        public void TestViewRequestLockedToAnother() {
            // Create a test request in the DB
            var rc = new RequestContent {
                patientFName = "VRInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            var rmc = new RequestManagementController();
            long rid = rmc.create(rc);

            // Create the User
            var up = new UserProfile {
                UserName = "VRInt-" +
                           _random.Next()
                                  .ToString(CultureInfo.InvariantCulture)
            };
            _cdc.UserProfiles.InsertOnSubmit(up);
            _cdc.SubmitChanges();

            // Create the Lock
            var rlmc = new RequestLockManagementController();
            rlmc.addLock(rid, up.UserId);

            // Remove the Viewer Role from the User
            _ctm.removeRole(Constants.Roles.ADMINISTRATOR);

            // Attempt to go to the appropriate View Request Page Directly
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _driver.Navigate()
                   .GoToUrl(CommonTestingMethods.getURL() + "/Request/Details/" +
                            rid.ToString(CultureInfo.InvariantCulture));
            _driver.FindElement(By.Id("error-header"));
            IWebElement msg = _driver.FindElement(By.Id("error-message"));
            StringAssert.AreEqualIgnoringCase(
                "This request has been locked to another person and cannot be viewed until unlocked.",
                msg.Text);

            // Assert that we're redirected to the not authorized page
            StringAssert.Contains("/Request/Details", _driver.Url);

            // Cleanup
            rlmc.removeLock(rid);
            _cdc.UserProfiles.DeleteOnSubmit(up);
            Request rq = _cdc.Requests.FirstOrDefault(r => r.RequestID == rid);
            if (rq == null) {
                Assert.Fail("Request is null");
            }
            _cdc.Requests.DeleteOnSubmit(rq);
            _cdc.SubmitChanges();

            _ctm.addRole(Constants.Roles.ADMINISTRATOR);
        }

        /// <summary>
        ///     Test View Request with no Viewer Role
        /// </summary>
        [Test]
        public void TestViewRequestNotViewer() {
            // Create a test request in the DB
            var rc = new RequestContent {
                patientFName = "VRInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            var rmc = new RequestManagementController();
            long rid = rmc.create(rc);

            // Remove the Viewer Role from the User
            _ctm.removeRole(Constants.Roles.VIEWER);

            // Attempt to go to the appropriate View Request Page Directly
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _driver.Navigate()
                   .GoToUrl(CommonTestingMethods.getURL() + "/Request/Details/" +
                            rid.ToString(CultureInfo.InvariantCulture));

            // Assert that we're redirected to the not authorized page
            StringAssert.Contains("/Account/Auth", _driver.Url);

            // Cleanup
            Request rq = _cdc.Requests.FirstOrDefault(r => r.RequestID == rid);
            if (rq == null) {
                Assert.Fail("Request is null");
            }

            _cdc.Requests.DeleteOnSubmit(rq);
            _cdc.SubmitChanges();

            _ctm.addRole(Constants.Roles.VIEWER);
        }

        /// <summary>
        ///     Test View Request with Open status and no Request Editor Role
        /// </summary>
        [Test]
        public void TestViewRequestOpen() {
            // Create a test request in the DB
            var rc = new RequestContent {
                patientFName = "VRInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Open
            };
            var rmc = new RequestManagementController();
            long rid = rmc.create(rc);

            // Remove the Viewer Role from the User
            _ctm.removeRole(Constants.Roles.REQUEST_EDITOR);
            _ctm.removeRole(Constants.Roles.ADMINISTRATOR);

            // Attempt to go to the appropriate View Request Page Directly
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _driver.Navigate()
                   .GoToUrl(CommonTestingMethods.getURL() + "/Request/Details/" +
                            rid.ToString(CultureInfo.InvariantCulture));

            // Assert that we're still on the same page
            StringAssert.Contains("/Request/Details", _driver.Url);
            _driver.FindElement(By.Id("error-header"));
            IWebElement msg = _driver.FindElement(By.Id("error-message"));
            StringAssert.AreEqualIgnoringCase(
                "You do not have the necessary permissions to view this request.",
                msg.Text);

            // Cleanup
            Request rq = _cdc.Requests.FirstOrDefault(r => r.RequestID == rid);
            if (rq == null) {
                Assert.Fail("Request is null");
            }

            _cdc.Requests.DeleteOnSubmit(rq);
            _cdc.SubmitChanges();

            _ctm.addRole(Constants.Roles.REQUEST_EDITOR);
            _ctm.addRole(Constants.Roles.ADMINISTRATOR);
        }

        /// <summary>
        ///     Test View Request which works
        /// </summary>
        [Test]
        public void TestViewRequestWorking() {
            // Add some Dependencies
            var rt = new RequestorType {
                Code = _random.Next(1000000)
                              .ToString(CultureInfo.InvariantCulture),
                Value = "VRInt-" +
                        _random.Next()
                               .ToString(CultureInfo.InvariantCulture),
                Active = true
            };
            _cdc.RequestorTypes.InsertOnSubmit(rt);

            var qt = new QuestionType {
                Code = _random.Next(1000000)
                              .ToString(CultureInfo.InvariantCulture),
                Value = "VRInt-" +
                        _random.Next()
                               .ToString(CultureInfo.InvariantCulture),
                Active = true
            };
            _cdc.QuestionTypes.InsertOnSubmit(qt);

            var tg = new TumourGroup {
                Code = _random.Next(1000000)
                              .ToString(CultureInfo.InvariantCulture),
                Value = "VRInt-" +
                        _random.Next()
                               .ToString(CultureInfo.InvariantCulture),
                Active = true
            };
            _cdc.TumourGroups.InsertOnSubmit(tg);

            var r = new Region {
                Code = _random.Next(1000000)
                              .ToString(CultureInfo.InvariantCulture),
                Value = "VRInt-" +
                        _random.Next()
                               .ToString(CultureInfo.InvariantCulture),
                Active = true
            };
            _cdc.Regions.InsertOnSubmit(r);

            var k = new Keyword {
                KeywordValue = "VRInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                Active = true
            };
            _cdc.Keywords.InsertOnSubmit(k);

            // Submit our changes so far.
            _cdc.SubmitChanges();

            // Create a test request in the DB
            var rc = new RequestContent {
                patientFName = "VRInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                patientLName = "VRInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                patientAgencyID = _random.Next()
                                         .ToString(CultureInfo.InvariantCulture),
                patientGender = Constants.Gender.Female,
                patientAge = 255,
                requestorTypeID = rt.RequestorTypeID,
                regionID = r.RegionID,
                requestorFirstName = "VRInt-" +
                                     _random.Next()
                                            .ToString(
                                                CultureInfo.InvariantCulture),
                requestorLastName = "VRInt-" +
                                    _random.Next()
                                           .ToString(
                                               CultureInfo.InvariantCulture),
                requestorEmail = _random.Next()
                                        .ToString(CultureInfo.InvariantCulture) +
                                 "@example.com",
                requestorPhoneNum = _random.Next()
                                           .ToString(
                                               CultureInfo.InvariantCulture),
                requestorPhoneExt = _random.Next()
                                           .ToString(
                                               CultureInfo.InvariantCulture)
            };

            var refCont = new ReferenceContent {
                referenceType = Constants.ReferenceType.Text,
                referenceString = "VRInt-" +
                                  _random.Next()
                                         .ToString(
                                             CultureInfo.InvariantCulture)
            };

            var qrc = new QuestionResponseContent {
                question = "VRInt-" +
                           _random.Next()
                                  .ToString(
                                      CultureInfo.InvariantCulture),
                response = "VRInt-" +
                           _random.Next()
                                  .ToString(
                                      CultureInfo.InvariantCulture),
                specialNotes = "VRInt-" +
                               _random.Next()
                                      .ToString(
                                          CultureInfo.InvariantCulture),
                consequence = Constants.Consequence.Certain,
                severity = Constants.Severity.Major,
                keywords = new List<string> {k.KeywordValue},
                referenceList = new List<ReferenceContent> {refCont},
                timeSpent = 255,
                questionTypeID = qt.QuestionTypeID,
                tumourGroupID = tg.TumourGroupID
            };
            rc.addQuestionResponse(qrc);
            var rmc = new RequestManagementController();
            long rid = rmc.create(rc);

            var dmc = new DropdownManagementController();

            // Attempt to go to the appropriate View Request Page Directly
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _driver.Navigate()
                   .GoToUrl(CommonTestingMethods.getURL() + "/Request/Details/" +
                            rid.ToString(CultureInfo.InvariantCulture));

            // Assert that we're not redirected
            StringAssert.Contains("/Request/Details", _driver.Url);

            // Go through fields and check values
            IWebElement element = _driver.FindElement(By.Id("status"));
            StringAssert.AreEqualIgnoringCase(rc.requestStatus.ToString(),
                                              element.Text);

            element = _driver.FindElement(By.Id("total-time-spent"));
            StringAssert.Contains(qrc.timeSpent.ToString(), element.Text);

            element = _driver.FindElement(By.Id("requestor-name"));
            StringAssert.Contains(rc.requestorFirstName, element.Text);
            StringAssert.Contains(rc.requestorLastName, element.Text);

            element = _driver.FindElement(By.Id("requestor-email"));
            StringAssert.AreEqualIgnoringCase(rc.requestorEmail, element.Text);

            element = _driver.FindElement(By.Id("requestor-phone"));
            StringAssert.Contains(rc.requestorPhoneNum, element.Text);
            StringAssert.Contains(rc.requestorPhoneExt, element.Text);

            element = _driver.FindElement(By.Id("caller-type"));
            StringAssert.Contains(rt.Code, element.Text);
            StringAssert.Contains(rt.Value, element.Text);

            element = _driver.FindElement(By.Id("region"));
            StringAssert.Contains(r.Code, element.Text);
            StringAssert.Contains(r.Value, element.Text);

            element = _driver.FindElement(By.Id("patient-name"));
            StringAssert.Contains(rc.patientFName, element.Text);
            StringAssert.Contains(rc.patientLName, element.Text);

            element = _driver.FindElement(By.Id("patient-gender"));
            StringAssert.AreEqualIgnoringCase(rc.patientGender.ToString(),
                                              element.Text);

            element = _driver.FindElement(By.Id("patient-id"));
            StringAssert.AreEqualIgnoringCase(rc.patientAgencyID, element.Text);

            element = _driver.FindElement(By.Id("patient-age"));
            StringAssert.AreEqualIgnoringCase(rc.patientAge.ToString(),
                                              element.Text);

            element = _driver.FindElement(By.ClassName("question"));
            StringAssert.AreEqualIgnoringCase(qrc.question, element.Text);

            element = _driver.FindElement(By.ClassName("response"));
            StringAssert.AreEqualIgnoringCase(qrc.response, element.Text);

            element = _driver.FindElement(By.ClassName("special-notes"));
            StringAssert.AreEqualIgnoringCase(qrc.specialNotes, element.Text);

            element = _driver.FindElement(By.ClassName("question-type"));
            StringAssert.Contains(qt.Code, element.Text);
            StringAssert.Contains(qt.Value, element.Text);

            element = _driver.FindElement(By.ClassName("tumour-group"));
            StringAssert.Contains(tg.Code, element.Text);
            StringAssert.Contains(tg.Value, element.Text);

            element = _driver.FindElement(By.ClassName("time-spent"));
            StringAssert.Contains(qrc.timeSpent.ToString(), element.Text);

            element = _driver.FindElement(By.ClassName("score"));
            StringAssert.AreEqualIgnoringCase(
                1.ToString(CultureInfo.InvariantCulture), element.Text);

            element = _driver.FindElement(By.ClassName("impact-sev"));
            StringAssert.AreEqualIgnoringCase(qrc.severity.ToString(),
                                              element.Text);

            element = _driver.FindElement(By.ClassName("impact-cons"));
            StringAssert.AreEqualIgnoringCase(qrc.consequence.ToString(),
                                              element.Text);

            element = _driver.FindElement(By.ClassName("reference-string"));
            StringAssert.AreEqualIgnoringCase(refCont.referenceString,
                                              element.Text);

            // Cleanup
            var cdc2 = new CAIRSDataContext();

            IQueryable<KeywordQuestion> kqs =
                cdc2.KeywordQuestions.Where(kq => kq.RequestID == rid);
            Assert.IsTrue(kqs.Any());
            cdc2.KeywordQuestions.DeleteAllOnSubmit(kqs);

            IQueryable<AuditLog> als =
                cdc2.AuditLogs.Where(al => al.RequestID == rid);
            Assert.IsTrue(als.Any());
            cdc2.AuditLogs.DeleteAllOnSubmit(als);

            IQueryable<QuestionResponse> qrs =
                cdc2.QuestionResponses.Where(dbQr => dbQr.RequestID == rid);
            Assert.IsTrue(qrs.Any());
            cdc2.QuestionResponses.DeleteAllOnSubmit(qrs);

            IQueryable<Request> rs =
                cdc2.Requests.Where(rq => rq.RequestID == rid);
            Assert.IsTrue(rs.Any());
            cdc2.Requests.DeleteAllOnSubmit(rs);

            IQueryable<Reference> refs =
                cdc2.References.Where(rf => rf.RequestID == rid);
            Assert.IsTrue(refs.Any());
            cdc2.References.DeleteAllOnSubmit(refs);

            IQueryable<RequestorType> rqts =
                cdc2.RequestorTypes.Where(
                    rqt => rqt.RequestorTypeID == rt.RequestorTypeID);
            Assert.IsTrue(rqts.Any());
            cdc2.RequestorTypes.DeleteAllOnSubmit(rqts);

            IQueryable<QuestionType> qts =
                cdc2.QuestionTypes.Where(
                    dbQt => dbQt.Value == qt.Value);
            Assert.IsTrue(qts.Any());
            cdc2.QuestionTypes.DeleteAllOnSubmit(qts);

            IQueryable<TumourGroup> tgs =
                cdc2.TumourGroups.Where(
                    dbTg => dbTg.TumourGroupID == tg.TumourGroupID);
            Assert.IsTrue(tgs.Any());
            cdc2.TumourGroups.DeleteAllOnSubmit(tgs);

            IQueryable<Region> regions =
                cdc2.Regions.Where(dbRg => dbRg.RegionID == r.RegionID);
            Assert.IsTrue(regions.Any());
            cdc2.Regions.DeleteAllOnSubmit(regions);

            IQueryable<Keyword> keywords =
                cdc2.Keywords.Where(kw => kw.KeywordID == k.KeywordID);
            Assert.IsTrue(keywords.Any());
            cdc2.Keywords.DeleteAllOnSubmit(keywords);

            IQueryable<Request> rqs =
                cdc2.Requests.Where(dbRq => dbRq.RequestID == rid);
            Assert.IsTrue(rqs.Any());
            cdc2.Requests.DeleteAllOnSubmit(rqs);

            cdc2.SubmitChanges();
        }
    }
}