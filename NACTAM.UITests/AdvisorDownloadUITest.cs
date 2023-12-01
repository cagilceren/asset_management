// marked as idempotent
namespace NACTAM.UITests {
	/// <summary>
	/// UI Test for download PDF from Advisor
	///
	/// author: Thuer Niklas, Tuan Bui
	/// </summary>
	[Collection("Sequential")]
	public class AdvisorDownloadUITest : BaseUITest {
		public AdvisorDownloadUITest() : base(true) { }

		[Fact]
		public void DownloadPDF() {
			GoToLogin();
			LoginUser("taxadvisor3");

			_webDriver.FindElement(By.CssSelector(".nav-item:nth-child(5) > .nav-link")).Click();
			_webDriver.FindElement(By.CssSelector("tr:nth-child(8) .row .btn")).Click();
			_webDriver.FindElement(By.CssSelector("#accordion-sidebar > .nav-item:nth-child(4) > .nav-link")).Click();
			_webDriver.FindElement(By.CssSelector(".btn-danger")).Click();


			SwitchToUser("user3");

			// Find and Accept Insight
			_webDriver.FindElement(By.CssSelector(".nav-item:nth-child(9) > .nav-link")).Click();
			Thread.Sleep(100);
			_webDriver.FindElement(By.CssSelector("#collapse-settings .collapse-item:nth-child(2) > span")).Click();
			_webDriver.FindElement(By.ClassName("btn-warning")).Click();
			_webDriver.FindElement(By.CssSelector(".btn-success")).Click();

			SwitchToUser("taxadvisor3");

			// Klick Download Button
			_webDriver.FindElement(By.CssSelector("button.btn:nth-child(2)")).Click();

			// Check ob download geschehen
			CheckAndDeletePDF();

			_webDriver.FindElement(By.CssSelector(".nav-item:nth-child(5) > .nav-link")).Click();
			_webDriver.FindElement(By.CssSelector(".btn-danger")).Click();

		}

		[Fact]
		public void DownloadPDFMissingPermission() {
			GoToLogin();
			LoginUser("taxadvisor3");

			_webDriver.FindElement(By.CssSelector(".nav-item:nth-child(5) > .nav-link")).Click();
			_webDriver.FindElement(By.CssSelector("tr:nth-child(8) .row .btn")).Click();
			_webDriver.FindElement(By.CssSelector("#accordion-sidebar > .nav-item:nth-child(4) > .nav-link")).Click();
			_webDriver.FindElement(By.CssSelector(".btn-danger")).Click();
			Assert.Throws<NoSuchElementException>(() => {
				_webDriver.FindElement(By.CssSelector("button.btn:nth-child(2)"));
			});

			// Check ob download geschehen
			CheckNoPDF();

			_webDriver.FindElement(By.CssSelector(".nav-item:nth-child(5) > .nav-link")).Click();
			_webDriver.FindElement(By.CssSelector(".btn-danger")).Click();

		}
	}
}
