using NACTAM.ViewModels.TaxRecommendation.RecommendationData;

namespace NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails {
	/// <summary>
	/// Represents the buy transactions after the coins of the sell transactions are reduced from the coins of all buy transactions.
	/// </summary>
	/// <remarks>
	/// They are the buy transactions that contain a quantity of coins that can be sold currently.
	/// </remarks>
	/// <author> Cagil Ceren Aslan </author>
	public class AfterSaleBuyTransactionViewModel {

		/// <summary>
		/// Shows the date of the transaction.
		/// </summary>
		/// <value></value>
		public DateTime Date { get; set; }

		/// <summary>
		/// Shows the rate of the coins as they were bought. (Euro - Crypto Currency).
		/// </summary>
		/// <value></value>
		public decimal Rate { get; set; }

		/// <summary>
		/// Shows the current rate of the coins (Euro - Crypto Currency).
		/// </summary>
		/// <value></value>
		public decimal CurrentRate { get; set; }


		/// <summary>
		/// Shows the type of the transaction. In this case it must be "Buy".
		/// </summary>
		/// <value></value>
		public string Type { get; set; }


		/// <summary>
		/// Shows the crypto currency of the transaction.
		/// </summary>
		/// <value></value>
		public string CryptoCurrency { get; set; }


		/// <summary>
		/// Shows the rest fee of the transaction.
		/// </summary>
		/// <value></value>
		public decimal RemainingFee { get; set; }


		/// <summary>
		/// Shows the rest amount of the coins with this transaction.
		/// </summary>
		/// <value></value>
		public decimal RemainingAmount { get; set; }

		/// <summary>
		/// Shows when the transaction is tax free.
		/// </summary>
		/// <value></value>
		public DateTime DueDate { get; set; }

		/// <summary>
		/// Shows if the transaction is tax free.
		/// </summary>
		/// <value></value>
		public bool IsTaxFree { get; set; }

		/// <summary>
		/// It is the expected gain, if all coins of the "AfterSaleBuyTransaction" is sold right now.
		/// </summary>
		/// <remarks>
		/// Gain refers to an amount of money earned from a sale. Fee from sale is NOT reduced from this amount.
		/// Since the sell-fee is not known.
		/// </remarks>
		/// <value></value>
		public decimal ExpectedGain { get; set; }

		/// <summary>
		/// Contains advices and tipps for user to increase the profit.
		/// </summary>
		/// <value></value>
		public TippsEnum Tipps { get; set; }


		/// <summary>
		/// Constructor for cref="AfterSaleBuyTransactionViewModel"
		/// </summary>
		public AfterSaleBuyTransactionViewModel() {
			IsTaxFree = false;
		}
	}
}