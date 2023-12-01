using System.Runtime.InteropServices;

namespace NACTAM.UITests;

/// <summary>
/// Base class for UI Tests
///
///
/// author: Tuan Bui
/// </summary>
public abstract class BaseUITest : IDisposable {
	public static string BinaryLocation = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome" :
		System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (
				File.Exists("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe") ?
				"C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe" :
				"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe") :
		"/usr/bin/google-chrome-stable";
	protected IWebDriver _webDriver;
	public string BaseURL = "http://localhost:5101";
	protected readonly string _downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

	/// <summary>
	/// constructor helper for the UI Base class
	/// </summary>
	/// <param name="headless">runs the test headless</param>
	public BaseUITest(bool headless) {
		var chromeOptions = new ChromeOptions();
		chromeOptions.AddArguments("--disable-dev-shm-usage");
		chromeOptions.AddArguments("disable-gpu", "--ignore-certificate-errors", "--no-sandbox", "--verbose");
		chromeOptions.AddArgument("--lang=de-DE");
		if (headless)
			chromeOptions.AddArguments("--headless=new");

		chromeOptions.BinaryLocation = BinaryLocation;
		if (!Directory.Exists(_downloadPath)) {
			Directory.CreateDirectory(_downloadPath);
		}
		chromeOptions.AddUserProfilePreference("download.default_directory", _downloadPath);
		_webDriver = new ChromeDriver(chromeOptions);
		_webDriver.Manage()
			.Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);


	}

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			_webDriver.Quit();
		}
	}

	/// <summary>
	/// a helper function with the path appended
	/// </summary>
	/// <param name="path">the path to be appended</param>
	public string UrlWith(string path) => BaseURL + path;


	/// <summary>
	/// helper function to login as user
	/// </summary>
	/// <param name="username">username to be used</param>
	/// <param name="password">password to be used</param>
	public void LoginUser(string username, string password = "Test$123") {
		Thread.Sleep(500);
		_webDriver.FindElement(By.Id("username")).SendKeys(username);
		_webDriver.FindElement(By.Id("password")).SendKeys(password);
		_webDriver.FindElement(By.CssSelector(".btn-block")).Click();
		Thread.Sleep(500);
	}

	/// <summary>
	/// helper function to login a private person
	/// </summary>
	public void LoginExamplePerson()
		=> LoginUser("User3");


	/// <summary>
	/// helper function to login a private person
	/// </summary>
	public void LoginExampleTaxAdvisor()
		=> LoginUser("taxadvisor2");

	public void Logout() {
		Thread.Sleep(50);
		_webDriver.FindElement(By.CssSelector(".fa-right-from-bracket")).Click();
		_webDriver.Navigate().GoToUrl("http://localhost:5101/?login=true");
		Thread.Sleep(25);
	}

	public void GoToLoginSlow() {
		GoHome();
		_webDriver.FindElement(By.CssSelector(".show-login-button")).Click();
		Thread.Sleep(50);
	}

	public void GoToLogin() {
		_webDriver.Navigate().GoToUrl("http://localhost:5101/?login=true");
	}

	public void GoHome() {
		_webDriver.Navigate().GoToUrl(BaseURL);
	}

	public string? GetLatestFileName(string downloadPath) {
		return Directory.GetFiles(downloadPath)
			.FirstOrDefault(filePath => Path.GetExtension(filePath)
				.Equals(".pdf", StringComparison.OrdinalIgnoreCase) && File.GetCreationTime(filePath) > DateTime.Now - TimeSpan.FromHours(1));
	}

	public void CheckNoPDF() {
		Thread.Sleep(500);
		string? path = GetLatestFileName(_downloadPath);
		Assert.Null(path);
	}

	public void CheckAndDeletePDF() {
		Thread.Sleep(500);
		string? path = GetLatestFileName(_downloadPath);
		Assert.NotNull(path);
		File.Delete(path);
	}

	public void SwitchToUser(string username, string password = "Test$123") {
		Logout();
		GoToLogin();
		LoginUser(username, password);
	}
}
