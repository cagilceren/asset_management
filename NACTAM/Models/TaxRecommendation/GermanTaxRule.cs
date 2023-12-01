namespace NACTAM.Models.TaxRecommendation {
	/// <summary>
	/// Represents german tax rules, that are used for profit and loss calculations and for tax recommendations.
	/// </summary>
	/// <seealso cref="ITaxRule"/>
	/// <author> Cagil Ceren Aslan </author>
	public class GermanTaxRule : ITaxRule {

		/// <summary>
		/// The beginning month of a german tax year.
		/// </summary>
		private readonly int _beginnMonth = 1;

		/// <summary>
		/// The end month of a german tax year.
		/// </summary>
		private readonly int _endMonth = 12;

		/// <summary>
		/// The beginning day of a german tax year.
		/// </summary>
		private readonly int _beginnDay = 1;

		/// <summary>
		///  The end day of a german tax year.
		/// </summary>
		private readonly int _endDay = 31;

		/// <summary>
		/// German day limit. The profit that comes from the coins that are older than this day limit is tax-free.
		/// </summary>
		private readonly int _dayLimit = 365;

		/// <summary>
		/// Limit of yearly profit earned from sales. If total amount of profit in a german tax year is more than this limit, it will be tax-liable.
		/// </summary>
		private readonly decimal _sellProfitLimit = 600.0M;

		/// <summary>
		/// Limit of yearly profit earned from other services (andere Leistungen in German). If total amount of profit in a german tax year is more than this limit, it will be tax-liable.
		/// </summary>
		private readonly decimal _othersProfitLimit = 256.0M;


		/// <inheritdoc/>
		public decimal CalculateSaleProfitAllowanceLimit(decimal profit) {
			return _sellProfitLimit - profit;
		}

		/// <inheritdoc/>
		public decimal CalculateOthersProfitAllowanceLimit(decimal profit) {
			return _othersProfitLimit - profit;
		}


		/// <inheritdoc/>
		public Tuple<DateTime, DateTime> CalculateTaxYearBeginnAndEndDate(int year) {
			DateTime beginnDate = new(year, _beginnMonth, _beginnDay);
			DateTime endDate = new(year, _endMonth, _endDay);
			return Tuple.Create(beginnDate, endDate);
		}

		/// <inheritdoc/>
		public Boolean IsTaxFreeByDateLimit(DateTime buyDate, DateTime sellDate) {
			if (buyDate > sellDate) {
				throw new Exception("Buy date can not be older than sell date!");
			}
			int ageInDays = (sellDate - buyDate).Days;
			return (_dayLimit - ageInDays) < 0;
		}

		/// <inheritdoc/>
		public DateTime CalculateDueDateByDateLimit(DateTime buyDate) {
			DateTime dueDate = buyDate.AddDays(_dayLimit);
			return dueDate;
		}
	}
}
