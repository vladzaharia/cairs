using System;
using NUnit.Framework;
using OpenQA.Selenium;
using SasquatchCAIRS;
using SasquatchCAIRS.Models.Common;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class TestReport {
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
        /// Test Report Generation without filling out fields
        /// </summary>
        [Test]
        public void TestReportBlank() {
            // Attempt to go to the Report Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.REPORTS, "/Report");

            // Click on Submit Button without selecting anything
            _ctm.findAndClick("submitBtn", "/Report");

            StringAssert.Contains("block",
                                  _driver.FindElement(By.Id("reportOptionAlert"))
                                         .GetAttribute("style"));
            StringAssert.Contains("block",
                                  _driver.FindElement(By.Id("dataTypeAlert"))
                                         .GetAttribute("style"));
            StringAssert.Contains("block",
                                  _driver.FindElement(By.Id("stratifyAlert"))
                                         .GetAttribute("style"));
        }

        /// <summary>
        /// Test Report Generation with Month by Year
        /// </summary>
        [Test]
        public void TestReportMonthPerYearInvalid() {
            // Attempt to go to the Report Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.REPORTS, "/Report");

            _ctm.findAndClick("monthPerYear-radio", "/Report");
            _ctm.findAndClick("dk_container_MPYStartYear", "/Report");
            _driver.FindElement(By.Id("dk_container_MPYStartYear"))
                   .FindElement(By.CssSelector("[data-dk-dropdown-value='2011']")).Click();
            _ctm.findAndClick("dk_container_MPYEndYear", "/Report");
            _driver.FindElement(By.Id("dk_container_MPYEndYear"))
                   .FindElement(By.CssSelector("[data-dk-dropdown-value='2010']")).Click();

            // Click on Submit Button
            _ctm.findAndClick("submitBtn", "/Report");

            StringAssert.Contains("block",
                                  _driver.FindElement(By.Id("alert2"))
                                         .GetAttribute("style"));
        }

        /// <summary>
        /// Test Report Generation with Fiscal Year
        /// </summary>
        [Test]
        public void TestReportFiscalYearInvalid() {
            // Attempt to go to the Report Page
            _driver.Navigate().GoToUrl(CommonTestingMethods.getURL());
            _ctm.findAndClick(Constants.UIString.ItemIDs.REPORTS, "/Report");

            _ctm.findAndClick("year-radio", "/Report");
            _ctm.findAndClick("dk_container_FYStartYear", "/Report");
            _driver.FindElement(By.Id("dk_container_FYStartYear"))
                   .FindElement(By.CssSelector("[data-dk-dropdown-value='2011']")).Click();
            _ctm.findAndClick("dk_container_FYEndYear", "/Report");
            _driver.FindElement(By.Id("dk_container_FYEndYear"))
                   .FindElement(By.CssSelector("[data-dk-dropdown-value='2010']")).Click();

            // Click on Submit Button
            _ctm.findAndClick("submitBtn", "/Report");

            StringAssert.Contains("block",
                                  _driver.FindElement(By.Id("alert3"))
                                         .GetAttribute("style"));
        }
    }
}
