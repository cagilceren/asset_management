namespace NACTAM.ViewModels.TaxEvaluation {

	/// <summary>
	/// This is a view model for the Waterflow chart data on TaxEvaluation view
	/// </summary>
	/// <remarks>
	/// <seealso cref="WaterFlowChartDataViewModel"/>
	/// </remarks>
	/// <author> Mervan Kilic </author>
	public class WaterFlowChartDataViewModel {
		/// <summary>
		/// buydate of a transaction
		/// </summary>
		public DateTime BeginDate { get; set; }


		/// <summary>
		/// selldate of a transaction
		/// </summary>
		public DateTime EndDate { get; set; }


		/// <summary>
		/// amount of transaction
		/// </summary>
		public decimal Amount { get; set; }

	}
}