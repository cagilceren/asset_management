using NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails;

namespace NACTAM.ViewModels.TaxRecommendation.RecommendationData {
	/// <summary>
	/// This is a view model to create a data for the datatable in TaxRecommendation view.
	/// </summary>
	/// <remarks>
	/// <seealso cref="TaxRecommendation"/>
	/// </remarks>
	/// <author> Cagil Ceren Aslan </author>
	public class RecommendationDataAssetTableViewModel {

		/// <summary>
		/// Constructor for RecommendationDataAssetTableViewModel
		/// </summary>
		public RecommendationDataAssetTableViewModel() {
			Transactions = new();
			IsTaxFree = false;
		}

		/// <summary>
		/// Shows amount of avaliable coins in an asset
		/// </summary>
		/// <value></value>
		public decimal AvaliableCoinAmount { get; set; }

		/// <summary>
		/// Shows if an asset is tax free.
		/// </summary>
		/// <value></value>
		public bool IsTaxFree { get; set; }

		/// <summary>
		/// Shows crypto currency of an asset.
		/// </summary>
		/// <value></value>
		public string CryptoCurrency { get; set; }

		/// <summary>
		/// Shows a list of transactions of an asset.
		/// </summary>
		/// <value></value>
		public List<AfterSaleBuyTransactionViewModel> Transactions { get; set; }

	}
}