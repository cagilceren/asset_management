using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

using NACTAM.Models.API;
using NACTAM.ViewModels;

namespace NACTAM.Models {

	/// <summary>
	/// Class representing a transaction in the csv format
	/// </summary>
	public class TransactionCsv {

		/// <summary>
		/// Name of the currency
		/// </summary>
		[Name("Währung")]
		public string CurrencyName { get; set; }

		/// <summary>
		/// Date of the transaction
		/// </summary>
		[Name("Datum")]
		public DateTime Date { get; set; }

		/// <summary>
		///	Amount of coins of the transaction, must be positive
		/// </summary>
		[Name("Menge")]
		public decimal Amount { get; set; }

		/// <summary>
		/// Fee of the transaction, must be positive
		/// </summary>
		[Name("Gebühr")]
		public decimal Fee { get; set; }

		/// <summary>
		/// Exchange rate of the transaction
		/// </summary>
		[Name("Kurs")]
		public decimal ExchangeRate { get; set; }

		/// <summary>
		/// Type of the transaction, must be either Sell, Buy, Staking or Mining
		/// </summary>
		[Name("Art")]
		public TransactionType Type { get; set; }

		/// <summary>
		/// Constructor for a transaction in csv format
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="currencyName"> Name of the currency</param>
		/// <param name="date"> Date of the transaction</param>
		/// <param name="amount"> Amount of coins of the transaction, must be positive</param>
		/// <param name="fee"> Fee of the transaction, must be positive</param>
		/// <param name="exchangeRate"> Exchange rate of the transaction</param>
		/// <param name="type"> Type of the transaction, must be either Sell, Buy, Staking or Mining</param>
		public TransactionCsv(string currencyName, DateTime date, decimal amount, decimal fee, decimal exchangeRate,
			TransactionType type) {
			CurrencyName = currencyName;
			Date = date;
			Amount = amount;
			Fee = fee;
			ExchangeRate = exchangeRate;
			Type = type;
		}

		/// <summary>
		/// Empty constructor for model binding
		/// Authornames: Marco Lembert, Philipp Eckel
		/// </summary>
		public TransactionCsv() {
		}

		/// <summary>
		/// Convert a transaction to a transaction into the csv representation (ready to be written to a csv file)
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="api"></param>
		public TransactionCsv(Transaction transaction, ICurrencyApi api) {
			CurrencyName = api.GetCryptoCurrency(transaction.CurrencyId).Name;
			Date = transaction.Date;
			Amount = transaction.Amount;
			Fee = transaction.Fee;
			ExchangeRate = transaction.ExchangeRate;
			Type = transaction.Type;
		}

		/// <summary>
		/// Converts a transaction in csv format to a transaction
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="userId"> Id of the user</param>
		/// <param name="api"> Api to resolve CurrencyId of Transactions</param>
		/// <returns> Transaction</returns>
		public Transaction ToTransaction(string userId, ICurrencyApi api) {
			return new Transaction(userId, Amount, Fee, ExchangeRate, Type, Date,
				api.GetCryptoCurrencyByName(CurrencyName).Id);
		}

		/// <summary>
		/// Reads transactions from a csv file
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="stream"> Stream of the csv file</param>
		/// <returns> List of transactions</returns>
		public static List<TransactionCsv> ReadTransactionsFromCsv(IFormFile csvFile) {
			try {
				if (csvFile != null && csvFile.Length > 0) {
					using (var streamReader = new StreamReader(csvFile.OpenReadStream())) {
						Stream stream = streamReader.BaseStream;
						using (var reader = new StreamReader(stream))
						using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture))) {
							return csv.GetRecords<TransactionCsv>().ToList();
						}
					}
				}

				return new List<TransactionCsv>();
			} catch (Exception e) {
				throw;
			}
		}

		/// <summary>
		/// Writes transactions to a csv file
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="transactions"> List of transactions to be written to the csv file</param>
		/// <param name="stream"> Stream of the csv file</param>
		public static void WriteTransactionsToCsv(IEnumerable<TransactionCsv> transactions, Stream stream) {
			using (var writer = new StreamWriter(stream))
			using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture))) {
				csv.WriteRecords(transactions);
			}
		}

	}
}