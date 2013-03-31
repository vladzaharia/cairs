using System;
using System.Collections.ObjectModel;
using System.Web.Security;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Models;

namespace CAIRSTestProject.Integration {
    public class CommonTestingMethods {
        private const string DOMAIN = "sasquatch.cloudapp.net";
        private const string USERNAME = "team";
        private const string PASSWORD = "trecU5He";
        private const string ADMIN_USERNAME = "test_admin";
        private const string ADMIN_PASSWORD = "trecU5He";

        IWebDriver _driver;
        IWebDriver _adminDriver;

        /// <summary>
        /// Gets the WebDriver for this session.
        /// </summary>
        /// <returns>A WebDriver, currently Chrome</returns>
        public IWebDriver getDriver() {
            return _driver ?? (_driver = new ChromeDriver());
        }

        /// <summary>
        /// Get the URL of the app, with username and password
        /// </summary>
        /// <returns>The Full URL</returns>
        public static string getURL() {
            return "http://" + USERNAME + ":" + PASSWORD + "@" + DOMAIN;
        }

        /// <summary>
        /// Gets the Admin WebDriver for this session.
        /// </summary>
        /// <returns>An Admin WebDriver, currently Chrome</returns>
        public IWebDriver getAdminDriver() {
            return _adminDriver ?? (_adminDriver = new ChromeDriver());
        }

        /// <summary>
        /// Get the URL of the app, with username and password for admin user
        /// </summary>
        /// <returns>The Full URL</returns>
        private static string getAdminURL() {
            return "http://" + ADMIN_USERNAME + ":" + ADMIN_PASSWORD + "@" + DOMAIN;
        }

        #region Test Helpers

        /// <summary>
        /// Checks that the item is not shown.
        /// </summary>
        /// <param name="itemId">The ID of the Item</param>
        public void verifyItemNotShown(string itemId) {
            // Get a list of items, then check if list is empty
            ReadOnlyCollection<IWebElement> elements = _driver.FindElements(By.Id(itemId));
            Assert.IsEmpty(elements);
        }

        #endregion

        #region Role Helpers

        /// <summary>
        /// Add a Role to the User
        /// </summary>
        /// <param name="role"></param>
        public void addRole(string role) {
            // Go to the User Page
            goToUserPage();

            IWebElement roleBox =
                _adminDriver.FindElement(By.CssSelector("[for='userRole-" + role + "']"));

            // If it's unchecked, click it
            if (!roleBox.GetAttribute("class").Contains("checked")) {
                roleBox.FindElement(By.ClassName("icon")).Click();
            } else {
                throw new Exception("Role already assigned to user!");
            }

            // Submit the Form
            _adminDriver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON)).Click();

            // Verify that we're back at the User List Screen
            StringAssert.AreEqualIgnoringCase(getAdminURL() + "/Admin/User/List?success=True", _adminDriver.Url);
        }

        /// <summary>
        /// Remove a Role from the User
        /// </summary>
        /// <param name="role"></param>
        public void removeRole(string role) {
            // Go to the User Page
            goToUserPage();
            
            IWebElement roleBox =
                _adminDriver.FindElement(By.CssSelector("[for='userRole-" + role + "']"));

            // If it's checked, click it
            if (roleBox.GetAttribute("class").Contains("checked")) {
                roleBox.FindElement(By.ClassName("icon")).Click();
            } else {
                throw new Exception("Role already missing from user!");
            }

            // Submit the Form
            _adminDriver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON)).Click();

            // Verify that we're back at the User List Screen
            StringAssert.AreEqualIgnoringCase(getAdminURL() + "/Admin/User/List?success=True", _adminDriver.Url);
        }

        /// <summary>
        /// Go to the user management page for the user
        /// </summary>
        private void goToUserPage() {
            // Instantiate the driver and go to User Management
            getAdminDriver();
            _adminDriver.Navigate().GoToUrl(getAdminURL());
            _adminDriver.Navigate().GoToUrl(getAdminURL() + "/Admin/User/List");

            // Find and Click on Appropriate User
            _adminDriver.FindElement(By.XPath("//td[contains(.,'" + USERNAME + "')]")).Click();
        }

        #endregion
    }
}
