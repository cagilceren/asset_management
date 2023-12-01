using System.ComponentModel;

using NACTAM.Models;
using NACTAM.Models.API;

namespace NACTAM.ViewModels {

	/// <summary>
	/// AssetViewModel used for displaying data into AssetsPage
	/// Also converts transactions to assets and summarizes them
	/// </summary>
	public class AssetsViewModel {

		/// <summary>
		/// Value of summarized transactions
		/// </summary>
		[DisplayName("Wert")]
		public decimal Value { get; set; }
		[DisplayName("Währung")]
		/// <summary>
		/// Currency name of asset
		/// </summary>
		public string? Currency { get; set; }
		/// <summary>
		/// ShortName of asset e.g. BTC
		/// </summary>
		[DisplayName("Kürzel")]
		public string? ShortName { get; set; }
		/// <summary>
		/// Currennt Rate of asset, maximum 24h old
		/// </summary>
		[DisplayName("Kurs")]
		public decimal Rate { get; set; }
		/// <summary>
		/// Logo of currency
		/// </summary>
		[DisplayName("Logo")]
		public string Logo { get; set; }
		/// <summary>
		/// Logo of currency
		/// </summary>
		[DisplayName("Menge")]
		public decimal Amount { get; set; }

		/// <summary>
		/// This constructor is needed to use the ConvertTransaction method
		/// </summary>
		public AssetsViewModel() { }

		/// <summary>
		/// This constructor is needed to initialize an asset without Logo
		/// Here the default coingecko icon is used
		/// This contructor could be called, if there are no requests for current Logo from coingecko left
		/// Authornames: Philipp Eckel, Marco Lembert
		/// </summary>
		/// <param name="value">Value of summarized asset</param>
		/// <param name="currency">Name of currency</param>
		/// <param name="rate">Current rate of currency</param>
		/// <param name="shortName">Shortname of currency e.g. BTC</param>
		/// <param name="amount">Amount of Asset</param>
		public AssetsViewModel(decimal value, string currency, decimal rate, string shortName, decimal amount) {
			this.Value = value;
			this.Currency = currency;
			this.ShortName = shortName;
			this.Rate = rate;
			this.Logo = "https://www.coingecko.com/favicon-96x96.png";
			this.Amount = amount;
		}

		/// <summary>
		/// This constructor is needed to initialize an asset
		/// Authornames: Philipp Eckel, Marco Lembert
		/// </summary>
		/// <param name="value">Value of summarized asset</param>
		/// <param name="currency">Name of currency</param>
		/// <param name="rate">Current rate of currency</param>
		/// <param name="logo">Logo of currency</param>
		/// <param name="shortName">Shortname of currency e.g. BTC</param>
		/// <param name="amount">Amount of Asset</param>
		public AssetsViewModel(decimal value, string currency, decimal rate, string logo, string shortName, decimal amount) {
			this.Value = value;
			this.Currency = currency;
			this.Rate = rate;
			this.Logo = logo;
			this.ShortName = shortName;
			this.Amount = amount;
		}

		/// <summary>
		/// This method is required for summarizing all transactions by their currency to their total value
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="transactions">List of all transactions from user</param>
		/// <param name="dictionary">Summarized transactions by their currencyId with their total amount</param>
		/// <param name="api">ICurrencyApi for accessing several api methods</param>
		/// <returns>Liste mit allen assetsviewmodels</returns>
		static public List<AssetsViewModel> ConvertTransactions(IEnumerable<Transaction> transactions, Dictionary<int, decimal> dictionary, ICurrencyApi api) {
			List<AssetsViewModel> assets = new List<AssetsViewModel>();
			List<int> keys = new List<int>(dictionary.Keys);
			// make sure all currencies have a recent exchange rate
			try {
				api.GetExchangeRates(keys);
			} catch (Exception e) {
				throw;
			}
			foreach (int key in dictionary.Keys) {
				CryptoCurrency currency = api.GetCryptoCurrency(key);
				decimal exchangeRate = api.GetExchangeRate(key);

				if (currency.Logo == null) {
					CryptoCurrency search = null;
					try {
						search = api.SearchCryptoCurrency(currency.ApiId).Where(c => c.ApiId == currency.ApiId).FirstOrDefault();
					} catch (Exception e) {
						Console.WriteLine(e.Message);
					}
					if (search != null && search.Logo != null) {
						AssetsViewModel model = new AssetsViewModel((dictionary[key] * exchangeRate), currency.Name, exchangeRate, search.Logo, currency.ShortName, dictionary[key]);
						assets.Add(model);
					} else {
						AssetsViewModel model = new AssetsViewModel((dictionary[key] * exchangeRate), currency.Name, exchangeRate, currency.ShortName, dictionary[key]);
						assets.Add(model);
					}
				} else {
					AssetsViewModel model = new AssetsViewModel((dictionary[key] * exchangeRate), currency.Name, exchangeRate, currency.Logo, currency.ShortName, dictionary[key]);
					assets.Add(model);
				}

			}

			return assets;
		}
	}
}
