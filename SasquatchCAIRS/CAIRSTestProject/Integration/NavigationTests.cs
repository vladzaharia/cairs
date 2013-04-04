using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS.Models;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class NavigationTests {
        private CommonTestingMethods _ctm = new CommonTestingMethods();
        private IWebDriver _driver;

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
        ///     Verifies that the Navigation Items Correctly Enforce Administrator Role
        /// </summary>
        [Test]
        public void VerifyNavAdministratorRole() {
            // Remove RequestEditor Role
            _ctm.removeRole(Constants.Roles.ADMINISTRATOR);

            //Navigate to the homepage
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            // Verify that all menu items taht should be there are there
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.DASHBOARD));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.CREATE_REQUEST));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.REPORTS));
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SEARCH_BUTTON));

            // Verify that Menu Items Aren't Shown
            _ctm.verifyItemNotShown(Constants.UIString.ItemIDs.ADMIN);

            // Add Back RequestEditor Role
            _ctm.addRole(Constants.Roles.ADMINISTRATOR);
        }

        /// <summary>
        ///     Verifies that the Navigation Items all Exist
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
        ///     Verifies that the Navigation Items Correctly Enforce ReportGenerator Role
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

        /// <summary>
        ///     Verifies that the Navigation Items Correctly Enforce RequestEditor Role
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
        ///     Verifies that the Navigation Items Correctly Enforce Viewer Role
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
        ///     Verifies that the Navigation Items all lead to the correct URLs
        /// </summary>
        [Test]
        public void VerifyNavWorks() {
            // Click on each item in the Nav Bar
            _ctm.findAndClick(Constants.UIString.ItemIDs.DASHBOARD, "/");
            _ctm.findAndClick(Constants.UIString.ItemIDs.CREATE_REQUEST,
                                   "/Request/Create");
            _ctm.findAndClick(Constants.UIString.ItemIDs.REPORTS, "/Report");
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                                   "/Admin/User/List");
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADVANCED_SEARCH,
                                   "/Search/Advanced");
        }
    }
}