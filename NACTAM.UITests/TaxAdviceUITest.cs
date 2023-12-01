// marked idempotent
namespace NACTAM.UITests {
	public class TaxAdviceUITest : BaseUITest {
		public TaxAdviceUITest() : base(true) { }

		/// <summary>
		/// Checks if Service-Steuerliche Empfehlung site loads properly.
		/// </summary>
		/// Cagil Ceren Aslan
		[Fact]
		public void TestWhenExecutedReturnsTaxRecommendationView() {
			GoToLogin();
			LoginUser("User1");

			_webDriver.FindElement(By.Id("services")).Click();
			_webDriver.FindElement(By.Id("tax-recommendation")).Click();

			Thread.Sleep(1000);
			Assert.Contains("Übersicht der steuerrelaventen Gewinne", _webDriver.PageSource);
			Assert.Contains("Gesamtgewinn aus Verkäufen in €", _webDriver.PageSource);
			Assert.Contains("Gesamtgewinn aus sonstigen Leistungen in €", _webDriver.PageSource);
			Assert.Contains("Gewinnoptimierung Empfehlungen", _webDriver.PageSource);
			Assert.Contains("Übersicht über Haltefristen", _webDriver.PageSource);
		}
	}
}
