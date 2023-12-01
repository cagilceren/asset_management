namespace NACTAM.ViewModels.TaxRecommendation.RecommendationData.RecommendationChart {
	/// <summary>
	/// It is a view model to create data for a chart.js.
	/// </summary>
	/// <remarks>
	/// <seealso cref="TaxRecommendation"/>
	/// </remarks>
	/// <author> Cagil Ceren Aslan </author>
	public class RecommendationAssetChartViewModel {

		/// <summary>
		/// Constructor for RecommendationAssetChartViewModel
		/// </summary>
		public RecommendationAssetChartViewModel() {
			Labels = new();
			DataSets = new();
		}

		/// <summary>
		/// A list of datasets for chart.js
		/// </summary>
		/// <value></value>
		public List<RecommendationAssetChartDataSet> DataSets { get; set; }

		/// <summary>
		/// A list of labels for the dataset for chart.js
		/// </summary>
		/// <value></value>
		public List<string> Labels { get; set; }

		/// <summary>
		/// The name of chart
		/// </summary>
		/// <value></value>
		public string ChartName { get; set; }

	}
}