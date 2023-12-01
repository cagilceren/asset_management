namespace NACTAM.Models.API {

	/// <summary>
	/// This interface provides a layer of abstraction between the CurrencyContainer and the rest of the application. It is used to load currencies, update rates, and search for currencies.
	/// </summary>
	public interface ICurrencyApi {
		/// <summary>
		/// This method loads all currencies from the database into the CurrencyContainer, should be called on startup and by BackgroundServices
		/// Authornames: Philipp Eckel
		/// </summary>
		public Task LoadCryptoCurrenciesAsync();

		/// <summary>
		/// This method returns a list of all available currencies
		/// Authorname: Philipp Eckel
		/// </summary>
		/// <returns>A list of all available currencies</returns>
		public List<CryptoCurrency> GetCryptoCurrencies();

		/// <summary>
		/// This method returns a desired currency
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="id"> The id of the desired currency</param>
		/// <returns>The desired currency</returns>
		public CryptoCurrency GetCryptoCurrency(int id);

		/// <summary>
		/// This method returns a currency with the specified name
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="name">The name of the desired currency</param>
		/// <returns>The desired currency</returns>
		public CryptoCurrency GetCryptoCurrencyByName(string name);

		/// <summary>
		/// This method updates the rates of all currencies, if the currencys are stored in the database
		/// Authornames: Philipp Eckel
		/// </summary>
		public Task UpdateRates();

		/// <summary>
		/// This method updates the rates of specified currencies, if the currencys are stored in the database
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="ids">The ids of the currencies to update</param>
		public Task UpdateRates(List<int> ids);

		/// <summary>
		/// Gets the exchange rate of a currency
		/// WARNING: This method should not be used to get the exchange rate of multiple currencies, as it will make multiple API calls
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="id">The id of the currency</param>
		/// <returns>The exchange rate of the currency</returns>
		public decimal GetExchangeRate(int id);

		/// <summary>
		/// Gets the exchange rate of multiple currencies. Use this method to get the exchange rate of multiple currencies, as it will only make one API call
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="ids">The ids of the currencies</param>
		/// <returns>The exchange rates of the currencies</returns>
		public List<decimal> GetExchangeRates(List<int> ids);

		/// <summary>
		/// Gets the exchange rate of a currency at a specific date
		/// WARNING: This method should not be used to get latest exchange rate, as no caching is implemented
		/// Authornames: Marco Lembert, modified by Philipp Eckel to save metadata in database
		/// </summary>
		/// <param name="id">The id of the currency</param>
		/// <param name="date">The desired date on which the exchange rate should be</param>
		/// <returns>The exchange rate of the currency at the specified date</returns>
		public decimal GetExchangeRate(int id, DateTime date);

		/// <summary>
		/// Searches for a currency with the specified query (can be used to get and cache icons)
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="query">The query to search for</param>
		/// <returns> MULTIPLE CURRENCIES: A list of currencies that match the query </returns>
		public List<CryptoCurrency> SearchCryptoCurrency(string query);

		/// <summary>
		/// Searches for a currency with the specified query (can be used to get and cache icons)
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="query">Multiple queries to search for</param>
		/// <returns> MULTIPLE CURRENCIES: A list of currencies that match the query </returns>
		public List<CryptoCurrency> SearchCryptoCurrency(List<string> query);

		/// <summary>
		/// Returns values which can be used to display a chart
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="id">The id of the currency</param>
		/// <param name="days">The number of days to display</param>
		/// <param name="interval"> The interval between the values</param>
		/// <param name="precision">The precision of the values</param>
		/// <returns>A list of values which can be used to display a chart</returns>
		public List<decimal> GetCoinMarketChart(int id, int days, string interval, string precision);
	}
}
