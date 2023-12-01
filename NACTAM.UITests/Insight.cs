// marked as idempotent

namespace NACTAM.UITests {

	/// <summary>
	/// UI Test for Insight requests
	///
	/// author: Tuan Bui
	/// </summary>
	public class InsightTest : BaseUITest {
		public InsightTest() : base(true) { }

		/// <summary>
		/// Login Test
		/// </summary>
		[Fact]
		void LoginTest() {
			GoToLogin();
			LoginExampleTaxAdvisor();
			_webDriver.FindElement(By.LinkText("Zuordnung")).Click();
		}

		/// <summary>
		/// UI Test for successful insightrequest as well as extended insight request
		/// </summary>
		[Fact]
		public void InsightRequestTest() {
			// login as TaxAdvisor
			GoToLogin();
			LoginExampleTaxAdvisor();

			// Access PrivatePerson
			_webDriver.FindElement(By.LinkText("Zuordnung")).Click();
			_webDriver.FindElement(By.CssSelector("tr:nth-child(8) .row .btn")).Click();
			_webDriver.FindElement(By.CssSelector(".nav-item:nth-child(4) span:nth-child(2)")).Click();
			// Requesting Insight
			_webDriver.FindElement(By.CssSelector("tr:nth-child(2) .btn")).Click();
			Logout();

			// Login as Private Person
			LoginExamplePerson();
			_webDriver.FindElement(By.CssSelector("#notification-count")).Click(); // Open Notifications
			Assert.Equal("Domnique Gregori fragt nach einer einfachen Einsicht.", _webDriver.FindElement(By.CssSelector(".dropdown-item:nth-child(1)  span:nth-child(2)")).GetAttribute("innerText"));

			_webDriver.FindElement(By.LinkText("Einstellungen")).Click();
			Thread.Sleep(50);
			_webDriver.FindElement(By.CssSelector("#collapse-settings .collapse-item:nth-child(2) > span")).Click();
			_webDriver.FindElement(By.CssSelector(".btn-warning")).Click();
			_webDriver.FindElement(By.CssSelector(".btn-success")).Click();
			Logout();
			LoginExampleTaxAdvisor();
			_webDriver.FindElement(By.CssSelector(".btn-group > .btn.open-simpleInsight"));
			_webDriver.FindElement(By.CssSelector(".btn-warning")).Click();


			Logout();
			LoginExamplePerson();
			_webDriver.FindElement(By.CssSelector("#notification-count")).Click(); // Open Notifications
			Assert.Equal("Domnique Gregori fragt nach einer erweiterten Einsicht.", _webDriver.FindElement(By.CssSelector(".dropdown-item:nth-child(1) span:nth-child(2)")).GetAttribute("innerText"));

			_webDriver.FindElement(By.LinkText("Einstellungen")).Click();
			Thread.Sleep(50);
			_webDriver.FindElement(By.CssSelector("#collapse-settings .collapse-item:nth-child(2) > span")).Click();
			_webDriver.FindElement(By.CssSelector(".btn-warning")).Click();
			_webDriver.FindElement(By.CssSelector(".btn-success")).Click();
			Logout();
			LoginExampleTaxAdvisor();

			_webDriver.FindElement(By.CssSelector(".btn-success.extendedInsight")).Click();
			_webDriver.Navigate().GoToUrl("http://localhost:5101/Advisor/MyUsers");
			_webDriver.FindElement(By.LinkText("Zuordnung")).Click();
			_webDriver.FindElement(By.CssSelector("tr:nth-child(2) .row .btn")).Click();
			_webDriver.FindElement(By.CssSelector(".nav-item:nth-child(4) span:nth-child(2)")).Click();
		}
	}
}
