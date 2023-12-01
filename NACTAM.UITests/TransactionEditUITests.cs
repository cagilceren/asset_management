namespace NACTAM.UITests;

/// <summary>
/// UI Test for different invalid edit scenarios
/// Couldnt test missing type or missing currency, because the default selected value is disabled
/// Authornames: Marco Lembert
/// </summary>
public class TransacionEditUITests : BaseUITest {
	public TransacionEditUITests() : base(true) {
		GoToLogin();
		LoginUser("User3");

		_webDriver.FindElement(By.Id("transactions")).Click();
		_webDriver.FindElement(By.CssSelector(".fa-edit")).Click();
		Thread.Sleep(100);
	}


	[Fact]
	public void MissingAmount() {
		//generate random decimal
		Random random = new Random();
		decimal randomDecimal = random.Next(1, 1000000);
		decimal randomDecimal2 = random.Next(1, 1000000);
		decimal randomDecimal3 = random.Next(1, 1000000);

		_webDriver.FindElement(By.Id("btn-now")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");

		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		_webDriver.FindElement(By.XPath("//*[@id=\"calculated\"]")).SendKeys(randomDecimal.ToString());
		_webDriver.FindElement(By.Id("Amount")).Clear();
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal3.ToString());
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();

		Assert.StartsWith(UrlWith("/EditTransaction"), _webDriver.Url);
	}

	[Fact]
	public void MissingFee() {
		//generate random decimal
		Random random = new Random();
		decimal randomDecimal = random.Next(1, 1000000);
		decimal randomDecimal2 = random.Next(1, 1000000);
		decimal randomDecimal3 = random.Next(1, 1000000);

		_webDriver.FindElement(By.Id("btn-now")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");

		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		//_webDriver.FindElement(By.XPath("/html/body/div/div/div/main/div[1]/div/div/div/div[2]/div/form/div[4]/div/div[2]/a")).Click();
		//this is somehow necessary
		_webDriver.FindElement(By.XPath("//*[@id=\"calculated\"]")).SendKeys(randomDecimal.ToString());
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal.ToString(randomDecimal2.ToString()));
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("Fee")).Clear();
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();

		Assert.StartsWith(UrlWith("/EditTransaction"), _webDriver.Url);
	}

	[Fact]
	public void MissingDate() {
		//generate random decimal
		Random random = new Random();
		decimal randomDecimal = random.Next(1, 1000000);
		decimal randomDecimal2 = random.Next(1, 1000000);
		decimal randomDecimal3 = random.Next(1, 1000000);

		_webDriver.FindElement(By.Id("btn-now")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		//_webDriver.FindElement(By.XPath("/html/body/div/div/div/main/div[1]/div/div/div/div[2]/div/form/div[4]/div/div[2]/a")).Click();
		//this is somehow necessary
		_webDriver.FindElement(By.XPath("//*[@id=\"calculated\"]")).SendKeys(randomDecimal.ToString());
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal.ToString(randomDecimal2.ToString()));
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("date-input")).Clear();
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal3.ToString());
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();
		Assert.StartsWith(UrlWith("/EditTransaction"), _webDriver.Url);
	}

	[Fact]
	public void MissingDateRate() {
		//generate random decimal
		Random random = new Random();
		decimal randomDecimal = random.Next(1, 1000000);
		decimal randomDecimal2 = random.Next(1, 1000000);
		decimal randomDecimal3 = random.Next(1, 1000000);

		_webDriver.FindElement(By.Id("btn-now")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-selection")).Click();
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys("01coin");
		_webDriver.FindElement(By.CssSelector(".select2-search__field")).SendKeys(Keys.Enter);

		//_webDriver.FindElement(By.XPath("/html/body/div/div/div/main/div[1]/div/div/div/div[2]/div/form/div[4]/div/div[2]/a")).Click();
		//this is somehow necessary
		_webDriver.FindElement(By.Id("Amount")).SendKeys(randomDecimal.ToString(randomDecimal2.ToString()));
		_webDriver.FindElement(By.XPath("//*[@id=\"calculated\"]")).Clear();
		{
			var dropdown = _webDriver.FindElement(By.Id("Type"));
			dropdown.FindElement(By.XPath("//option[. = 'Kauf']")).Click();
		}
		_webDriver.FindElement(By.Id("Fee")).SendKeys(randomDecimal.ToString());
		_webDriver.FindElement(By.Id("btn-bestaetigen")).Click();
		Assert.StartsWith(UrlWith("/EditTransaction"), _webDriver.Url);
	}
}
