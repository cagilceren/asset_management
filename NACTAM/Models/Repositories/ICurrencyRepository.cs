namespace NACTAM.Models.Repositories {

	public interface ICurrencyRepository {

		/// <summary>
		/// This method returns a list of all currencies in the database.
		/// Authornames: Marco Lembert, modified by Philipp Eckel for async
		/// </summary>
		public Task<List<CryptoCurrency>> GetCurrenciesAsync();
		/// <summary>
		/// This method returns a currency with the specified id.
		/// Authornames: Marco Lembert, modified by Philipp Eckel for async
		/// </summary>
		/// <param name="id">The id of the desired currency</param>
		public Task<CryptoCurrency> GetCurrencyAsync(int id);
		/// <summary>
		/// This method returns a currency with the specified name.
		/// Authornames: Marco Lembert, modified by Philipp Eckel for async
		/// </summary>
		/// <param name="name">The name of the desired currency</param>
		public Task<CryptoCurrency> GetCurrencyByNameAsync(string name);
		/// <summary>
		/// This method updates and adds currencies to the database, recommended for batch updates.
		/// Authornames: Marco Lembert, modified by Philipp Eckel for async
		/// </summary>
		/// <param name="currency">Currencies to update or add</param>
		public Task UpdateCurrencyAsync(List<CryptoCurrency> currency);
		/// <summary>
		/// This method updates and adds a currency to the database, recommended for single updates.
		/// WARNING: Using this method for batch updates will result in a lot of database calls.
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="currency">Currency to update or add</param>
		public Task UpdateCurrencyAsync(CryptoCurrency currency);
	}
}