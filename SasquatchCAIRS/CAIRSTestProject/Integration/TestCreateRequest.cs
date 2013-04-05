using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Models.Common;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class TestCreateRequest {
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
        /// Test Create Request without Request Editor Role
        /// </summary>
        [Test]
        public void TestCreateRequestNoEditor() {
            // Remove the Viewer Role from the User
            _ctm.removeRole(Constants.Roles.REQUEST_EDITOR);

            // Attempt to go to the Create Request Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL() + "/Request/Create");

            // Assert that we're redirected to the not authorized page
            StringAssert.Contains("/Account/Auth", _driver.Url);

            // Cleanup
           _ctm.addRole(Constants.Roles.REQUEST_EDITOR);
        }

        /// <summary>
        /// Test Create Request field limits
        /// </summary>
        [Test]
        public void TestCreateRequestLimit() {
            // Attempt to go to the Create Request Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.CREATE_REQUEST,
                              "/Request/Create");

        }

        /// <summary>
        /// Test Create Request Validation
        /// </summary>
        [Test]
        public void TestCreateRequestValidation() {
        }

        /// <summary>
        /// Test Create Request Normally
        /// </summary>
        [Test]
        public void TestCreateRequestValid() {
        }
    }
}
