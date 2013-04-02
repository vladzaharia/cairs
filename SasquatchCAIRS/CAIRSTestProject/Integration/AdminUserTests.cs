using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Models;
using StringAssert = NUnit.Framework.StringAssert;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class AdminUserTests {
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
        /// Test that the role is added successfully
        /// </summary>
        [Test]
        public void AddRoleTest() {
            // Remove the Viewer Role from the User
            _ctm.removeRole(Constants.Roles.VIEWER);

            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClickOnNav(Constants.UIString.ItemIDs.ADMIN,
                                   "/Admin/User/List");

            // Find and Click on Appropriate User
            _driver.FindElement(
                By.XPath("//td[contains(.,'" + CommonTestingMethods.USERNAME +
                         "')]")).Click();

            // Check the box and submit the form
            _driver.FindElement(
                By.CssSelector("[for='userRole-" + Constants.Roles.VIEWER + "']"))
                   .Click();
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();

            // Verify that we're back at the User List Screen
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/User/List?success=True",
                _driver.Url);

            // Click on the User Link in the header and verify the role is there
            _ctm.findAndClickOnNav("username", "/Account/Manage");
            IWebElement roles = _driver.FindElement(By.Id("user-roles"));
            StringAssert.Contains(Constants.Roles.VIEWER, roles.Text);
        }
        
        /// <summary>
        /// Test that the role is properly removed from the user.
        /// </summary>
        [Test]
        public void RemoveRoleTest() {
            // Go to the Admin User Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClickOnNav(Constants.UIString.ItemIDs.ADMIN,
                                   "/Admin/User/List");

            // Find and Click on Appropriate User
            _driver.FindElement(
                By.XPath("//td[contains(.,'" + CommonTestingMethods.USERNAME +
                         "')]")).Click();

            // Check the box and submit the form
            _driver.FindElement(
                By.CssSelector("[for='userRole-" + Constants.Roles.VIEWER + "']"))
                   .Click();
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();

            // Verify that we're back at the User List Screen
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/User/List?success=True",
                _driver.Url);

            // Click on the User Link in the header and verify the role is there
            _ctm.findAndClickOnNav("username", "/Account/Manage");
            IWebElement roles = _driver.FindElement(By.Id("user-roles"));
            StringAssert.DoesNotContain(Constants.Roles.VIEWER, roles.Text);

            // Add the Viewer Role Back through the Admin Screen
            _ctm.addRole(Constants.Roles.VIEWER);
        }

        /// <summary>
        /// Test that adding a group works.
        /// </summary>
        [Test]
        public void AddGroupTest() {
            CAIRSDataContext cdc = new CAIRSDataContext();
            UserGroup ug =
                cdc.UserGroups.FirstOrDefault(group => group.Active);

            if (ug == null) {
                Assert.Fail("No User Groups in system are Active!");
            }

            // Go to the Admin User Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClickOnNav(Constants.UIString.ItemIDs.ADMIN,
                                   "/Admin/User/List");

            // Find and Click on Appropriate User
            _driver.FindElement(
                By.XPath("//td[contains(.,'" + CommonTestingMethods.USERNAME +
                         "')]")).Click();

            // Check the UG is
            IWebElement groupElement = _driver.FindElement(
               By.CssSelector("[for='userGroup-" + ug.GroupID + "']"));

            // Select the group if not already checked, else fail test
            if (!groupElement.GetAttribute("class").Contains("checked")) {
                groupElement.Click();
            } else {
                Assert.Fail("Group is already selected!");
            }

            // Submit the form
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();

            // Verify that we're back at the User List Screen
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/User/List?success=True",
                _driver.Url);

            // Click on the User Link in the header and verify the role is there
            _ctm.findAndClickOnNav("username", "/Account/Manage");
            IWebElement groups = _driver.FindElement(By.Id("user-groups"));

            StringAssert.Contains(ug.Value, groups.Text);

            // Clean up and remove the group
            _ctm.findAndClickOnNav(Constants.UIString.ItemIDs.ADMIN,
                                   "/Admin/User/List");
            _driver.FindElement(
                By.XPath("//td[contains(.,'" + CommonTestingMethods.USERNAME +
                         "')]")).Click();
            _driver.FindElement(
               By.CssSelector("[for='userGroup-" + ug.GroupID + "']")).Click();
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/User/List?success=True",
                _driver.Url);
        }

        /// <summary>
        /// Test that removing a group works.
        /// </summary>
        [Test]
        public void RemoveGroupTest() {
            CAIRSDataContext cdc = new CAIRSDataContext();
            UserGroup ug =
                cdc.UserGroups.FirstOrDefault(group => group.Active);

            if (ug == null) {
                Assert.Fail("No User Groups in system are Active!");
            }

            // Set up by adding the group
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClickOnNav(Constants.UIString.ItemIDs.ADMIN,
                                   "/Admin/User/List");
            _driver.FindElement(
                By.XPath("//td[contains(.,'" + CommonTestingMethods.USERNAME +
                         "')]")).Click();
            _driver.FindElement(
               By.CssSelector("[for='userGroup-" + ug.GroupID + "']")).Click();
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/User/List?success=True",
                _driver.Url);

            // Go to the Admin User Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClickOnNav(Constants.UIString.ItemIDs.ADMIN,
                                   "/Admin/User/List");

            // Find and Click on Appropriate User
            _driver.FindElement(
                By.XPath("//td[contains(.,'" + CommonTestingMethods.USERNAME +
                         "')]")).Click();

            // Check the UG is
            IWebElement groupElement = _driver.FindElement(
               By.CssSelector("[for='userGroup-" + ug.GroupID + "']"));

            // Select the group if not already checked, else fail test
            if (groupElement.GetAttribute("class").Contains("checked")) {
                groupElement.Click();
            } else {
                Assert.Fail("Group is already removed!");
            }

            // Submit the form
            _driver.FindElement(By.Id(Constants.UIString.ItemIDs.SUBMIT_BUTTON))
                   .Click();

            // Verify that we're back at the User List Screen
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/User/List?success=True",
                _driver.Url);

            // Click on the User Link in the header and verify the role is there
            _ctm.findAndClickOnNav("username", "/Account/Manage");
            IWebElement groups = _driver.FindElement(By.Id("user-groups"));

            StringAssert.DoesNotContain(ug.Value, groups.Text);
        }
    }
}