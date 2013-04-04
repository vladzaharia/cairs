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
                KeywordValue = _random.Next().ToString(CultureInfo.InvariantCulture)
            };
            _cdc.Keywords.InsertOnSubmit(kw);
            _cdc.SubmitChanges();

            // Setup the request
            QuestionResponseContent qrc = new QuestionResponseContent {
                keywords = new List<string> { kw.KeywordValue }
            };
            RequestContent rc = new RequestContent {
                patientFName = _random.Next().ToString(CultureInfo.InvariantCulture),
            };
            rc.addQuestionResponse(qrc);

            // Create the RequestContent
            RequestManagementController rmc = new RequestManagementController();
            long rid = rmc.create(rc);

            



            // Cleanup KeywordQuestion
            KeywordQuestion keyq = _cdc.KeywordQuestions.FirstOrDefault(kq => kq.RequestID == rid);
            if (keyq == null) {
                Assert.Fail("KeywordQuestion can't be found for Teardown!");
            }
            _cdc.KeywordQuestions.DeleteOnSubmit(keyq);

            // Cleanup Keyword
            _cdc.Keywords.DeleteOnSubmit(kw);

            // Cleanup QuestionResponse
            QuestionResponse qresp = _cdc.QuestionResponses.FirstOrDefault(qr => qr.RequestID == rid);
            if (qresp == null) {
                Assert.Fail("QuestionResponse can't be found for Teardown!");
            }
            _cdc.QuestionResponses.DeleteOnSubmit(qresp);

            // Cleanup Request
            Request req = _cdc.Requests.FirstOrDefault(r => r.RequestID == rid);
            if (req == null) {
                Assert.Fail("Request can't be found for Teardown!");
            }
            _cdc.Requests.DeleteOnSubmit(req);

            _cdc.SubmitChanges();
        }
    }
}