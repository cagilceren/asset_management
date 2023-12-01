using NACTAM.Models.TaxRecommendation;

namespace NACTAM.UnitTests {
	/// <summary>
	/// Tests all functions in <see cref="GermanTaxRule"/>
	/// </summary>
	/// <author> Cagil Ceren Aslan </author>
	public class GermanTaxRuleTest {
		public GermanTaxRule GermanTaxRule = new();

		/// <summary>
		/// Checks if it calculates sale-profit allowance limit according to german tax rules.
		/// </summary>
		[Fact]
		public void TestCalculateSaleProfitAllowanceLimit() {
			Assert.Equal(600 - 200, GermanTaxRule.CalculateSaleProfitAllowanceLimit(200));
		}

		/// <summary>
		/// Checks if it calculates profit allowance limit from other profits (not from sales) according to german tax rules.
		/// </summary>
		[Fact]
		public void TestCalculateOthersProfitAllowanceLimit() {
			Assert.Equal(256 - 200, GermanTaxRule.CalculateOthersProfitAllowanceLimit(200));
		}


		/// <summary>
		/// Checks if the function calculates the beginning and end of a given year properly.
		/// </summary>
		[Fact]
		public void TestCalculateTaxYearBeginnAndEndDate() {
			int year = 2023;
			DateTime beginnDate = new(2023, 1, 1);
			DateTime endDate = new(2023, 12, 31);
			var dateToTest = Tuple.Create(beginnDate, endDate);
			Assert.Equal(dateToTest, GermanTaxRule.CalculateTaxYearBeginnAndEndDate(year));
		}

		/// <summary>
		/// Checks if the function really checks if a sell transaction is performed after 365 days after a buy transaction or not.
		/// <remarks>
		/// In the test the year 2000 is used. 2000 is a lap year and year 2000 has 366 days. German date limit for tax is 365 days.
		/// </remarks>
		/// </summary>
		[Fact]
		public void TestIsTaxFreeByDateLimit() {
			DateTime buyDate = new(2000, 1, 1);
			DateTime sellDateLiable = new(2000, 12, 12);
			DateTime sellDateFree = new(2001, 1, 1);

			Assert.True(GermanTaxRule.IsTaxFreeByDateLimit(buyDate, sellDateFree));
			Assert.Throws<Exception>(() => GermanTaxRule.IsTaxFreeByDateLimit(sellDateFree, buyDate));
			Assert.False(GermanTaxRule.IsTaxFreeByDateLimit(buyDate, sellDateLiable));
		}


		/// <summary>
		/// Checks if the function returns the due date exactly after 365 days of a given buy-transaction date.
		/// <remarks>
		/// In the test the year 2000 is used. 2000 is a lap year and year 2000 has 366 days.
		/// German date limit for tax is 365 days. On 366. day it is tax free.
		/// </remarks>
		/// </summary>
		[Fact]
		public void TestCalculateDueDateByDateLimit() {
			DateTime buyDate = new(2000, 1, 1);
			DateTime dueDate = new(2000, 12, 31);

			Assert.Equal(dueDate, GermanTaxRule.CalculateDueDateByDateLimit(buyDate));
		}
	}
}