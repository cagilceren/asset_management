using NACTAM.ViewModels.TaxRecommendation.RecommendationData.RecommendationChart;

namespace NACTAM.ViewModels.TaxRecommendation.RecommendationData {
	/// <summary>
	/// This is a view model to create data for TaxRecommendation view.
	/// </summary>
	/// <remarks>
	/// <seealso cref="TaxRecommendation"/>
	/// </remarks>
	/// <author> Cagil Ceren Aslan </author>
	public class RecommendationDataViewModel {

		/// <summary>
		/// Constructor for RecommendationDataViewModel.
		/// </summary>
		public RecommendationDataViewModel() {
			AssetsForTable = new List<RecommendationDataAssetTableViewModel>();
			ChartDataSets = new List<RecommendationAssetChartViewModel>();
		}

		/// <summary>
		/// A list of assets for the datatable in TaxRecommendation view.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxRecommendation"/>
		/// </remarks>
		/// <value></value>
		public List<RecommendationDataAssetTableViewModel> AssetsForTable { get; set; }

		/// <summary>
		/// A list of assets for the charts of chart.js in TaxRecommendation view.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxRecommendation"/>
		/// </remarks>
		/// <value></value>
		public List<RecommendationAssetChartViewModel> ChartDataSets { get; set; }
	}
}
