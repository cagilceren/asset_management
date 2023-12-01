using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using NACTAM.Models.API;


namespace NACTAM.Models;

/// <summary>
/// Transaction Model class (also used for the database)
/// </summary>
public class Transaction {
	/// <summary>
	/// Database primary key generated by the database used to identify the transaction.
	/// Warning: The id is artificially generated by the database and if lost, cannot be recreated!
	/// </summary>
	[Key]
	[Required]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	/// <summary>
	/// Userid from user to specify whether a transaction belongs to a user
	/// </summary>
	public string UserId { get; set; }
	/// <summary>
	/// Amount of selected cryptocurrency
	/// </summary>
	public decimal Amount { get; set; }
	/// <summary>
	/// Deposited fee after transaction was bought / sold
	/// </summary>
	public decimal Fee { get; set; }
	/// <summary>
	/// DateTTime of transaction
	/// </summary>
	public DateTime Date { get; set; }
	/// <summary>
	/// Id from selected currency
	/// Warning: This is not the ApiId, its the database id
	/// </summary>
	public int CurrencyId { get; set; }
	/// <summary>
	/// Rate of transaction
	/// </summary>
	public decimal ExchangeRate { set; get; }
	/// <summary>
	/// Transactiontype refers to enum, whether buy, sell, mining, staking
	/// </summary>
	public TransactionType Type { get; set; }
	/// <summary>
	/// Delete attribut, default to false
	/// After delete of transaction, set to be true
	/// Its for softdelete function
	/// </summary>
	public bool Deleted { get; set; }

	/// <summary>
	/// Constructor for a transaction not yet saved in the database
	/// Authorname: Philipp Eckel
	/// </summary>
	/// <param name="userId"> user id of the user who owns the transaction</param>
	/// <param name="amount"> amount of coins of the transaction, must be positive</param>
	/// <param name="fee"> fee of the transaction, must be positive</param>
	/// <param name="exchangeRate"> exchange rate of the transaction</param>
	/// <param name="type"> type of the transaction, must be either Sell, Buy, Staking or Mining</param>
	/// <param name="date"> date of the transaction</param>
	/// <param name="currencyId"> id of the currency of the transaction</param>
	public Transaction(string userId, decimal amount, decimal fee, decimal exchangeRate, TransactionType type,
		DateTime date, int currencyId) {
		UserId = userId;
		Amount = amount;
		Fee = fee;
		Date = date;
		CurrencyId = currencyId;
		ExchangeRate = exchangeRate;
		Type = type;
		Deleted = false;
	}

	/// <summary>
	/// Constructor for a transaction already saved in the database
	/// Authorname: Marco Lembert
	/// </summary>
	/// <param name="id"> id of the transaction</param>
	/// <param name="userId"> user id of the user who owns the transaction</param>
	/// <param name="amount"> amount of coins of the transaction, must be positive</param>
	/// <param name="fee"> fee of the transaction, must be positive</param>
	/// <param name="exchangeRate"> exchange rate of the transaction</param>
	/// <param name="type"> type of the transaction, must be either Sell, Buy, Staking or Mining</param>
	/// <param name="date"> date of the transaction</param>
	/// <param name="currencyId"> id of the currency of the transaction</param>
	public Transaction(int id, string userId, decimal amount, decimal fee, decimal exchangeRate,
		TransactionType type,
		DateTime date, int currencyId) {
		Id = id;
		UserId = userId;
		Amount = amount;
		Fee = fee;
		Date = date;
		CurrencyId = currencyId;
		ExchangeRate = exchangeRate;
		Type = type;
		Deleted = false;
	}

	/// <summary>
	/// Empty constructor for model binding
	/// Authorname: Philipp Eckel, Marco Lembert
	/// </summary>
	public Transaction() {
	}

}

