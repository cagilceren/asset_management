namespace NACTAM.ViewModels.TaxRecommendation.RecommendationData {
	/// <summary>
	/// Enumeration for profit optimizing tipps.
	/// </summary>
	/// <author> Cagil Ceren Aslan </author>
	public enum TippsEnum {

		/// <summary>
		/// Tipp for a tax liable transaction, which could cause loss, if it is sold.
		/// </summary>
		lossTaxLiableTipp,

		/// <summary>
		/// Tipp for a tax liable transaction, which could cause profit, if it is sold.
		/// </summary>
		profitTaxLiableTipp,

		/// <summary>
		/// Tipp for a tax free transaction, which could cause loss, if it is sold.
		/// </summary>
		lossTaxFreeTipp,

		/// <summary>
		/// Tipp for a tax free transaction, which could cause loss, if it is sold.
		/// </summary>
		profitTaxFreeTipp
	}
}