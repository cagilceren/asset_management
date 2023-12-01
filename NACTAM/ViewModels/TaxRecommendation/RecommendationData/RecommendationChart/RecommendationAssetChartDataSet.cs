namespace NACTAM.ViewModels.TaxRecommendation.RecommendationData.RecommendationChart {
	/// <summary>
	/// This is a vew model to create a data for chart.js.
	/// </summary>
	/// <remarks>
	/// <seealso cref="TaxRecommendation"/>
	/// </remarks>
	/// <author> Cagil Ceren Aslan </author>
	public class RecommendationAssetChartDataSet {

		/// <summary>
		/// Constructor for RecommendationAssetChartDataSet
		/// </summary>
		public RecommendationAssetChartDataSet() {
			Data = new();
		}

		/// <summary>
		/// Data format of chart.js
		/// </summary>
		/// <value></value>
		public List<decimal> Data { get; set; }
	}
}