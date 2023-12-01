namespace NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails {
	/// <summary>
	/// Represents the sell transactions after the coins of the sell transactions are reduced from the coins of all buy transactions.
	/// </summary>
	/// <remarks>
	/// This sell transactions do not match with normal sell transaction from user.
	/// Whenever the coins of a sell transaction are subtracted from the coins of a buy transaction,
	/// an cref="AfterSaleSellTransactionViewModel" object is created.
	/// </remarks>
	/// <author> Cagil Ceren Aslan </author>
	public class AfterSaleSellTransactionViewModel {

		/// <summary>
		/// Shows the sell date of the transaction.
		/// </summary>
		/// <value></value>
		public DateTime SellDate { get; set; }

		/// <summary>
		/// Shows the buy date of the transaction.
		/// </summary>
		/// <value></value>
		public DateTime BuyDate { get; set; }

		/// <summary>
		/// Shows the rate of the coins as they were sold. (Euro - Crypto Currency).
		/// </summary>
		/// <value></value>
		public decimal SellRate { get; set; }

		/// <summary>
		/// Shows the rate of the coins as they were bought. (Euro - Crypto Currency).
		/// </summary>
		/// <value></value>
		public decimal BuyRate { get; set; }


		/// <summary>
		/// Shows the type of the transaction. In this case it must be "Sell".
		/// </summary>
		/// <value></value>
		public string Type { get; set; }


		/// <summary>
		/// Shows the crypto currency of the transaction.
		/// </summary>
		/// <value></value>
		public string CryptoCurrency { get; set; }


		/// <summary>
		/// Shows the total fee of the transaction.
		/// </summary>
		/// <remarks>
		/// Total Fee is the sum of the fee from buying and the fee from selling.
		/// Fees from buying are calculated according to number of coins sold.
		/// </remarks>
		/// <value></value>
		public decimal TotalFee { get; set; }


		/// <summary>
		/// Shows the sold amount of the coins with this transaction.
		/// </summary>
		/// <value></value>
		public decimal SoldAmount { get; set; }


		/// <summary>
		/// Shows if the transaction is tax free.
		/// </summary>
		/// <value></value>
		public bool IsTaxFree { get; set; }

		/// <summary>
		///
		/// </summary>
		/// <remarks>
		/// Gain refers to an amount of money earned from a sale. Fee is NOT reduced from this amount.
		/// </remarks>
		/// <value></value>
		public decimal Gain { get; set; }

		/// <summary>
		/// Constructor for cref="AfterSaleSellTransactionViewModel"
		/// </summary>
		public AfterSaleSellTransactionViewModel() {
			IsTaxFree = false;
		}

	}
}