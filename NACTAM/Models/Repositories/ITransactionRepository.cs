using NACTAM.ViewModels;

namespace NACTAM.Models.Repositories {

	/// <summary>
	/// This interface provides a layer of abstraction between the TransactionContainer and the rest of the application. It is used to load transactions, update attributes, and delete transactions.
	/// </summary>
	public interface ITransactionRepository {
		/// <summary>
		/// This method returns all transactions from certain user
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="userid">The id from user</param>
		/// <returns>A List of all Transactions</returns>
		public IEnumerable<Transaction> GetTransactions(string userid);
		/// <summary>
		/// This method returns transaction with certain id from user
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="id">The id from transaction</param>
		/// <param name="userid">The id from user</param>
		/// <returns>A single Transaction from the specified user</returnes>
		public Transaction GetTransaction(int id, string userId);
		/// <summary>
		/// This method returns all deleted transactions from submitted user
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="userid">The id from user</param>
		/// <returns>A List of all Transactions with attribut Deleted=true</returns>
		public IEnumerable<Transaction> GetDeletedTransactions(string userid);
		/// <summary>
		/// This method updates the submitted transaction and saves it into database
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="userid">The id from user</param>
		/// <param name="transaction">Desired transaction to update</param> 
		public Task UpdateTransaction(string userid, Transaction transaction);
		/// <summary>
		/// This method changes attribut of transaction with submitted id to Deleted = true (Softdelete) 
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="userid">The id from user</param>
		/// <param name="id">The id from transaction</param> 
		public Task RemoveTransaction(string userid, int id);
		/// <summary>
		/// This method adds submitted transaction into database and autogenerates its id
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="userid">The id from user</param>
		/// <param name="id">Desired transaction to add</param> 
		public Task AddTransaction(string userid, Transaction transaction);
		/// <summary>
		/// This method checks whether negative assets can arise when deleting a transaction
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="transaction">Desired transaction to validate</param>
		/// <param name="userid">The id from user</param>
		/// <returns>boolean on success is true and false otherwise</returns>
		public bool ValidateTransactionBalanceDelete(Transaction transaction, string userid);
		/// <summary>
		/// This method checks before transactions is created and if type is sell, then checks if user has already 
		/// bought the coins, he wants to sell
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="transaction">Desired transaction to validate</param>
		/// <param name="userid">The id from user</param>
		/// <returns>boolean on success is true and false otherwise</returns>
		public bool ValidateTransactionBalance(Transaction transaction, string userid);
		/// <summary>
		/// This method checks if date is in future
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="transaction">Desired transaction to validate</param>
		/// <returns>If date is in future it returnes false, otherwise true</returns>
		public bool ValidateTransactionDate(Transaction transaction);
	}
}
