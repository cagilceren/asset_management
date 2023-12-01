using NACTAM.Exceptions;
using NACTAM.Identity.Data;
using NACTAM.Models.Repositories;
using NACTAM.ViewModels;

namespace NACTAM.Models;

/// <summary>
/// Implementation of the ITransactionRepository interface for the TransactionContainer
/// </summary>
public class TransactionContainer : ITransactionRepository {

	private readonly NACTAMContext _db;

	/// <summary>
	/// Constructor for the TransactionContainer class, injects the NACTAMContext.
	/// </summary>
	/// <param name="db">Contextclass with all DbSets</param>
	public TransactionContainer(NACTAMContext db) {
		_db = db;
	}

	/// <inheritdoc />
	public IEnumerable<Transaction> GetTransactions(string userid) {
		return _db.Transaction.Where(t => t.UserId.Equals(userid) && t.Deleted == false).OrderByDescending(t => t.Date);
	}

	/// <inheritdoc />
	public IEnumerable<Transaction> GetDeletedTransactions(string userid) {
		return _db.Transaction.Where(t => t.UserId.Equals(userid) && t.Deleted);
	}

	/// <summary>
	/// This method returns a transaction with the desired id
	/// Authornames: Marco Lembert
	/// </summary>
	/// <param name="id">The id from transaction</param>
	/// <returns>A single Transaction with the submitted id</returnes>
	public Transaction GetTransaction(int id) {
		Transaction transaction = _db.Transaction.FirstOrDefault(x => x.Id == id);
		return transaction;
	}

	/// <inheritdoc />
	public Transaction GetTransaction(int id, string userId) {
		Transaction transaction = GetTransaction(id);
		if (transaction.UserId != userId) throw new IllegalUserOperationException("Transaction does not belong to user");
		return transaction;
	}

	/// <inheritdoc />
	public async Task AddTransaction(string userid, Transaction transaction) {
		transaction.UserId = userid;
		if (!ValidateTransactionDate(transaction)) {
			throw new IllegalDateException("Transaction is not valid");
		}
		if (!ValidateTransactionBalance(transaction, userid)) {
			throw new TransactionBalanceException("Transaction is not valid");
		}

		await _db.Transaction.AddAsync(transaction);
		await _db.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task UpdateTransaction(string userid, Transaction transaction) {
		if (!ValidateTransactionBalance(transaction, userid)) {
			throw new TransactionBalanceException("Transaction is not valid");
		}
		if (!ValidateTransactionDate(transaction)) {
			throw new IllegalDateException("Transaction is not valid");
		}
		var entity = GetTransaction(transaction.Id, userid);
		if (entity != null) {
			entity.Amount = transaction.Amount;
			entity.Fee = transaction.Fee;
			entity.Date = transaction.Date;
			entity.ExchangeRate = transaction.ExchangeRate;
			entity.CurrencyId = transaction.CurrencyId;
			entity.Type = transaction.Type;
		}


		await _db.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task RemoveTransaction(string userid, int id) {
		Transaction? transaction = _db.Transaction.FirstOrDefault(x => x.Id == id);

		if (transaction == null)
			throw new TransactionNotFoundException("Transaction not found");
		if (transaction.UserId != userid)
			throw new IllegalUserOperationException("Transaction does not belong to user");
		if (!ValidateTransactionBalanceDelete(transaction, userid))
			throw new TransactionBalanceException("Transaction is not valid");

		transaction.Deleted = true;
		await _db.SaveChangesAsync();
	}

	/// <inheritdoc />
	public bool ValidateTransactionBalance(Transaction transaction, string userid) {
		if (transaction.Type != TransactionType.Sell) {
			return true;
		}

		// total = buy + mining + staking
		decimal total = GetTransactions(userid).Where(item => (item.Type == TransactionType.Buy || item.Type == TransactionType.Mining || item.Type == TransactionType.Staking) && item.CurrencyId == transaction.CurrencyId).Sum(item => item.Amount);
		decimal totalSell = GetTransactions(userid).Where(item => item.Type == TransactionType.Sell && item.CurrencyId == transaction.CurrencyId).Sum(item => item.Amount);

		// check if transaction is new or will be edited
		// difference is: If its edited, then you must check the difference between the amounts
		Transaction? exists = GetTransaction(transaction.Id);
		if (exists == null) {
			// New Transaction was created
			return (total - totalSell - transaction.Amount) >= 0;
		}

		decimal diff = exists.Amount - transaction.Amount;
		return diff > 0 || // less than before, nothing to check
			(total - totalSell) >= Math.Abs(diff);
	}

	/// <inheritdoc />
	public bool ValidateTransactionDate(Transaction transaction) {
		// validate date
		DateTime current = DateTime.Now;
		return transaction.Date <= current;
	}

	/// <inheritdoc />
	public bool ValidateTransactionBalanceDelete(Transaction transaction, string userid) {
		if (transaction.Type.ToString() == "Sell") {
			return true;
		}

		// total = buy + mining + staking
		decimal total = GetTransactions(userid).Where(item => (item.Type == TransactionType.Buy || item.Type == TransactionType.Mining || item.Type == TransactionType.Staking) && item.CurrencyId == transaction.CurrencyId).Sum(item => item.Amount);
		decimal totalSell = GetTransactions(userid).Where(item => item.Type == TransactionType.Sell && item.CurrencyId == transaction.CurrencyId).Sum(item => item.Amount);

		return totalSell == 0 || total - transaction.Amount > totalSell;
	}
}
