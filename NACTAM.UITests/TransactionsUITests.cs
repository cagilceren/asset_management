// marked as idempotent

using System.Globalization;

namespace NACTAM.UITests;

public class TransactionsUiTests : BaseUITest {
	// German CultureInfo
	CultureInfo _germanInfo = new CultureInfo("de-DE");
	public TransactionsUiTests() : base(true) {
		GoToLogin();
		LoginUser("user1");
	}

	[Fact]
	public void EmptyTransaction() {
		_webDriver.FindElement(By.Id("transactions")).Click();
		_webDriver.FindElement(By.CssSelector(".btn-warning")).Click();
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();
		Assert.Equal(UrlWith("/CreateTransaction"), _webDriver.Url);
	}



	[Fact]
	public void CorrectTransactionAutomaticDate() {

		decimal randomDecimal = 192.3M;
		decimal randomDecimal2 = 32.3M;
		decimal randomDecimal3 = 384.3M;

		_webDriver.Navigate().GoToUrl(UrlWith("/Transaction/CreateTransaction"));
		_webDriver.FindElement(By.Id("btn-now")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		//_webDriver.FindElement(By.XPath("/html/body/div/div/div/main/div[1]/div/div/div/div[2]/div/form/div[4]/div/div[2]/a")).Click();
		//this is somehow necessary
		_webDriver.FindElement(By.Id("calculated")).SendKeys(randomDecimal.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal2.ToString(_germanInfo));
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal3.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();

		var totalCount = _webDriver.FindElements(By.CssSelector("tbody > tr")).Count;
		Assert.Equal(UrlWith("/Transactions"), _webDriver.Url);
		Assert.Equal(1, totalCount);
		//check if the values exist on the page
		Assert.Contains(randomDecimal.ToString(_germanInfo), _webDriver.PageSource);
		Assert.Contains(randomDecimal2.ToString(_germanInfo), _webDriver.PageSource);
		Assert.Contains(randomDecimal3.ToString(_germanInfo), _webDriver.PageSource);



		_webDriver.FindElement(By.CssSelector(".btn.delete-btn")).Click();
		Thread.Sleep(500);
		_webDriver.FindElement(By.CssSelector("#deleteForm > .btn-danger")).Click();
	}


	[Fact]
	public void SellTooMuch() {

		decimal randomDecimal = 192.3M;
		decimal randomDecimal2 = 32.3M;
		decimal randomDecimal3 = 384.3M;

		_webDriver.Navigate().GoToUrl(UrlWith("/Transaction/CreateTransaction"));
		_webDriver.FindElement(By.Id("btn-now")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		//_webDriver.FindElement(By.XPath("/html/body/div/div/div/main/div[1]/div/div/div/div[2]/div/form/div[4]/div/div[2]/a")).Click();
		//this is somehow necessary
		_webDriver.FindElement(By.Id("calculated")).SendKeys(randomDecimal.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal2.ToString(_germanInfo));
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Verkauf']")).Click();
		}
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal3.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();

		Assert.Equal(UrlWith("/CreateTransaction"), _webDriver.Url);
		//check if the values exist on the page
		Assert.Contains("nicht verkaufen", _webDriver.PageSource);



	}


	[Fact]
	public void CorrectTransactionManualDate() {
		decimal randomDecimal = 192.3M;
		decimal randomDecimal2 = 32.3M;
		decimal randomDecimal3 = 384.3M;
		string dat = DateTime.Now.ToString(_germanInfo);


		_webDriver.Navigate().GoToUrl(UrlWith("/Transaction/CreateTransaction"));
		((IJavaScriptExecutor)_webDriver).ExecuteScript("document.querySelector('input[type=datetime-local]').type='datetime';");
		_webDriver.FindElement(By.Id("date-input")).Click();
		_webDriver.FindElement(By.Id("date-input")).SendKeys(dat);
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		_webDriver.FindElement(By.Id("calculated")).SendKeys(randomDecimal.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal2.ToString(_germanInfo));
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal3.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();
		Assert.Equal(UrlWith("/Transactions"), _webDriver.Url);
		var totalCount = _webDriver.FindElements(By.CssSelector("tbody > tr")).Count;
		Assert.Equal(1, totalCount);
		//check if the values exist on the page
		Assert.Contains(randomDecimal.ToString(_germanInfo), _webDriver.PageSource);
		Assert.Contains(randomDecimal2.ToString(_germanInfo), _webDriver.PageSource);
		Assert.Contains(randomDecimal3.ToString(_germanInfo), _webDriver.PageSource);
		_webDriver.FindElement(By.CssSelector(".btn.delete-btn")).Click();
		Thread.Sleep(500);
		_webDriver.FindElement(By.CssSelector("#deleteForm > .btn-danger")).Click();
	}


	[Fact]
	public void MissingFee() {
		decimal randomDecimal = 192.3M;
		decimal randomDecimal2 = 32.3M;

		_webDriver.Navigate().GoToUrl(UrlWith("/Transaction/CreateTransaction"));
		_webDriver.FindElement(By.Id("btn-now")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		//_webDriver.FindElement(By.XPath("/html/body/div/div/div/main/div[1]/div/div/div/div[2]/div/form/div[4]/div/div[2]/a")).Click();
		//this is somehow necessary
		_webDriver.FindElement(By.Id("calculated")).SendKeys(randomDecimal.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal.ToString(randomDecimal2.ToString(_germanInfo)));
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();
		Assert.Equal(UrlWith("/CreateTransaction"), _webDriver.Url);
	}

	[Fact]
	public void MissingType() {
		decimal randomDecimal = 192.3M;
		decimal randomDecimal3 = 384.3M;

		_webDriver.Navigate().GoToUrl(UrlWith("/Transaction/CreateTransaction"));
		_webDriver.FindElement(By.Id("btn-now")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		//_webDriver.FindElement(By.XPath("/html/body/div/div/div/main/div[1]/div/div/div/div[2]/div/form/div[4]/div/div[2]/a")).Click();
		//this is somehow necessary
		_webDriver.FindElement(By.Id("calculated")).SendKeys(randomDecimal.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal3.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();
		Assert.Equal(UrlWith("/CreateTransaction"), _webDriver.Url);
	}

	[Fact]
	public void MissingDate() {
		decimal randomDecimal = 192.3M;
		decimal randomDecimal2 = 32.3M;
		decimal randomDecimal3 = 384.3M;


		_webDriver.Navigate().GoToUrl(UrlWith("/Transaction/CreateTransaction"));
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		//this is somehow necessary
		_webDriver.FindElement(By.Id("calculated")).SendKeys(randomDecimal.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal.ToString(randomDecimal2.ToString(_germanInfo)));
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal3.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();
		Assert.Equal(UrlWith("/CreateTransaction"), _webDriver.Url);
	}

	[Fact]
	public void MissingCurrency() {
		decimal randomDecimal = 192.3M;
		decimal randomDecimal2 = 32.3M;
		decimal randomDecimal3 = 384.3M;


		_webDriver.Navigate().GoToUrl(UrlWith("/Transaction/CreateTransaction"));
		_webDriver.FindElement(By.Id("calculated")).SendKeys(randomDecimal.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal.ToString(randomDecimal2.ToString(_germanInfo)));
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal3.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();
		Assert.Equal(UrlWith("/CreateTransaction"), _webDriver.Url);
	}

	[Fact]
	public void MissingDateRate() {
		decimal randomDecimal = 192.3M;
		decimal randomDecimal2 = 32.3M;

		_webDriver.Navigate().GoToUrl(UrlWith("/Transaction/CreateTransaction"));
		_webDriver.FindElement(By.Id("btn-now")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		//this is somehow necessary
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal.ToString(randomDecimal2.ToString(_germanInfo)));
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal.ToString(_germanInfo));
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();
		Assert.Equal(UrlWith("/CreateTransaction"), _webDriver.Url);
	}
}
