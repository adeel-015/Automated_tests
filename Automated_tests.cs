using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CloudQA.InternshipTask
{
    [TestFixture]
    public class AutomationPracticeTests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private const string TargetUrl = "https://app.cloudqa.io/home/AutomationPracticeForm";

        [SetUp]
        public void Setup()
        {
            // Initialize Chrome Driver
            // Ensure you have the ChromeDriver executable in your path or managed via NuGet
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void FillFormResiliently()
        {
            // 1. Visit the web page
            _driver.Navigate().GoToUrl(TargetUrl);

            // 2. Interact with three specific fields using robust locators

            // 30 seconds to manually break the page
            // Thread.Sleep(30000);

            // Field 1: First Name
            // Strategy: Find label with text 'First Name', find the nearest input following it
            FillTextInputByLabel("First Name", "John");

            // Field 2: Mobile Number
            // Strategy: Find label with text 'Mobile Number', find the nearest input following it
            FillTextInputByLabel("Mobile #", "9876543210");

            // Field 3: Gender (Radio Button)
            // Strategy: Find the specific option text (e.g., "Male") and click it directly. 
            // This is often more robust than finding the 'Gender' group and looking inside.
            SelectRadioOrCheckboxByLabel("Male");

            // Optional: Add a small pause or assertion here to verify visually if running locally
            Thread.Sleep(2000);

            Console.WriteLine("Test passed: 3 fields populated successfully despite potential attribute changes.");
        }

        [TearDown]
        public void Teardown()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }

        // ---------------------------------------------------------
        // ROBUST HELPER METHODS
        // ---------------------------------------------------------

        /// <summary>
        /// Finds a text input relative to its visible Label text.
        /// This ignores ID, Name, and Class attributes, making it resilient to code refactors.
        /// </summary>
        private void FillTextInputByLabel(string labelText, string valueToType)
        {
            // XPath Explanation:
            // 1. //label[contains(normalize-space(), '{labelText}')] 
            //    Finds a label containing the specific text (ignoring extra whitespace).
            // 2. /following::input[1]
            //    Locates the very first input tag that appears in the DOM after that label.
            //    This works for both nested inputs (<label>Text <input/></label>) 
            //    and adjacent inputs (<label>Text</label><input/>).
            string xpath = $"//label[contains(normalize-space(), '{labelText}')]/following::input[1]";

            try
            {
                IWebElement inputElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                inputElement.Clear();
                inputElement.SendKeys(valueToType);
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail($"Could not find input field associated with label: '{labelText}'");
            }
        }

        /// <summary>
        /// Selects a Radio button or Checkbox by its visible text.
        /// Clicks the Label itself if possible (standard practice for accessible forms) 
        /// or the input immediately preceding/following it.
        /// </summary>
        private void SelectRadioOrCheckboxByLabel(string optionText)
        {
            // Strategy:
            // Often clicking the text (Label) next to a radio button selects the radio button.
            // This is safer than trying to click the small <input> circle which might be obscured or styled out of view.
            string labelXpath = $"//label[contains(normalize-space(), '{optionText}')]";

            try
            {
                IWebElement elementToClick = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(labelXpath)));
                elementToClick.Click();
            }
            catch (WebDriverTimeoutException)
            {
                // Fallback: If clicking the label doesn't work, try to find the input relative to the text
                // Look for an input preceding the text (common in radios: (O) Text)
                string fallbackXpath = $"//*[contains(text(),'{optionText}')]//preceding-sibling::input[@type='radio' or @type='checkbox'][1]";
                var inputElement = _driver.FindElement(By.XPath(fallbackXpath));
                inputElement.Click();
            }
        }
    }
}