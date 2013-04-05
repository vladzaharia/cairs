using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Models.Common;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class AdminDropdownTests {
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
        ///     Tries to create a Dropdown Entry with Already-Used Fields
        /// </summary>
        [Test]
        public void CreateDropDownAlreadyExists() {
            var cdc = new CAIRSDataContext();
            Region r =
                cdc.Regions.FirstOrDefault(region => true);
            var random = new Random();
            String codeVal =
                random.Next(1000000).ToString(CultureInfo.InvariantCulture);
            String valVal =
                random.Next(100000000).ToString(CultureInfo.InvariantCulture);

            if (r == null) {
                Assert.Fail("No Regions exist in the system!");
            }

            // Navigate to the right page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                              "/Admin/User/List");
            _driver.FindElement(By.Id("nav-dropdown")).Click();

            // Go to Region List
            _driver.FindElement(By.CssSelector("[data-dropdown='Region']"))
                   .Click();

            // Go to Create Region
            _ctm.findAndClick("button-Region",
                              "/Admin/Dropdown/Create/Region");

            // Enter Already Existing Data
            _driver.FindElement(By.Id("code")).SendKeys(r.Code);
            _driver.FindElement(By.Id("value")).SendKeys(r.Value);

            // Try to Submit the Form + Verify that we're on the same page
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Dropdown/Create/Region");

            // Find the Code error and check the text
            IWebElement codeMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='code']"));
            IWebElement valMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='value']"));

            StringAssert.AreEqualIgnoringCase("That code is already in use!",
                                              codeMsg.Text);
            StringAssert.AreEqualIgnoringCase("That value is already in use!",
                                              valMsg.Text);

            // Replace the Code with a random value and recheck
            _driver.FindElement(By.Id("code")).Clear();
            _driver.FindElement(By.Id("code")).SendKeys(codeVal);
            _driver.FindElement(By.Id("value")).Clear();
            _driver.FindElement(By.Id("value")).SendKeys(r.Value);

            // Try to Submit the Form + Verify that we're on the same page
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Dropdown/Create/Region");

            valMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='value']"));
            StringAssert.AreEqualIgnoringCase("That value is already in use!",
                                              valMsg.Text);

            // Replace the Value with a random value and recheck
            _driver.FindElement(By.Id("code")).Clear();
            _driver.FindElement(By.Id("code")).SendKeys(r.Code);
            _driver.FindElement(By.Id("value")).Clear();
            _driver.FindElement(By.Id("value")).SendKeys(valVal);

            // Try to Submit the Form + Verify that we're on the same page
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Dropdown/Create/Region");

            valMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='code']"));
            StringAssert.AreEqualIgnoringCase("That code is already in use!",
                                              valMsg.Text);
        }

        /// <summary>
        ///     Tries to create a Dropdown Entry with Empty Fields
        /// </summary>
        [Test]
        public void CreateDropDownEmpty() {
            // Navigate to the right page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                              "/Admin/User/List");
            _driver.FindElement(By.Id("nav-dropdown")).Click();

            // Go to Region List
            _driver.FindElement(By.CssSelector("[data-dropdown='Region']"))
                   .Click();

            // Go to Create Region
            _driver.FindElement(By.Id("button-Region")).Click();
            StringAssert.AreEqualIgnoringCase(
                CommonTestingMethods.getURL() + "/Admin/Dropdown/Create/Region",
                _driver.Url);

            // Try to Submit the Form + Verify that we're on the same page
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Dropdown/Create/Region");

            // Find the Code error and check the text
            IWebElement codeMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='code']"));
            IWebElement valMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='value']"));

            StringAssert.AreEqualIgnoringCase("Code cannot be empty!",
                                              codeMsg.Text);
            StringAssert.AreEqualIgnoringCase("Value cannot be empty!",
                                              valMsg.Text);

            // Add text to the Code field and check just the Value Error Message
            _driver.FindElement(By.Id("code"))
                   .SendKeys(
                       (new Random()).Next(1000000)
                                     .ToString(CultureInfo.InvariantCulture));
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Dropdown/Create/Region");
            valMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='value']"));
            StringAssert.AreEqualIgnoringCase("Value cannot be empty!",
                                              valMsg.Text);

            // Add text to Valuew field, clear Code field, and just check Code Error Message
            _driver.FindElement(By.Id("code")).Clear();
            _driver.FindElement(By.Id("value"))
                   .SendKeys(
                       (new Random()).Next(1000000)
                                     .ToString(CultureInfo.InvariantCulture));
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Dropdown/Create/Region");
            codeMsg =
                _driver.FindElement(By.CssSelector("[data-valmsg-for='code']"));
            StringAssert.AreEqualIgnoringCase("Code cannot be empty!",
                                              codeMsg.Text);
        }

        /// <summary>
        ///     Test that a dropdown is successfully created.
        /// </summary>
        [Test]
        public void CreateDropDownSuccess() {
            var random = new Random();
            String codeVal =
                random.Next(1000000).ToString(CultureInfo.InvariantCulture);
            String valVal = "AdmDDInt-" +
                            random.Next(100000000)
                                  .ToString(CultureInfo.InvariantCulture);

            // Navigate to the right page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                              "/Admin/User/List");
            _driver.FindElement(By.Id("nav-dropdown")).Click();

            // Go to Region List
            _driver.FindElement(By.CssSelector("[data-dropdown='Region']"))
                   .Click();

            // Go to Create Region
            _ctm.findAndClick("button-Region",
                              "/Admin/Dropdown/Create/Region");

            // Enter Already Existing Data
            _driver.FindElement(By.Id("code")).SendKeys(codeVal);
            _driver.FindElement(By.Id("value")).SendKeys(valVal);

            // Try to Submit the Form + Verify that we're back to admin page
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Dropdown/List?success=True");

            // Verify Region Shows up in Create Request Page
            var cdc = new CAIRSDataContext();
            Region r =
                cdc.Regions.FirstOrDefault(region => region.Code == codeVal);
            if (r == null) {
                Assert.Fail("Region wasn't actually created successfully!");
            }

            _ctm.findAndClick(Constants.UIString.ItemIDs.CREATE_REQUEST,
                              "/Request/Create");
            _driver.FindElement(By.Id("regionID")).FindElement(
                By.CssSelector("[value='" + r.RegionID + "']"));

            // Clean up the DB
            cdc.Regions.DeleteOnSubmit(r);
            cdc.SubmitChanges();

            // Ensure Cleaned Up
            r = cdc.Regions.FirstOrDefault(region => region.Code == codeVal);
            Assert.IsTrue(r == null, "Region wasn't cleaned up successfully!");
        }

        /// <summary>
        ///     Test that invalidating a dropdown works
        /// </summary>
        [Test]
        public void TestInvalidateDropdown() {
            // Add the Region to the Database directly
            var cdc = new CAIRSDataContext();
            var random = new Random();
            String codeVal =
                random.Next(1000000).ToString(CultureInfo.InvariantCulture);
            String valVal = "AdmDDInt-" +
                            random.Next(100000000)
                                  .ToString(CultureInfo.InvariantCulture);

            var r = new Region {
                Code = codeVal,
                Value = valVal,
                Active = true
            };

            cdc.Regions.InsertOnSubmit(r);
            cdc.SubmitChanges();

            // Navigate to the right page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                              "/Admin/User/List");
            _driver.FindElement(By.Id("nav-dropdown")).Click();

            // Go to Region List
            _driver.FindElement(By.CssSelector("[data-dropdown='Region']"))
                   .Click();

            // Find and Click on Appropriate Region
            _driver.FindElement(
                By.XPath("//td[contains(.,'" + valVal +
                         "')]")).Click();

            // Invalidate the Region
            _driver.FindElement(By.Id("dk_container_active")).Click();
            Thread.Sleep(500);
            _driver.FindElement(By.CssSelector("[data-dk-dropdown-value=false]"))
                   .Click();

            // Try to Submit the Form + Verify that we're back to admin page
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Dropdown/List?success=True");

            // Verify Region Does not show up in Create Request Page
            _ctm.findAndClick(Constants.UIString.ItemIDs.CREATE_REQUEST,
                              "/Request/Create");
            ReadOnlyCollection<IWebElement> regionElements = _driver.FindElement
                (By.Id("regionID"))
                                                                    .FindElements
                (By.TagName("option"));

            foreach (IWebElement element in regionElements) {
                if (element.GetAttribute("value") ==
                    r.RegionID.ToString(CultureInfo.InvariantCulture)) {
                    Assert.Fail("Region appears in Create Request Form!");
                }
            }

            // Clean up the DB
            var cdc2 = new CAIRSDataContext();
            Region reg =
                cdc2.Regions.FirstOrDefault(region => region.Code == codeVal);
            if (reg != null) {
                cdc2.Regions.DeleteOnSubmit(reg);
                cdc2.SubmitChanges();
            } else {
                Assert.Fail("Region has disappeared!");
            }

            // Ensure Cleaned Up
            r = cdc2.Regions.FirstOrDefault(region => region.Code == codeVal);
            Assert.IsTrue(r == null, "Region wasn't cleaned up successfully!");
        }

        /// <summary>
        ///     Test that validating a dropdown works
        /// </summary>
        [Test]
        public void TestValidateDropdown() {
            // Add the Region to the Database directly
            var cdc = new CAIRSDataContext();
            var random = new Random();
            String codeVal =
                random.Next(1000000).ToString(CultureInfo.InvariantCulture);
            String valVal = "AdmDDInt-" +
                            random.Next(100000000)
                                  .ToString(CultureInfo.InvariantCulture);

            var r = new Region {
                Code = codeVal,
                Value = valVal,
                Active = false
            };

            cdc.Regions.InsertOnSubmit(r);
            cdc.SubmitChanges();

            // Navigate to the right page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.ADMIN,
                              "/Admin/User/List");
            _driver.FindElement(By.Id("nav-dropdown")).Click();

            // Go to Region List
            _driver.FindElement(By.CssSelector("[data-dropdown='Region']"))
                   .Click();

            // Find and Click on Appropriate Region
            _driver.FindElement(
                By.XPath("//td[contains(.,'" + valVal +
                         "')]")).Click();

            // Invalidate the Region
            _driver.FindElement(By.Id("dk_container_active")).Click();
            Thread.Sleep(500);
            _driver.FindElement(By.CssSelector("[data-dk-dropdown-value=true]"))
                   .Click();

            // Try to Submit the Form + Verify that we're back to admin page
            _ctm.findAndClick(Constants.UIString.ItemIDs.SUBMIT_BUTTON,
                              "/Admin/Dropdown/List?success=True");

            // Verify Region Shows up in Create Request Page
            _ctm.findAndClick(Constants.UIString.ItemIDs.CREATE_REQUEST,
                              "/Request/Create");
            _driver.FindElement(By.Id("regionID")).FindElement(
                By.CssSelector("[value='" + r.RegionID + "']"));

            // Clean up the DB
            var cdc2 = new CAIRSDataContext();
            Region reg =
                cdc2.Regions.FirstOrDefault(region => region.Code == codeVal);
            if (reg != null) {
                cdc2.Regions.DeleteOnSubmit(reg);
                cdc2.SubmitChanges();
            } else {
                Assert.Fail("Region has disappeared!");
            }

            // Ensure Cleaned Up
            r = cdc2.Regions.FirstOrDefault(region => region.Code == codeVal);
            Assert.IsTrue(r == null, "Region wasn't cleaned up successfully!");
        }
    }
}