using NACTAM.Models;
using NACTAM.Models.API;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Overview of all transactions for Transactions page
	/// </summary>
	public class TransactionsOverviewViewModel {
		/// <summary>
		/// List of transactions
		/// </summary>
		public List<TransactionsViewModel> Transactions { get; set; }

		/// <summary>
		/// Constructor for TransactionsOverviewViewModel
		/// Authornames: Marco Lembert, Tuan Bui
		/// </summary>
		/// <param name="transactions">List of Transactions</param>
		/// <param name="api">Api to resolve CurrencyId of Transactions</param>
		public TransactionsOverviewViewModel(IEnumerable<Transaction> transactions, ICurrencyApi api) {
			Transactions = transactions.Select(transaction => {
				CryptoCurrency currency = api.GetCryptoCurrency(transaction.CurrencyId);
				string? logo = currency.Logo;
				if (currency.Logo == null) {
					try {
						var search = api
							.SearchCryptoCurrency(currency.Name)
							.Where(c => c.ApiId == currency.ApiId)
							.FirstOrDefault();
						logo = search?.Logo ?? "https://www.coingecko.com/favicon-96x96.png";
					} catch (Exception e) {
						Console.WriteLine(e);
						logo = "https://www.coingecko.com/favicon-96x96.png";
					}

				}
				return new TransactionsViewModel(transaction, currency.Name, currency.ShortName, logo);
			}).ToList();
		}


		/// <summary>
		/// Empty constructor for TransactionsOverviewViewModel, required for Bindings
		/// Authornames: Marco Lembert, Philipp Eckel
		/// </summary>
		public TransactionsOverviewViewModel() {
		}
	}
}
