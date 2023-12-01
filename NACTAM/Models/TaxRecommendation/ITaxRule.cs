namespace NACTAM.Models.TaxRecommendation {
	/// <summary>
	/// Represents an interface for the tax rules, that are used for profit and loss calculations and for tax recommendations.
	/// </summary>
	/// <author> Cagil Ceren Aslan </author>
	public interface ITaxRule {

		/// <summary>
		/// Reduces the profits earned from sales from the max allowed tax-free sale-profit limit.
		/// </summary>
		/// <param name="profit"></param>
		/// <returns>remaining tax-free sale-profit.</returns>
		decimal CalculateSaleProfitAllowanceLimit(decimal profit);

		/// <summary>
		/// Reduces the profits earned from other services from the max allowed tax-free others-profit limit.
		/// </summary>
		/// <param name="profit"></param>
		/// <returns>remaining tax-free others-profit.</returns>
		decimal CalculateOthersProfitAllowanceLimit(decimal profit);

		/// <summary>
		/// Calculates the tax year of a given year.
		/// </summary>
		/// <param name="year"></param>
		/// <returns>the beginning and ending dates of the tax year as tuple.</returns>
		Tuple<DateTime, DateTime> CalculateTaxYearBeginnAndEndDate(int year);

		/// <summary>
		/// Checks if the date of sale is old enough from the date of buy, so that profit from sale is tax-free.
		/// </summary>
		/// <param name="buyDate"></param>
		/// <param name="sellDate"></param>
		/// <returns>tax-free or not tax-free as true or false</returns>
		Boolean IsTaxFreeByDateLimit(DateTime buyDate, DateTime sellDate);

		/// <summary>
		/// Calculates when a buy date will be old enough, so that profit from sale will be tax-free.
		/// </summary>
		/// <param name="buyDate"></param>
		/// <returns>the due date as string</returns>
		DateTime CalculateDueDateByDateLimit(DateTime buyDate);
	}
}