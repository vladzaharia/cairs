using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class SearchTests {
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
        ///     Test Advanced Search with "All" entry.
        /// </summary>
        [Test]
        public void TestAdvancedSearchAll() {
            var kw1 = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            var kw2 = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            _cdc.Keywords.InsertAllOnSubmit(new List<Keyword> {kw1, kw2});
            _cdc.SubmitChanges();

            // Setup the request
            var qrc1 = new QuestionResponseContent {
                keywords = new List<string> {kw1.KeywordValue, kw2.KeywordValue}
            };
            var rc1 = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Open
            };
            rc1.addQuestionResponse(qrc1);

            var qrc2 = new QuestionResponseContent {
                keywords = new List<string> {kw1.KeywordValue}
            };
            var rc2 = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            rc2.addQuestionResponse(qrc2);

            // Create the RequestContents
            var rmc = new RequestManagementController();
            long rid1 = rmc.create(rc1);
            long rid2 = rmc.create(rc2);

            //========================================
            // T-Minus 5, 4, 3, 2, 1. Blast Off!
            //========================================
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADVANCED_SEARCH,
                              "/Search/Advanced");

            _driver.FindElement(By.Id("allKeywords"))
                   .SendKeys(kw1.KeywordValue + ", ");
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Check Results
            ReadOnlyCollection<IWebElement> row1 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            ReadOnlyCollection<IWebElement> row2 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid2 + "']"));

            Assert.IsTrue(row1.Count > 0, "Request 1 not in results!");
            Assert.IsTrue(row2.Count > 0, "Request 2 not in results!");

            // Modify the Search
            _ctm.findAndClick(Constants.UIString.ItemIDs.MODIFY_SEARCH,
                              "/Search/Modify");
            _driver.FindElement(By.Id("allKeywords"))
                   .SendKeys(kw2.KeywordValue + ", ");
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Check Results
            row1 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            row2 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid2 + "']"));

            Assert.IsTrue(row1.Count > 0, "Request 1 not in results!");
            Assert.IsTrue(row2.Count == 0, "Request 2 in results!");

            //===============================================
            // Scrubbing Bubbles scrub away all the data!
            //===============================================
            // Cleanup KeywordQuestion
            var cdc2 = new CAIRSDataContext();
            IQueryable<KeywordQuestion> keyq1 =
                cdc2.KeywordQuestions.Where(
                    kq => kq.KeywordID == kw1.KeywordID);
            IQueryable<KeywordQuestion> keyq2 =
                cdc2.KeywordQuestions.Where(
                    kq => kq.KeywordID == kw2.KeywordID);
            if (keyq1 == null || keyq2 == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq1);
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq2);
            cdc2.SubmitChanges();

            // Cleanup Keyword
            Keyword kwDel1 =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw1.KeywordID);
            Keyword kwDel2 =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw2.KeywordID);
            if (kwDel1 == null || kwDel2 == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.Keywords.DeleteAllOnSubmit(new List<Keyword> {kwDel1, kwDel2});
            cdc2.SubmitChanges();

            // Cleanup QuestionResponse
            QuestionResponse qresp1 =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid1);
            QuestionResponse qresp2 =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid2);
            if (qresp1 == null || qresp2 == null) {
                Assert.Fail("QuestionResponse can't be found for Teardown!");
            }
            cdc2.QuestionResponses.DeleteAllOnSubmit(
                new List<QuestionResponse> {qresp1, qresp2});
            cdc2.SubmitChanges();

            // Cleanup Request
            Request req1 = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid1);
            Request req2 = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid2);
            if (req1 == null || req2 == null) {
                Assert.Fail("Request can't be found for Teardown!");
            }
            cdc2.Requests.DeleteAllOnSubmit(new List<Request> {req1, req2});
            cdc2.SubmitChanges();
        }

        /// <summary>
        ///     Test Advanced Search with "Any" keyword.
        /// </summary>
        [Test]
        public void TestAdvancedSearchAny() {
            var kw1 = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            var kw2 = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            _cdc.Keywords.InsertAllOnSubmit(new List<Keyword> {kw1, kw2});
            _cdc.SubmitChanges();

            // Setup the request
            var qrc1 = new QuestionResponseContent {
                keywords = new List<string> {kw1.KeywordValue}
            };
            var rc1 = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Open
            };
            rc1.addQuestionResponse(qrc1);

            TumourGroup tg = _cdc.TumourGroups.FirstOrDefault(t => t.Active);
            if (tg == null) {
                Assert.Fail("No active TumourGroups in the system!");
            }
            var qrc2 = new QuestionResponseContent {
                keywords = new List<string> {kw2.KeywordValue},
                tumourGroupID = tg.TumourGroupID
            };
            var rc2 = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Completed
            };
            rc2.addQuestionResponse(qrc2);

            QuestionType qt = _cdc.QuestionTypes.FirstOrDefault(q => q.Active);
            if (qt == null) {
                Assert.Fail("No active QuestionTypes in the system!");
            }
            var qrc3 = new QuestionResponseContent {
                keywords = new List<string> {kw2.KeywordValue},
                questionTypeID = qt.QuestionTypeID
            };
            var rc3 = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Invalid
            };
            rc3.addQuestionResponse(qrc3);

            // Create the RequestContents
            var rmc = new RequestManagementController();
            long rid1 = rmc.create(rc1);
            long rid2 = rmc.create(rc2);
            long rid3 = rmc.create(rc3);

            //========================================
            // Vroom! Vroom!
            //========================================
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADVANCED_SEARCH,
                              "/Search/Advanced");

            _driver.FindElement(By.Id("keywordString"))
                   .SendKeys(kw1.KeywordValue + ", " + kw2.KeywordValue + ", ");
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Check Results
            ReadOnlyCollection<IWebElement> row1 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            ReadOnlyCollection<IWebElement> row2 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid2 + "']"));
            ReadOnlyCollection<IWebElement> row3 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid3 + "']"));

            Assert.IsTrue(row1.Count > 0, "Request 1 not in results!");
            Assert.IsTrue(row2.Count > 0, "Request 2 not in results!");
            Assert.IsTrue(row3.Count > 0, "Request 3 not in results!");

            // Modify the Search
            _ctm.findAndClick(Constants.UIString.ItemIDs.MODIFY_SEARCH,
                              "/Search/Modify");
            _driver.FindElement(By.Id(Constants.RequestStatus.Open.ToString()))
                   .FindElement(By.ClassName("icon")).Click();
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Check Results
            row1 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            row2 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid2 + "']"));
            row3 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid3 + "']"));

            Assert.IsTrue(row1.Count > 0, "Request 1 not in results!");
            Assert.IsTrue(row2.Count == 0, "Request 2 in results!");
            Assert.IsTrue(row3.Count == 0, "Request 3 in results!");

            // Modify the Search
            _ctm.findAndClick(Constants.UIString.ItemIDs.MODIFY_SEARCH,
                              "/Search/Modify");

            // Change the Status from Open to Completed
            _driver.FindElement(By.Id(Constants.RequestStatus.Open.ToString()))
                   .FindElement(By.ClassName("icon")).Click();
            _driver.FindElement(
                By.Id(Constants.RequestStatus.Completed.ToString()))
                   .FindElement(By.ClassName("icon")).Click();

            // Add a Tumour Group Search
            _driver.FindElement(By.Id("tumour-group"))
                   .FindElement(
                       By.Id(
                           tg.TumourGroupID.ToString(
                               CultureInfo.InvariantCulture)))
                   .FindElement(By.ClassName("icon"))
                   .Click();

            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Check Results
            row1 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            row2 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid2 + "']"));
            row3 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid3 + "']"));

            Assert.IsTrue(row1.Count == 0, "Request 1 in results!");
            Assert.IsTrue(row2.Count > 0, "Request 2 not in results!");
            Assert.IsTrue(row3.Count == 0, "Request 3 in results!");

            // Modify the Search
            _ctm.findAndClick(Constants.UIString.ItemIDs.MODIFY_SEARCH,
                              "/Search/Modify");

            // Change the Status from Completed to Invalid
            _driver.FindElement(
                By.Id(Constants.RequestStatus.Completed.ToString()))
                   .FindElement(By.ClassName("icon")).Click();
            _driver.FindElement(By.Id(Constants.RequestStatus.Invalid.ToString()))
                   .FindElement(By.ClassName("icon")).Click();

            // Remove the Tumour Group Search
            _driver.FindElement(By.Id("tumour-group"))
                   .FindElement(
                       By.Id(
                           tg.TumourGroupID.ToString(
                               CultureInfo.InvariantCulture)))
                   .FindElement(By.ClassName("icon"))
                   .Click();

            // Add a Question Type Search
            _driver.FindElement(By.Id("question-type"))
                   .FindElement(
                       By.Id(
                           qt.QuestionTypeID.ToString(
                               CultureInfo.InvariantCulture)))
                   .FindElement(By.ClassName("icon"))
                   .Click();

            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Check Results
            row1 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            row2 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid2 + "']"));
            row3 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid3 + "']"));

            Assert.IsTrue(row1.Count == 0, "Request 1 in results!");
            Assert.IsTrue(row2.Count == 0, "Request 2 in results!");
            Assert.IsTrue(row3.Count > 0, "Request 3 not in results!");

            //========================================
            // Mr. Clean to the rescue!
            //========================================
            // Cleanup KeywordQuestion
            var cdc2 = new CAIRSDataContext();
            IQueryable<KeywordQuestion> keyq1 =
                cdc2.KeywordQuestions.Where(kq => kq.RequestID == rid1);
            IQueryable<KeywordQuestion> keyq2 =
                cdc2.KeywordQuestions.Where(kq => kq.RequestID == rid2);
            IQueryable<KeywordQuestion> keyq3 =
                cdc2.KeywordQuestions.Where(kq => kq.RequestID == rid3);
            if (keyq1 == null || keyq2 == null || keyq3 == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq1);
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq2);
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq3);
            cdc2.SubmitChanges();

            // Cleanup Keyword
            Keyword kwDel1 =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw1.KeywordID);
            Keyword kwDel2 =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw2.KeywordID);
            if (kwDel1 == null || kwDel2 == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.Keywords.DeleteAllOnSubmit(new List<Keyword> {kwDel1, kwDel2});
            cdc2.SubmitChanges();

            // Cleanup QuestionResponse
            QuestionResponse qresp1 =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid1);
            QuestionResponse qresp2 =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid2);
            QuestionResponse qresp3 =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid3);
            if (qresp1 == null || qresp2 == null || qresp3 == null) {
                Assert.Fail("QuestionResponse can't be found for Teardown!");
            }
            cdc2.QuestionResponses.DeleteAllOnSubmit(
                new List<QuestionResponse> {qresp1, qresp2, qresp3});
            cdc2.SubmitChanges();

            // Cleanup Request
            Request req1 = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid1);
            Request req2 = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid2);
            Request req3 = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid3);
            if (req1 == null || req2 == null || req3 == null) {
                Assert.Fail("Request can't be found for Teardown!");
            }
            cdc2.Requests.DeleteAllOnSubmit(new List<Request> {req1, req2, req3});
            cdc2.SubmitChanges();
        }

        /// <summary>
        ///     Test Advanced Search with Combination of entries.
        /// </summary>
        [Test]
        public void TestAdvancedSearchCombo() {
            var kw1 = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            var kw2 = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            var kw3 = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            _cdc.Keywords.InsertAllOnSubmit(new List<Keyword> {kw1, kw2, kw3});
            _cdc.SubmitChanges();

            // Setup the request
            var qrc1 = new QuestionResponseContent {
                keywords =
                    new List<string> {
                        kw1.KeywordValue,
                        kw2.KeywordValue,
                        kw3.KeywordValue
                    }
            };
            var rc1 = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Open
            };
            rc1.addQuestionResponse(qrc1);

            var qrc2 = new QuestionResponseContent {
                keywords = new List<string> {kw2.KeywordValue, kw3.KeywordValue}
            };
            var rc2 = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            rc2.addQuestionResponse(qrc2);

            // Create the RequestContents
            var rmc = new RequestManagementController();
            long rid1 = rmc.create(rc1);
            long rid2 = rmc.create(rc2);

            //========================================
            // All Systems are Go!
            //========================================
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADVANCED_SEARCH,
                              "/Search/Advanced");

            _driver.FindElement(By.Id("keywordString"))
                   .SendKeys(kw1.KeywordValue + ", " + kw2.KeywordValue + ", ");
            _driver.FindElement(By.Id("allKeywords"))
                   .SendKeys(kw2.KeywordValue + ", " + kw3.KeywordValue + ", ");
            _driver.FindElement(By.Id("noneKeywords"))
                   .SendKeys(kw1.KeywordValue + ", ");
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Check Results
            ReadOnlyCollection<IWebElement> row1 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            ReadOnlyCollection<IWebElement> row2 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid2 + "']"));

            Assert.IsTrue(row1.Count == 0, "Request 1 in results!");
            Assert.IsTrue(row2.Count > 0, "Request 2 not in results!");

            //===============================================
            // Windex. It fixes all your problems.
            //===============================================
            // Cleanup KeywordQuestion
            var cdc2 = new CAIRSDataContext();
            IQueryable<KeywordQuestion> keyq1 =
                cdc2.KeywordQuestions.Where(
                    kq => kq.KeywordID == kw1.KeywordID);
            IQueryable<KeywordQuestion> keyq2 =
                cdc2.KeywordQuestions.Where(
                    kq => kq.KeywordID == kw2.KeywordID);
            IQueryable<KeywordQuestion> keyq3 =
                cdc2.KeywordQuestions.Where(
                    kq => kq.KeywordID == kw3.KeywordID);
            if (keyq1 == null || keyq2 == null || keyq3 == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq1);
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq2);
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq3);
            cdc2.SubmitChanges();

            // Cleanup Keyword
            Keyword kwDel1 =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw1.KeywordID);
            Keyword kwDel2 =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw2.KeywordID);
            Keyword kwDel3 =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw3.KeywordID);
            if (kwDel1 == null || kwDel2 == null || kwDel3 == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.Keywords.DeleteAllOnSubmit(new List<Keyword> {
                kwDel1,
                kwDel2,
                kwDel3
            });
            cdc2.SubmitChanges();

            // Cleanup QuestionResponse
            QuestionResponse qresp1 =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid1);
            QuestionResponse qresp2 =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid2);
            if (qresp1 == null || qresp2 == null) {
                Assert.Fail("QuestionResponse can't be found for Teardown!");
            }
            cdc2.QuestionResponses.DeleteAllOnSubmit(
                new List<QuestionResponse> {qresp1, qresp2});
            cdc2.SubmitChanges();

            // Cleanup Request
            Request req1 = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid1);
            Request req2 = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid2);
            if (req1 == null || req2 == null) {
                Assert.Fail("Request can't be found for Teardown!");
            }
            cdc2.Requests.DeleteAllOnSubmit(new List<Request> {req1, req2});
            cdc2.SubmitChanges();
        }

        /// <summary>
        ///     Test Advanced Search with no entry
        /// </summary>
        [Test]
        public void TestAdvancedSearchNoEntry() {
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADVANCED_SEARCH,
                              "/Search/Advanced");

            // Click on the Search button and verify that we're on the same page
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Verify an error message shows up and the contents of the message
            IWebElement message = _driver.FindElement(By.ClassName("message"));
            StringAssert.AreEqualIgnoringCase(
                "You have not specified any search criteria.", message.Text);
        }

        /// <summary>
        ///     Test Advanced Search with a non-existant keyword
        /// </summary>
        [Test]
        public void TestAdvancedSearchNoResults() {
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADVANCED_SEARCH,
                              "/Search/Advanced");

            _driver.FindElement(By.Id("keywordString"))
                   .SendKeys(_random.Next()
                                    .ToString(CultureInfo.InvariantCulture));

            // Click on the Search button and verify that we're on the same page
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Verify an error message shows up and the contents of the message
            IWebElement message = _driver.FindElement(By.ClassName("message"));
            StringAssert.AreEqualIgnoringCase(
                "No results were found.", message.Text);
        }

        /// <summary>
        ///     Test Advanced Search with "None" entry.
        /// </summary>
        [Test]
        public void TestAdvancedSearchNone() {
            var kw1 = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            var kw2 = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            _cdc.Keywords.InsertAllOnSubmit(new List<Keyword> {kw1, kw2});
            _cdc.SubmitChanges();

            // Setup the request
            var qrc1 = new QuestionResponseContent {
                keywords = new List<string> {kw1.KeywordValue}
            };
            var rc1 = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Open
            };
            rc1.addQuestionResponse(qrc1);

            var qrc2 = new QuestionResponseContent {
                keywords = new List<string> {kw2.KeywordValue}
            };
            var rc2 = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            rc2.addQuestionResponse(qrc2);

            // Create the RequestContents
            var rmc = new RequestManagementController();
            long rid1 = rmc.create(rc1);
            long rid2 = rmc.create(rc2);

            //========================================================
            // The 2:08 train for Bug-Free Ville is now Departing
            //========================================================
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADVANCED_SEARCH,
                              "/Search/Advanced");

            _driver.FindElement(By.Id("noneKeywords"))
                   .SendKeys(kw1.KeywordValue + ", ");
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Search/Results");

            // Check Results
            ReadOnlyCollection<IWebElement> row1 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            ReadOnlyCollection<IWebElement> row2 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid2 + "']"));

            Assert.IsTrue(row1.Count == 0, "Request 1 in results!");
            Assert.IsTrue(row2.Count > 0, "Request 2 not in results!");

            //=====================================================
            // Oh no! The train crashed! :( Away goes the data!
            //=====================================================
            // Cleanup KeywordQuestion
            var cdc2 = new CAIRSDataContext();
            IQueryable<KeywordQuestion> keyq1 =
                cdc2.KeywordQuestions.Where(
                    kq => kq.KeywordID == kw1.KeywordID);
            IQueryable<KeywordQuestion> keyq2 =
                cdc2.KeywordQuestions.Where(
                    kq => kq.KeywordID == kw2.KeywordID);
            if (keyq1 == null || keyq2 == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq1);
            cdc2.KeywordQuestions.DeleteAllOnSubmit(keyq2);
            cdc2.SubmitChanges();

            // Cleanup Keyword
            Keyword kwDel1 =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw1.KeywordID);
            Keyword kwDel2 =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw2.KeywordID);
            if (kwDel1 == null || kwDel2 == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.Keywords.DeleteAllOnSubmit(new List<Keyword> {kwDel1, kwDel2});
            cdc2.SubmitChanges();

            // Cleanup QuestionResponse
            QuestionResponse qresp1 =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid1);
            QuestionResponse qresp2 =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid2);
            if (qresp1 == null || qresp2 == null) {
                Assert.Fail("QuestionResponse can't be found for Teardown!");
            }
            cdc2.QuestionResponses.DeleteAllOnSubmit(
                new List<QuestionResponse> {qresp1, qresp2});
            cdc2.SubmitChanges();

            // Cleanup Request
            Request req1 = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid1);
            Request req2 = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid2);
            if (req1 == null || req2 == null) {
                Assert.Fail("Request can't be found for Teardown!");
            }
            cdc2.Requests.DeleteAllOnSubmit(new List<Request> {req1, req2});
            cdc2.SubmitChanges();
        }

        /// <summary>
        ///     Test the quick search function with keywords
        /// </summary>
        [Test]
        public void TestQuickSearchKeywords() {
            var kw = new Keyword {
                KeywordValue = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture)
            };
            _cdc.Keywords.InsertOnSubmit(kw);
            _cdc.SubmitChanges();

            // Setup the request
            var qrc = new QuestionResponseContent {
                keywords = new List<string> {kw.KeywordValue}
            };
            var rc = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
            };
            rc.addQuestionResponse(qrc);

            // Create the RequestContent
            var rmc = new RequestManagementController();
            long rid = rmc.create(rc);

            //========================================
            // And we're ready to go!
            //========================================
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SEARCH_DIV))
                   .SendKeys(kw.KeywordValue);
            _ctm.findAndClick(Constants.UIString.ItemIDs.SEARCH_BUTTON,
                              "/Search/Search");

            // Check Results
            ReadOnlyCollection<IWebElement> row1 =
                _driver.FindElements(By.CssSelector("[data-id='" + rid + "']"));

            Assert.IsTrue(row1.Count > 0, "Request 1 not in results!");

            //========================================
            // All done! Cleanup time!
            //========================================
            // Cleanup KeywordQuestion
            var cdc2 = new CAIRSDataContext();
            KeywordQuestion keyq =
                cdc2.KeywordQuestions.FirstOrDefault(kq => kq.RequestID == rid);
            if (keyq == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.KeywordQuestions.DeleteOnSubmit(keyq);
            cdc2.SubmitChanges();

            // Cleanup Keyword
            Keyword kwDel =
                cdc2.Keywords.FirstOrDefault(k => k.KeywordID == kw.KeywordID);
            if (kwDel == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.Keywords.DeleteOnSubmit(kwDel);
            cdc2.SubmitChanges();

            // Cleanup QuestionResponse
            QuestionResponse qresp =
                cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid);
            if (qresp == null) {
                Assert.Fail("QuestionResponse can't be found for Teardown!");
            }
            cdc2.QuestionResponses.DeleteOnSubmit(qresp);
            cdc2.SubmitChanges();

            // Cleanup Request
            Request req = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid);
            if (req == null) {
                Assert.Fail("Request can't be found for Teardown!");
            }
            cdc2.Requests.DeleteOnSubmit(req);
            cdc2.SubmitChanges();
        }

        /// <summary>
        ///     Test the quick search function with Request ID
        /// </summary>
        [Test]
        public void TestQuickSearchRequestID() {
            var rc = new RequestContent {
                patientFName = "SInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
            };

            // Create the RequestContent
            var rmc = new RequestManagementController();
            long rid = rmc.create(rc);

            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SEARCH_DIV))
                   .SendKeys(rid.ToString(CultureInfo.InvariantCulture));
            _ctm.findAndClick(Constants.UIString.ItemIDs.SEARCH_BUTTON,
                              "/Request/Details/" +
                              rid.ToString(CultureInfo.InvariantCulture));

            //========================================
            // All done! Cleanup time!
            //========================================
            var cdc2 = new CAIRSDataContext();
            // Cleanup the AuditLog
            IQueryable<AuditLog> logs =
                cdc2.AuditLogs.Where(al => al.RequestID == rid);
            cdc2.AuditLogs.DeleteAllOnSubmit(logs);
            cdc2.SubmitChanges();

            // Cleanup Request
            Request req = cdc2.Requests.FirstOrDefault(r => r.RequestID == rid);
            if (req == null) {
                Assert.Fail("Request can't be found for Teardown!");
            }
            cdc2.Requests.DeleteOnSubmit(req);
            cdc2.SubmitChanges();
        }
    }
}