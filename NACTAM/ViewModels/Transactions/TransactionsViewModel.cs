using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

using NACTAM.Exceptions;
using NACTAM.Models;
using NACTAM.Models.API;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Viewmodel for the transactions
	/// </summary>
	public class TransactionsViewModel : ICloneable {
		[DisplayName("ID")] public int? Id { get; set; }

		/// <summary>
		/// Date of the transaction
		/// </summary>
		[Required(ErrorMessage = "Datum darf nicht leer sein!")]
		[DisplayName("Datum")]
		public DateTime Date { get; set; }

		/// <summary>
		/// Exchange rate of the transaction
		/// </summary>
		[Required(ErrorMessage = "Kurs darf nicht leer sein!")]
		[Range(0, double.MaxValue, ErrorMessage = "Kurs darf nicht negativ sein!")]
		[DisplayName("Kurs")]
		public decimal Rate { get; set; }

		/// <summary>
		/// Type of the transaction, must be either Sell, Buy, Staking or Mining
		/// </summary>
		[DisplayName("Art")]
		[Required(ErrorMessage = "Bitte wählen Sie eine Art aus!")]
		[RegularExpression(@"(^(Sell|Buy|Staking|Mining)$)",
			ErrorMessage = "Art der Transaktion muss Sell, Buy, Staking oder Mining sein!")]
		public string Type { get; set; }

		/// <summary>
		/// Currency of the transaction as a string
		/// </summary>
		[DisplayName("Kryptowährung")]
		[Required(ErrorMessage = "Bitte wählen Sie eine Kryptowährung aus!")]
		public string CryptoCurrency { get; set; }

		/// <summary>
		/// Fee of the transaction, must be positive
		/// </summary>
		[Required(ErrorMessage = "Gebühr darf nicht leer sein!")]
		[Range(0, double.MaxValue, ErrorMessage = "Gebühr darf nicht negativ sein!")]
		[DisplayName("Gebühr")]
		public decimal Fee { get; set; }

		/// <summary>
		/// Amount of coins of the transaction, must be positive
		/// </summary>
		[Required(ErrorMessage = "Menge darf nicht leer sein!")]
		[Range(0, double.MaxValue, ErrorMessage = "Menge darf nicht negativ sein!")]
		[DisplayName("Menge")]
		public decimal Amount { get; set; }

		/// <summary>
		/// Symbol of the currency for the TransactionOverview
		/// </summary>
		[BindNever]
		public string? CryptoSymbol { get; set; }

		/// <summary>
		/// Logo of currency
		/// </summary>
		[DisplayName("Logo")]
		public string? Logo { get; set; }


		/// <summary>
		/// Constructor for the viewmodel
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="transaction"> Transaction to be converted to viewmodel</param>
		/// <param name="coingeckoName">Name of the currency</param>
		public TransactionsViewModel(Transaction transaction, string coingeckoName) {
			Id = transaction.Id;
			Date = transaction.Date;
			Rate = transaction.ExchangeRate;
			Type = transaction.Type.ToString();
			Amount = transaction.Amount;
			Fee = transaction.Fee;
			CryptoCurrency = coingeckoName;
		}

		/// <summary>
		/// Constructor for the viewmodel
		/// this constructor is specifically for the transactionsoverviewmodel
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="transaction"> Transaction to be converted to viewmodel</param>
		/// <param name="coingeckoName">Name of the currency</param>
		/// <param symbol="">Symbol like BTC</param>
		public TransactionsViewModel(Transaction transaction, string coingeckoName, string symbol, string logo) {
			Id = transaction.Id;
			Date = transaction.Date;
			Rate = transaction.ExchangeRate;
			Type = transaction.Type.ToString();
			Amount = transaction.Amount;
			Fee = transaction.Fee;
			CryptoCurrency = coingeckoName;
			CryptoSymbol = symbol;
			Logo = logo;
		}


		/// <summary>
		/// Empty constructor for the viewmodel, required for Bindings
		/// Autornames: Philipp Eckel, Marco Lembert
		/// </summary>
		public TransactionsViewModel() { }

		/// <summary>
		/// Converts the viewmodel to a transaction
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="userId"> Id of the user</param>
		/// <param name="api"> Api to get the currency</param>
		/// <returns> Transaction with the data of the viewmodel</returns>
		/// <exception cref="CurrencyNotFoundException"></exception>
		public Transaction ToTransaction(string userId, ICurrencyApi api) {
			var type = (TransactionType)Enum.Parse(typeof(TransactionType), Type);
			var currency = api.GetCryptoCurrencyByName(CryptoCurrency);
			// if no currency is found, throw an exception
			if (currency == null) throw new CurrencyNotFoundException("Currency not found");
			//if no rate is saved or last rate is older than 1 day, get new rate
			if (currency.Rate == null || DateTime.Now - currency.LastUpdated > TimeSpan.FromDays(1)) {
				api.GetExchangeRate(currency.Id);

				if (currency == null) throw new CurrencyNotFoundException("Currency not found");
			}

			var currencyId = currency.Id;
			if (Id != null) {
				return new Transaction(Id.Value, userId, Amount, Fee, Rate, type, Date, currencyId);
			} else {
				return new Transaction(userId, Amount, Fee, Rate, type, Date, currencyId);
			}
		}


		/// <summary>
		/// Creates a deep copy of <see cref="TransactionsViewModel"/> object.
		/// </summary>
		/// <returns><see cref="TransactionsViewModel"/> object</returns>
		/// <author>Cagil Ceren Aslan</author>
		public object Clone() {
			return new TransactionsViewModel() {
				Id = Id,
				Date = Date,
				Rate = Rate,
				Type = Type,
				CryptoCurrency = CryptoCurrency,
				Fee = Fee,
				Amount = Amount,
			};
		}
	}
}
