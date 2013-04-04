using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Test]
        public void TestAdvancedSearchAny() {
            Keyword kw1 = new Keyword {
                KeywordValue = "SInt-" +
                    _random.Next().ToString(CultureInfo.InvariantCulture)
            };
            Keyword kw2 = new Keyword {
                KeywordValue = "SInt-" +
                    _random.Next().ToString(CultureInfo.InvariantCulture)
            };
            _cdc.Keywords.InsertAllOnSubmit(new List<Keyword> {kw1,kw2});
            _cdc.SubmitChanges();

            // Setup the request
            QuestionResponseContent qrc1 = new QuestionResponseContent {
                keywords = new List<string> { kw1.KeywordValue }
            };
            RequestContent rc1 = new RequestContent {
                patientFName = "SInt-" +
                    _random.Next().ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Open
            };
            rc1.addQuestionResponse(qrc1);

            QuestionResponseContent qrc2 = new QuestionResponseContent {
                keywords = new List<string> { kw2.KeywordValue }
            };
            RequestContent rc2 = new RequestContent {
                patientFName = "SInt-" +
                    _random.Next().ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Completed
            };
            rc2.addQuestionResponse(qrc2);

            QuestionResponseContent qrc3 = new QuestionResponseContent {
                keywords = new List<string> { kw2.KeywordValue }
            };
            RequestContent rc3 = new RequestContent {
                patientFName = "SInt-" +
                    _random.Next().ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Invalid
            };
            rc3.addQuestionResponse(qrc3);

            // Create the RequestContents
            RequestManagementController rmc = new RequestManagementController();
            long rid1 = rmc.create(rc1);
            long rid2 = rmc.create(rc2);
            long rid3 = rmc.create(rc3);

            //========================================
            // Vroom! Vroom!
            //========================================


            //========================================
            // Mr. Clean to the rescue!
            //========================================
            // Cleanup KeywordQuestion
            CAIRSDataContext cdc2 = new CAIRSDataContext();
            KeywordQuestion keyq1 = cdc2.KeywordQuestions.FirstOrDefault(kq => kq.RequestID == rid1);
            KeywordQuestion keyq2 = cdc2.KeywordQuestions.FirstOrDefault(kq => kq.RequestID == rid2);
            KeywordQuestion keyq3 = cdc2.KeywordQuestions.FirstOrDefault(kq => kq.RequestID == rid3);
            if (keyq1 == null || keyq2 == null || keyq3 == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            cdc2.KeywordQuestions.DeleteAllOnSubmit(new List<KeywordQuestion> {keyq1, keyq2, keyq3});
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
            QuestionResponse qresp1 = cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid1);
            QuestionResponse qresp2 = cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid2);
            QuestionResponse qresp3 = cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid3);
            if (qresp1 == null || qresp2 == null || qresp3 == null) {
                Assert.Fail("QuestionResponse can't be found for Teardown!");
            }
            cdc2.QuestionResponses.DeleteAllOnSubmit(new List<QuestionResponse> {qresp1, qresp2, qresp3});
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
        /// Test the quick search function
        /// </summary>
        [Test]
        public void TestQuickSearch() {
            Keyword kw = new Keyword {
                KeywordValue = "SInt-" + 
                    _random.Next().ToString(CultureInfo.InvariantCulture)
            };
            _cdc.Keywords.InsertOnSubmit(kw);
            _cdc.SubmitChanges();

            // Setup the request
            QuestionResponseContent qrc = new QuestionResponseContent {
                keywords = new List<string> { kw.KeywordValue }
            };
            RequestContent rc = new RequestContent {
                patientFName = "SInt-" + 
                    _random.Next().ToString(CultureInfo.InvariantCulture),
            };
            rc.addQuestionResponse(qrc);

            // Create the RequestContent
            RequestManagementController rmc = new RequestManagementController();
            long rid = rmc.create(rc);

            //========================================
            // And we're ready to go!
            //========================================
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SEARCH_DIV))
                .SendKeys(kw.KeywordValue);
            _ctm.findAndClick(Constants.UIString.ItemIDs.SEARCH_BUTTON, "/Search/Search");

            IWebElement requestId = _driver.FindElement(By.Id("request-id"));

            StringAssert.AreEqualIgnoringCase(
                rid.ToString(CultureInfo.InvariantCulture), 
                requestId.Text);

            //========================================
            // All done! Cleanup time!
            //========================================
            // Cleanup KeywordQuestion
            CAIRSDataContext cdc2 = new CAIRSDataContext();
            KeywordQuestion keyq = cdc2.KeywordQuestions.FirstOrDefault(kq => kq.RequestID == rid);
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
            QuestionResponse qresp = cdc2.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid);
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
    }
}