using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace CAIRSTestProject.Integration {
    [TestFixture]
    public class LoginAndVerifyNavigation {
        IWebDriver _driver;

        [SetUp]
        public void Setup() {
            // Create a new instance of the Firefox driver
            _driver = new InternetExplorerDriver();
        }

        [TearDown]
        public void Teardown() {
            _driver.Quit();
        }

        [Test]
        public void GoogleSearch() {
            //Navigate to the site
            _driver.Navigate().GoToUrl("http://www.google.com");

            // Find the text input element by its name
            IWebElement query = _driver.FindElement(By.Name("q"));

            // Enter something to search for
            query.SendKeys("Selenium");

            // Now submit the form
            query.Submit();

            // Google's search is rendered dynamically with JavaScript.
            // Wait for the page to load, timeout after 5 seconds
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until((d) => {
                return d.Title.StartsWith("selenium");
            });

            //Check that the Title is what we are expecting
            Assert.AreEqual("selenium - Google Search", _driver.Title);
        }
    }
}
