// marked idempotent
namespace NACTAM.UITests {
	public class PdfExportUITest : BaseUITest {
		public PdfExportUITest() : base(true) { }

		/// <summary>
		/// Checks the pdf export for the user. Has to add one buy and one sell transaction additionally.
		/// Mervan Kilic, Tuan Bui
		/// </summary>
		[Fact]
		public void TestPdfExportForUser() {
			string dat = (new DateTime(2022, 09, 09)).ToString();
			GoToLogin();
			LoginUser("User4");

			Thread.Sleep(100);
			_webDriver.FindElement(By.Id("transactions")).Click();

			Thread.Sleep(100);
			_webDriver.FindElement(By.CssSelector(".btn-warning")).Click();

			Thread.Sleep(100);
			((IJavaScriptExecutor)_webDriver).ExecuteScript("document.querySelector('input[type=datetime-local]').type='datetime';");
			_webDriver.FindElement(By.Id("date-input")).SendKeys(dat);
			_webDriver.FindElement(By.CssSelector(".select2-selection.select2-selection--single")).Click();
			_webDriver.FindElement(By.CssSelector(".select2-selection.select2-selection--single")).SendKeys("b2m");
			_webDriver.FindElement(By.CssSelector(".select2-selection.select2-selection--single")).SendKeys(Keys.Enter);
			_webDriver.FindElement(By.Id("Amount")).SendKeys("10");
			_webDriver.FindElement(By.Id("calculated")).SendKeys("2");
			{
				var dropdown = _webDriver.FindElement(By.Id("Type"));
				dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
			}
			_webDriver.FindElement(By.Id("calculated")).SendKeys("2");
			_webDriver.FindElement(By.Id("Fee")).SendKeys("1");
			_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();

			Thread.Sleep(1000);
			_webDriver.FindElement(By.CssSelector(".btn-warning")).Click();

			Thread.Sleep(100);
			_webDriver.FindElement(By.Id("date-input")).SendKeys("09122022\t1212");
			_webDriver.FindElement(By.CssSelector(".select2-selection.select2-selection--single")).Click();
			_webDriver.FindElement(By.CssSelector(".select2-selection.select2-selection--single")).SendKeys("b2m");
			_webDriver.FindElement(By.CssSelector(".select2-selection.select2-selection--single")).SendKeys(Keys.Enter);
			_webDriver.FindElement(By.Id("Amount")).SendKeys("2");
			_webDriver.FindElement(By.Id("calculated")).SendKeys("2");
			{
				var dropdown = _webDriver.FindElement(By.Id("Type"));
				dropdown.FindElement(By.XPath("//option[. = 'Verkauf']")).Click();
			}
			_webDriver.FindElement(By.Id("calculated")).SendKeys("2");
			_webDriver.FindElement(By.Id("Fee")).SendKeys("1");
			_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();


			Thread.Sleep(100);
			_webDriver.FindElement(By.Id("services")).Click();
			_webDriver.FindElement(By.Id("tax-evaluation")).Click();

			Thread.Sleep(500);
			_webDriver.FindElement(By.Id("generate-pdf")).Click();

			CheckAndDeletePDF();

			// deleting the transactions again
			_webDriver.FindElement(By.Id("transactions")).Click();
			_webDriver.FindElement(By.CssSelector("tr.odd:nth-child(1) > td:nth-child(8) > div:nth-child(1) > div:nth-child(2) > button:nth-child(1)")).Click();
			Thread.Sleep(100);
			((IJavaScriptExecutor)_webDriver).ExecuteScript("document.getElementById('deleteButton').click()");
			Thread.Sleep(100);
			_webDriver.FindElement(By.CssSelector("tr.odd:nth-child(1) > td:nth-child(8) > div:nth-child(1) > div:nth-child(2) > button:nth-child(1)")).Click();
			Thread.Sleep(100);
			((IJavaScriptExecutor)_webDriver).ExecuteScript("document.getElementById('deleteButton').click()");
		}
	}
}
