namespace NACTAM.ViewModels.TaxRecommendation.ProfitChart {
	/// <summary>
	/// This is a view model consisting a dataset for a chart.js to show the profit from others (except sale).
	/// </summary>
	public class OthersProfitChartViewModel {

		/// <summary>
		/// Total amount of profit from mining
		/// </summary>
		/// <value></value>
		public decimal MiningProfit { get; set; }

		/// <summary>
		/// Total amount of profit from staking
		/// </summary>
		/// <value></value>
		public decimal StakingProfit { get; set; }

		/// <summary>
		/// Total amount of profit from non crypto other resources (except sale)
		/// </summary>
		/// <value></value>
		public decimal NonCryptoOthersProfit { get; set; }

		/// <summary>
		/// Shows the free limit of the profit from other resources (except sale)
		/// </summary>
		/// <value></value>
		public decimal FreeLimitOthersProfit { get; set; }
	}
}