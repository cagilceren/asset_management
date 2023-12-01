namespace NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails {
	/// <summary>
	/// This is a viewmodel consisting yearly total fee, tax-free and tax-liable loss, profit from sales and profit from others.
	///
	/// author: Cagil Ceren Aslan
	/// </summary>
	public class SaleTaxDetailsViewModel {

		/// <summary>
		/// The relevant year for the tax details.
		/// </summary>
		/// <value></value>
		public int Year { get; set; }

		/// <summary>
		/// Total amount of tax liable fee in a year.
		/// </summary>
		/// <value></value>
		public decimal TaxLiableFee { get; set; }

		/// <summary>
		/// Total amount of tax free fee in a year.
		/// </summary>
		/// <value></value>
		public decimal TaxFreeFee { get; set; }

		/// <summary>
		/// Total amount of tax liable profits from sales in a year
		/// </summary>
		/// <value></value>
		public decimal TaxLiableSaleProfit { get; set; }

		/// <summary>
		/// Total amount of tax free profits from sales in a year
		/// </summary>
		/// <value></value>
		public decimal TaxFreeSaleProfit { get; set; }

		/// <summary>
		/// Total amount of tax liable loss from sales in a year
		/// </summary>
		/// <value></value>
		public decimal TaxLiableSaleLoss { get; set; }

		/// <summary>
		/// Total amount of tax free loss from sales in a year
		/// </summary>
		/// <value></value>
		public decimal TaxFreeSaleLoss { get; set; }

	}
}
