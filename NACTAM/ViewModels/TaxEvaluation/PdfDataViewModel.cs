using NACTAM.Controllers;
using NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails;
namespace NACTAM.ViewModels.TaxEvaluation {

	/// <summary>
	/// This is a view model for all the data on pdf.
	/// </summary>
	/// <remarks>
	/// <seealso cref="PdfDataViewModel"/>
	/// </remarks>
	/// <author> Mervan Kilic </author>
	public class PdfDataViewModel {


		/// <summary>
		/// Profit for pdf
		/// </summary>
		public decimal Profit { get; set; }

		/// <summary>
		/// Loss for pdf
		/// </summary>
		public decimal Loss { get; set; }

		/// <summary>
		/// Fee for pdf
		/// </summary>
		public decimal Fee { get; set; }

		/// <summary>
		/// Year for pdf
		/// </summary>
		public int Year { get; set; }


		/// <summary>
		/// Limit for taxfree profit by mining and stacking for pdf
		/// </summary>
		public decimal OthersProfitLimit { get; set; }


		/// <summary>
		/// Limit for taxfree profit by sales for pdf
		/// </summary>
		public decimal SellProfitLimit { get; set; }



		/// <summary>
		/// The date of creation for displaying on the pdf
		/// </summary>
		public DateTime CreationDate { get; set; }



		/// <summary>
		/// The mining and staking profits for displaying on the pdf
		/// </summary>
		public Tuple<decimal, decimal> ProfitMiningStaking { get; set; }


		/// <summary>
		/// profit from sales for displaying on the pdf
		/// </summary>
		public decimal ProfitFromSales { get; set; }



		/// <summary>
		/// Sale details calculated in ServicesController for displaying on the PDF.
		/// </summary>
		/// <remarks>
		/// <seealso cref="ServicesController"/>
		/// </remarks>
		public SaleTaxDetailsViewModel SaleTaxDetails { get; set; }



		/// <summary>
		/// A constructor for saving all relevant data for pdf generation
		/// </summary>
		/// <remarks>
		/// <param name="year">year for pdf</param>
		/// <param name="profit">profit for pdf</param>
		/// <param name="loss">loss for pdf</param>
		/// <param name="fee">fee for pdf</param>
		/// <param name="profitFromSales">profit from sales for pdf</param>
		/// <param name="profitMiningStaking">profit from Item1:staking and Item2:mining for pdf</param>
		/// <param name="sellProfitLimit">limit for sales for pdf</param>
		/// <param name="othersProfitLimit">limit for staking and mining for pdf</param>
		/// <param name="saleTaxDetails">taxfree and taxliable sale data for pdf</param>
		/// <seealso cref="PdfDataViewModel"/>
		/// </remarks>
		public PdfDataViewModel(int year, decimal profit, decimal loss, decimal fee, decimal profitFromSales, Tuple<decimal, decimal> profitMiningStaking, decimal sellProfitLimit, decimal othersProfitLimit, SaleTaxDetailsViewModel saleTaxDetails) {
			Year = year;
			Profit = profit;
			Loss = loss;
			Fee = fee;
			ProfitFromSales = profitFromSales;
			ProfitMiningStaking = profitMiningStaking;
			SaleTaxDetails = saleTaxDetails;
			SellProfitLimit = sellProfitLimit;
			OthersProfitLimit = othersProfitLimit;
			CreationDate = DateTime.Now;
		}

	}
}