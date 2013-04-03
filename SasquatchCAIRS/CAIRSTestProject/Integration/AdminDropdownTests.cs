using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Models;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class AdminDropdownTests {
        private CommonTestingMethods _ctm = new CommonTestingMethods();
        private IWebDriver _driver;

        [TestFixtureSetUp]
        public void Setup() {
            _driver = _ctm.getDriver();
        }

        [TestFixtureTearDown]
        public void Teardown() {
            _driver.Quit();
            _ctm.getAdminDriver().Quit();
        }

        /// <summary>
        /// Tries to create a Dropdown Entry with Empty Fields
        /// </summary>
        [Test]
        public void CreateDropDownEmpty() {
            // Navigate to the right page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                                   "/Admin/User/List");
            _driver.FindElement(By.Id("nav-dropdown")).Click();

            // Go to Region List
            _driver.FindElement(By.CssSelector("[data-dropdown='Region']")).Click();

            // Go to Create Region
            _driver.FindElement(By.Id("button-Region")).Click();
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/Dropdown/Create/Region",
                _driver.Url);

            // Try to Submit the Form + Verify that we're on the same page
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/Dropdown/Create/Region",
                _driver.Url);

            // Find the Code error and check the text
            IWebElement codeMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='code']"));
            IWebElement valMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='value']"));

            StringAssert.AreEqualIgnoringCase("Code cannot be empty!", codeMsg.Text);
            StringAssert.AreEqualIgnoringCase("Value cannot be empty!", valMsg.Text);

            // Add text to the Code field and check just the Value Error Message
            _driver.FindElement(By.Id("code"))
                   .SendKeys(
                       (new Random()).Next(1000000)
                                     .ToString(CultureInfo.InvariantCulture));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/Dropdown/Create/Region",
                _driver.Url);
            valMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='value']"));
            StringAssert.AreEqualIgnoringCase("Value cannot be empty!", valMsg.Text);

            // Add text to Valuew field, clear Code field, and just check Code Error Message
            _driver.FindElement(By.Id("code")).Clear();
            _driver.FindElement(By.Id("value"))
                  .SendKeys(
                      (new Random()).Next(1000000)
                                    .ToString(CultureInfo.InvariantCulture));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/Dropdown/Create/Region",
                _driver.Url);
            codeMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='code']"));
            StringAssert.AreEqualIgnoringCase("Code cannot be empty!", codeMsg.Text);
        }

        [Test]
        public void CreateDropDownAlreadyExists() {
            CAIRSDataContext cdc = new CAIRSDataContext();
            Region r =
                cdc.Regions.FirstOrDefault(region => true);
            Random random = new Random();
            String codeVal = random.Next(1000000).ToString();
            String valVal = random.Next(100000000000).ToString();

            if (r == null) {
                Assert.Fail("No Regions exist in the system!");
            }

            // Navigate to the right page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                                   "/Admin/User/List");
            _driver.FindElement(By.Id("nav-dropdown")).Click();

            // Go to Region List
            _driver.FindElement(By.CssSelector("[data-dropdown='Region']")).Click();

            // Go to Create Region
            _driver.FindElement(By.Id("button-Region")).Click();
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/Dropdown/Create/Region",
                _driver.Url);

            // Enter Already Existing Data
            _driver.FindElement(By.Id("code")).SendKeys(r.Code);
            _driver.FindElement(By.Id("value")).SendKeys(r.Value);

            // Try to Submit the Form + Verify that we're on the same page
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/Dropdown/Create/Region",
                _driver.Url);

            // Find the Code error and check the text
            IWebElement codeMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='code']"));
            IWebElement valMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='value']"));

            StringAssert.AreEqualIgnoringCase("Code cannot be empty!", codeMsg.Text);
            StringAssert.AreEqualIgnoringCase("Value cannot be empty!", valMsg.Text);
        }
    }
}
