namespace NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails {
	/// <summary>
	/// This is a viewmodel which consists of a list of AfterSaleBuyTransactions and AfterSaleSellTransactions
	/// <seealso cref="AfterSaleBuyTransactions"/>
	/// <seealso cref="AfterSaleSellTransactions"/>
	/// </summary>
	/// <author> Cagil Ceren Aslan </author>
	public class AfterSaleTaxDetailsViewModel {

		/// <summary>
		/// A list of AfterSaleBuyTransactions
		/// </summary>
		/// <value></value>
		public List<AfterSaleBuyTransactionViewModel>? AfterSaleBuyTransactions { get; set; }

		/// <summary>
		/// A list of AfterSaleSellTransactions
		/// </summary>
		/// <value></value>
		public List<AfterSaleSellTransactionViewModel>? AfterSaleSellTransactions { get; set; }


		/// <summary>
		/// Constructor for AfterSaleTaxDetailsViewModel.
		/// </summary>
		public AfterSaleTaxDetailsViewModel() {

		}
	}
}