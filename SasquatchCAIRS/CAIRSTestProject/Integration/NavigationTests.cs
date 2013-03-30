using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
using SasquatchCAIRS.Models;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class NavigationTests {
        CommonTestingMethods _ctm = new CommonTestingMethods();
        IWebDriver _driver;

        [TestFixtureSetUp]
        public void Setup() {
            _driver = _ctm.getDriver();
        }

        [TestFixtureTearDown]
        public void Teardown() {
            _driver.Quit();
        }

        /// <summary>
        /// Verifies that the Navigation Items all Exist
        /// </summary>
        [Test]
        public void VerifyNavExists() {
            //Navigate to the site
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            // Verify that all menu items are there
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.DASHBOARD));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.CREATE_REQUEST));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.REPORTS));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.ADMIN));

            // Verify all search menubar items are there
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SEARCH_BUTTON));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.ADVANCED_SEARCH));
        }

        /// <summary>
        /// Verifies that the Navigation Items all lead to the correct URLs
        /// </summary>
        [Test]
        public void VerifyNavWorks() {
            // Click on each item in the Nav Bar
            findAndClickOnNav(Constants.UIString.ItemIDs.DASHBOARD, "/");
            findAndClickOnNav(Constants.UIString.ItemIDs.CREATE_REQUEST, "/Request/Create");
            findAndClickOnNav(Constants.UIString.ItemIDs.REPORTS, "/Report");
            findAndClickOnNav(Constants.UIString.ItemIDs.ADMIN, "/Admin/User/List");
            findAndClickOnNav(Constants.UIString.ItemIDs.ADVANCED_SEARCH, "/Search/Advanced");
        }

        /// <summary>
        /// Verifies that the Navigation Items Correctly Enforce Viewer Role
        /// </summary>
        [Test]
        public void VerifyNavViewerRole() {
            // Remove Viewer Role
            _ctm.removeRole(Constants.Roles.VIEWER);

            //Navigate to the homepage
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            // Verify that all menu items are there
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.DASHBOARD));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.CREATE_REQUEST));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.REPORTS));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.ADMIN));

            // Verify that Menu Items Aren't Shown
            _ctm.verifyItemNotShown(Constants.UIString.ItemIDs.SEARCH_BUTTON);

            // Add Back Viewer Role
            _ctm.addRole(Constants.Roles.VIEWER);
        }

        /// <summary>
        /// Verifies that the Navigation Items Correctly Enforce RequestEditor Role
        /// </summary>
        [Test]
        public void VerifyNavRequestEditorRole() {
            // Remove RequestEditor Role
            _ctm.removeRole(Constants.Roles.REQUEST_EDITOR);

            //Navigate to the homepage
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            // Verify that all menu items taht should be there are there
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.DASHBOARD));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.REPORTS));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.ADMIN));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SEARCH_BUTTON));

            // Verify that Menu Items Aren't Shown
            _ctm.verifyItemNotShown(Constants.UIString.ItemIDs.CREATE_REQUEST);

            // Add Back RequestEditor Role
            _ctm.addRole(Constants.Roles.REQUEST_EDITOR);
        }

        /// <summary>
        /// Verifies that the Navigation Items Correctly Enforce ReportGenerator Role
        /// </summary>
        [Test]
        public void VerifyNavReportGeneratorRole() {
            // Remove RequestEditor Role
            _ctm.removeRole(Constants.Roles.REPORT_GENERATOR);

            //Navigate to the homepage
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            // Verify that all menu items taht should be there are there
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.DASHBOARD));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.CREATE_REQUEST));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.ADMIN));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SEARCH_BUTTON));

            // Verify that Menu Items Aren't Shown
            _ctm.verifyItemNotShown(Constants.UIString.ItemIDs.REPORTS);

            // Add Back RequestEditor Role
            _ctm.addRole(Constants.Roles.REPORT_GENERATOR);
        }

        #region Helpers
        /// <summary>
        /// Finds a Nav Element and clicks on it, then asserts the URL matches
        /// </summary>
        /// <param name="id">ID of Nav Element</param>
        /// <param name="expectedPath">Path Expected</param>
        private void findAndClickOnNav(string id, string expectedPath) {
            // Go Home and try this
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            // Find Item and Click
            IWebElement navItem = _driver.FindElement(By.Id(id));
            navItem.Click();

            // Check URL
            StringAssert.AreEqualIgnoringCase(CommonTestingMethods.getURL() + expectedPath, _driver.Url);
        }
        #endregion
    }
}
