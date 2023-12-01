namespace NACTAM.ViewModels.TaxRecommendation.ProfitChart {
	/// <summary>
	/// This is a view model consisting a dataset for a chart.js to show the profit from sale.
	/// </summary>
	public class SaleProfitChartViewModel {
		/// <summary>
		/// Total amount of profit from crypto sales.
		/// </summary>
		/// <value></value>
		public decimal CryptoSaleProfit { get; set; }

		/// <summary>
		/// Total amount of profit from non crypto sales.
		/// </summary>
		/// <value></value>
		public decimal NonCryptoSaleProfit { get; set; }

		/// <summary>
		/// Shows the free limit of the profit from sales.
		/// </summary>
		/// <value></value>
		public decimal FreeLimitSaleProfit { get; set; }
	}
}