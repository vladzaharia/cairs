using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Security;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Service;

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

            // First Name
            _driver.FindElement(By.Id("requestorFirstName"))
                   .SendKeys("CrInt-" + Membership.GeneratePassword(128, 0));
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Last Name
            _driver.FindElement(By.Id("requestorFirstName")).Clear();
            _driver.FindElement(By.Id("requestorLastName"))
                   .SendKeys("CrInt-" + Membership.GeneratePassword(128, 0));
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Email
            _driver.FindElement(By.Id("requestorLastName")).Clear();
            _driver.FindElement(By.Id("requestorEmail"))
                   .SendKeys("CrInt-" + Membership.GeneratePassword(128, 0));
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Phone Number
            _driver.FindElement(By.Id("requestorEmail")).Clear();
            _driver.FindElement(By.Id("requestorPhoneNum"))
                   .SendKeys("CrInt-" + Membership.GeneratePassword(128, 0));
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Phone Extension
            _driver.FindElement(By.Id("requestorPhoneNum")).Clear();
            _driver.FindElement(By.Id("requestorPhoneExt"))
                   .SendKeys("CrInt-" + Membership.GeneratePassword(128, 0));
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Patient First Name
            _driver.FindElement(By.Id("requestorPhoneExt")).Clear();
            _driver.FindElement(By.Id("patientFName"))
                   .SendKeys("CrInt-" + Membership.GeneratePassword(128, 0));
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Patient Last Name
            _driver.FindElement(By.Id("patientFName")).Clear();
            _driver.FindElement(By.Id("patientLName"))
                   .SendKeys("CrInt-" + Membership.GeneratePassword(128, 0));
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Agency ID
            _driver.FindElement(By.Id("patientLName")).Clear();
            _driver.FindElement(By.Id("patientAgencyID"))
                   .SendKeys("CrInt-" + Membership.GeneratePassword(128, 0));
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Reference
            _driver.FindElement(By.ClassName("patientAgencyID")).Clear();
            _driver.FindElement(By.ClassName("reference"))
                   .SendKeys("CrInt-" + Membership.GeneratePassword(128, 0));
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));
        }

        /// <summary>
        /// Test Create Request Validation
        /// </summary>
        [Test]
        public void TestCreateRequestValidation() {
            // Attempt to go to the Create Request Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.CREATE_REQUEST,
                              "/Request/Create");

            // Patient Age
            _driver.FindElement(By.Id("patientAge"))
                   .SendKeys("256");
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Time Spent
            _driver.FindElement(By.Id("patientAge")).Clear();
            _driver.FindElement(By.ClassName("time-spent"))
                   .SendKeys("32768");
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));

            // Email
            _driver.FindElement(By.ClassName("time-spent")).Clear();
            _driver.FindElement(By.Id("requestorEmail"))
                   .SendKeys("abcd");
            _ctm.findAndClick("save_draft", "/Request/Create");
            _driver.FindElement(By.ClassName("validation-summary-errors"));
        }

        /// <summary>
        /// Test Create Request Normally
        /// </summary>
        [Test]
        public void TestCreateRequestValid() {
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

            // Attempt to go to the Create Request Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.CREATE_REQUEST,
                              "/Request/Create");

            String rFName = "CrInt-" + Membership.GeneratePassword(10, 0);
            String rLName = "CrInt-" + Membership.GeneratePassword(10, 0);
            String rEmail = "abcd@example.com";
            String rPhone = "CrInt-" + Membership.GeneratePassword(10, 0);
            String rExt = "CrInt-" + Membership.GeneratePassword(10, 0);
            String pFName = "CrInt-" + Membership.GeneratePassword(10, 0);
            String pLName = "CrInt-" + Membership.GeneratePassword(10, 0);
            String pId = "CrInt-" + Membership.GeneratePassword(10, 0);
            String pAge = "123";
            String timeSpent = "1234";
            String parentRequest = rid1.ToString();
            String reference = "CrInt-" + Membership.GeneratePassword(10, 0);

            _driver.FindElement(By.Id("requestorFirstName"))
                   .SendKeys(rFName);
            _driver.FindElement(By.Id("requestorLastName"))
                   .SendKeys(rLName);
            _driver.FindElement(By.Id("requestorEmail"))
                   .SendKeys(rEmail);
            _driver.FindElement(By.Id("requestorPhoneNum"))
                   .SendKeys(rPhone);
            _driver.FindElement(By.Id("requestorPhoneExt"))
                   .SendKeys(rExt);
            _driver.FindElement(By.Id("patientFName"))
                   .SendKeys(pFName);
            _driver.FindElement(By.Id("patientLName"))
                   .SendKeys(pLName);
            _driver.FindElement(By.Id("patientAgencyID"))
                   .SendKeys(pId);
            _driver.FindElement(By.Id("patientAge"))
                   .SendKeys(pAge);
            _driver.FindElement(By.ClassName("time-Spent"))
                   .SendKeys(timeSpent);
            _driver.FindElement(By.Id("parentRequestId"))
                   .SendKeys(parentRequest);
            _driver.FindElement(By.ClassName("reference"))
                   .SendKeys(reference);

            // Submit the Form
            _driver.FindElement(By.Id("save_draft"));
            StringAssert.Contains("/Request/Details", _driver.Url);

            long newRid =
                Convert.ToInt64(
                    Regex.Match(_driver.Url, ".*\\/([0-9]+)").Groups[0]);

            Request newReq =
                _cdc.Requests.FirstOrDefault(r => r.RequestID == newRid);
            if (newReq == null) {
                Assert.Fail("Can't find newly-created Request.");
            }

            StringAssert.AreEqualIgnoringCase(rFName, newReq.RequestorFName);
            StringAssert.AreEqualIgnoringCase(rLName, newReq.RequestorLName);
            StringAssert.AreEqualIgnoringCase(rEmail, newReq.RequestorEmail);
            StringAssert.AreEqualIgnoringCase(rPhone, newReq.RequestorPhone);
            StringAssert.AreEqualIgnoringCase(rExt, newReq.RequestorPhoneExt);
            StringAssert.AreEqualIgnoringCase(pFName, newReq.PatientFName);
        }
    }
}
