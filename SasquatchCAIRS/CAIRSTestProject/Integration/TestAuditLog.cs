using System;
using System.Threading;
using System.Web.Security;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Models.Common;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class TestAuditLog {
        private CommonTestingMethods _ctm = new CommonTestingMethods();
        private IWebDriver _driver;
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
        ///  Test Audit Log with Blank Values 
        /// </summary>
        [Test]
        public void TestAuditLogBlank() {
            // Attempt to go to the Audit Log Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                              "/Admin/User/List");
            _ctm.findAndClick("nav-audit",
                              "/Admin/Audit");

            // Click the Submit Button and check for an error
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Audit");
            _driver.FindElement(By.CssSelector("[for='criteriaType']"));

            // Click the User Radio, Submit Button, and check for an error
            _ctm.findAndClick("username-radio", "/Admin/Audit");
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Audit");
            _driver.FindElement(By.CssSelector("[data-valmsg-for='userName']"));
            _driver.FindElement(By.CssSelector("[data-valmsg-for='startDate']"));
            _driver.FindElement(By.CssSelector("[data-valmsg-for='endDate']"));

            // Click the Request Radio, Submit Button, and check for an error
            _ctm.findAndClick("username-radio", "/Admin/Audit");
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Audit");
            _driver.FindElement(By.CssSelector("[data-valmsg-for='requestID']"));
        }

        /// <summary>
        ///  Test Audit Log with Incorrect Username Criteria 
        /// </summary>
        [Test]
        public void TestAuditLogIncorrectUsernameCriteria() {
            // Attempt to go to the Audit Log Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                              "/Admin/User/List");
            _ctm.findAndClick("nav-audit",
                              "/Admin/Audit");

            // Click the User Radio, and add in random data
            _ctm.findAndClick("username-radio", "/Admin/Audit");
            _driver.FindElement(By.Id("userName")).SendKeys("ALInt-" + Membership.GeneratePassword(10, 0));
            _driver.FindElement(By.Id("startDate")).SendKeys("04/16/2013");
            _driver.FindElement(By.Id("endDate")).SendKeys("04/18/2013");
            _driver.FindElement(By.Id("userName")).Click();
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Audit");
            _driver.FindElement(By.CssSelector("[data-valmsg-for='userName']"));
            Thread.Sleep(500);
            // Test the start/end date
            _driver.FindElement(By.Id("startDate")).Clear();
            _driver.FindElement(By.Id("startDate")).SendKeys("04/20/2013");
            _driver.FindElement(By.Id("userName")).Click();
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Audit");
            _driver.FindElement(By.CssSelector("[data-valmsg-for='startDate']"));
        }

        /// <summary>
        ///  Test Audit Log with Incorrect Request Criteria 
        /// </summary>
        [Test]
        public void TestAuditLogIncorrectRequestCriteria() {
            // Attempt to go to the Audit Log Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                              "/Admin/User/List");
            _ctm.findAndClick("nav-audit",
                              "/Admin/Audit");

            // Click the User Radio, and add in random data
            _ctm.findAndClick("request-radio", "/Admin/Audit");
            Random r = new Random();
            _driver.FindElement(By.Id("requestID")).SendKeys(r.Next().ToString());
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Audit");
            _driver.FindElement(By.CssSelector("[data-valmsg-for='requestID']"));
            Thread.Sleep(500);
            // Test the start/end date
            _driver.FindElement(By.Id("requestID")).Clear();
            _driver.FindElement(By.Id("requestID")).SendKeys("1000-100");
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Audit");
            _driver.FindElement(By.CssSelector("[data-valmsg-for='requestID']"));
        }
    }
}
