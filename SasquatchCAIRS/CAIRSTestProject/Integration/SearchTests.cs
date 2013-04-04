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
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SEARCH_FIELD))
                .SendKeys(kw.KeywordValue);
            _ctm.findAndClick(Constants.UIString.ItemIDs.SEARCH_BUTTON, "/Search/Results");

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