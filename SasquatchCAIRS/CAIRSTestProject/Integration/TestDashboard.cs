using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Service;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class TestDashboard {
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
        ///     Test Dashboard without Request Editor Role
        /// </summary>
        [Test]
        public void TestDashboardNoRequestEditor() {
            // Create a test request in the DB
            var rc1 = new RequestContent {
                patientFName = "DInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Completed,
                timeOpened = DateTime.Now
            };
            var rc2 = new RequestContent {
                patientFName = "DInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Open,
                timeOpened = DateTime.Now
            };
            var rmc = new RequestManagementController();
            long rid1 = rmc.create(rc1);
            long rid2 = rmc.create(rc2);

            _ctm.removeRole(Constants.Roles.REQUEST_EDITOR);
            _ctm.removeRole(Constants.Roles.ADMINISTRATOR);

            // Go to the Dashboard
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            ReadOnlyCollection<IWebElement> elements =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            Assert.Greater(elements.Count, 0);

            elements =
                _driver.FindElements(By.CssSelector("[data-id='" + rid2 + "']"));
            Assert.AreEqual(0, elements.Count);

            elements = _driver.FindElements(By.ClassName("add-button"));
            Assert.AreEqual(0, elements.Count);

            // Add the roles back
            _ctm.addRole(Constants.Roles.ADMINISTRATOR);
            _ctm.addRole(Constants.Roles.REQUEST_EDITOR);

            Request rq1 = _cdc.Requests.FirstOrDefault(r => r.RequestID == rid1);
            Request rq2 = _cdc.Requests.FirstOrDefault(r => r.RequestID == rid2);
            if (rq1 == null || rq2 == null) {
                Assert.Fail("Request is null");
            }
            _cdc.Requests.DeleteOnSubmit(rq1);
            _cdc.Requests.DeleteOnSubmit(rq2);
            _cdc.SubmitChanges();
        }

        /// <summary>
        ///     Test the Dashboard with no Viewer Role
        /// </summary>
        [Test]
        public void TestDashboardNoViewer() {
            // Remove the Viewer Role from the User
            _ctm.removeRole(Constants.Roles.VIEWER);

            // Go to the Dashboard
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            IWebElement element = _driver.FindElement(By.Id("error-message"));
            StringAssert.AreEqualIgnoringCase("Nothing to see here!",
                                              element.Text);

            _driver.FindElement(By.ClassName("add-button"));

            _ctm.removeRole(Constants.Roles.REQUEST_EDITOR);

            // Go to the Dashboard again
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            element = _driver.FindElement(By.Id("error-message"));
            StringAssert.AreEqualIgnoringCase("Nothing to see here!",
                                              element.Text);

            ReadOnlyCollection<IWebElement> elements =
                _driver.FindElements(By.ClassName("add-button"));
            Assert.AreEqual(0, elements.Count);

            // Add the roles back
            _ctm.addRole(Constants.Roles.VIEWER);
            _ctm.addRole(Constants.Roles.REQUEST_EDITOR);
        }

        /// <summary>
        ///     Test Dashboard normally
        /// </summary>
        [Test]
        public void TestDashboardWorking() {
            // Create a test request in the DB
            var rc1 = new RequestContent {
                patientFName = "DInt-" +
                               _random.Next()
                                      .ToString(CultureInfo.InvariantCulture),
                requestStatus = Constants.RequestStatus.Open,
                timeOpened = DateTime.Now
            };
            var rmc = new RequestManagementController();
            long rid1 = rmc.create(rc1);

            _ctm.removeRole(Constants.Roles.REQUEST_EDITOR);

            // Go to the Dashboard
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());

            ReadOnlyCollection<IWebElement> elements =
                _driver.FindElements(By.CssSelector("[data-id='" + rid1 + "']"));
            Assert.Greater(elements.Count, 0);

            elements = _driver.FindElements(By.ClassName("add-button"));
            Assert.AreEqual(0, elements.Count);

            // Add the roles back
            _ctm.addRole(Constants.Roles.REQUEST_EDITOR);

            Request rq1 = _cdc.Requests.FirstOrDefault(r => r.RequestID == rid1);
            if (rq1 == null) {
                Assert.Fail("Request is null");
            }
            _cdc.Requests.DeleteOnSubmit(rq1);
            _cdc.SubmitChanges();
        }
    }
}