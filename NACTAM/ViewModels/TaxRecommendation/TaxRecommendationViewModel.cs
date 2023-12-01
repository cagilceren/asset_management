using NACTAM.ViewModels.TaxRecommendation.ProfitChart;
using NACTAM.ViewModels.TaxRecommendation.RecommendationData;
using NACTAM.ViewModels.TaxRecommendation.RecommendationData.RecommendationChart;

namespace NACTAM.ViewModels.TaxRecommendation {
	/// <summary>
	/// It is a view model to create the data which is used in TaxRecommendation view.
	/// </summary>
	/// <remarks>
	/// <seealso cref="TaxRecommendation"/>
	/// </remarks>
	/// <author> Cagil Ceren Aslan </author>
	public class TaxRecommendationViewModel {

		/// <summary>
		/// Constructor for TaxRecommendationViewModel
		/// </summary>
		public TaxRecommendationViewModel() {
		}

		/// <summary>
		/// User input for the profit from non crypto sales.
		/// </summary>
		/// <value></value>
		public decimal NonCryptoSaleProfit { get; set; }

		/// <summary>
		/// User input for the profit from non crypto other resources (except sale).
		/// </summary>
		/// <value></value>
		public decimal NonCryptoOthersProfit { get; set; }

		/// <summary>
		/// The data for a Chart.js chart displays the amount of profit from earned other resources (except sale) and the available free limit.
		/// </summary>
		/// <value></value>
		public OthersProfitChartViewModel OthersProfitChart { get; set; }

		/// <summary>
		/// The data for a Chart.js chart displays the amount of profit earned from sales and the available free limit.
		/// </summary>
		/// <value></value>
		public SaleProfitChartViewModel SaleProfitChart { get; set; }

		/// <summary>
		/// It is a combined version of RecommendationDataAssetTableViewModel and RecommendationAssetChartViewModel
		/// </summary>
		/// <remarks>
		/// <seealso cref="RecommendationDataAssetTableViewModel"/>
		/// <seealso cref="RecommendationAssetChartViewModel"/>
		/// </remarks>
		/// <value></value>
		public RecommendationDataViewModel RecommendationData { get; set; }
	}
}