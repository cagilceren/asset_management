using System.ComponentModel.DataAnnotations;

namespace NACTAM.Models {


	/// <summary>
	/// TransactionType for creating a Transaction
	/// </summary>
	public enum TransactionType {
		/// <summary>
		/// Indicates that the transaction is a buy transaction
		/// </summary>
		[Display(Name = "Kauf")] Buy,
		/// <summary>
		/// Indicates that the transaction is a sell transaction
		/// </summary>
		[Display(Name = "Verkauf")] Sell,
		/// <summary>
		/// Indicates that the transaction is a mining transaction
		/// </summary>
		[Display(Name = "Mining")] Mining,
		/// <summary>
		/// Indicates that the transaction is a staking transaction
		/// </summary>
		[Display(Name = "Staking")] Staking
	}
}
