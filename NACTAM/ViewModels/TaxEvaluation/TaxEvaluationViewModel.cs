
using NACTAM.Controllers;
using NACTAM.Models.TaxRecommendation;
using NACTAM.ViewModels.TaxEvaluation;
using NACTAM.ViewModels.TaxRecommendation;
using NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails;

namespace NACTAM.ViewModels.TaxEvaluation {

	public class TaxEvaluationViewModel {
		/// <summary>
		/// This is a view model for data and functions of TaxEvaluation view.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxEvaluationViewModel"/>
		/// </remarks>
		/// <author> Mervan Kilic </author>

		/// <summary>
		/// Total positive gain calculated by TaxCalculationsService.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxCalculationsService"/>
		/// </remarks>
		public decimal Profit { get; set; }


		/// <summary>
		/// Total negative gain calculated by TaxCalculationsService.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxCalculationsService"/>
		/// </remarks>
		public decimal Loss { get; set; }


		/// <summary>
		/// Total fee calculated by TaxCalculationsService.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxCalculationsService"/>
		/// </remarks>
		public decimal Fee { get; set; }


		/// <summary>
		/// The sum of every profit and loss of all transactions.
		/// </summary>
		public decimal ProfitFromSales { get; set; }


		/// <summary>
		/// The year selected by the user and handled by ServicesController.
		/// </summary>
		/// <remarks>
		/// <seealso cref="ServicesController"/>
		/// </remarks>
		public int Year { get; set; }


		/// <summary>
		/// The crypto-currency selected by the user and handled by ServicesController.
		/// </summary>
		/// <remarks>
		/// <seealso cref="ServicesController"/>
		/// </remarks>
		public string CryptoCurrency { get; set; }


		/// <summary>
		/// The date of creation for displaying on the PDF.
		/// </summary>
		public Tuple<int, int> TaxationCount { get; set; }


		/// <summary>
		/// Profit for Mining and Stacking calculated by TaxCalculationsService.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxCalculationsService"/>
		/// </remarks>
		public decimal ProfitMiningStaking { get; set; }


		/// <summary>
		/// Limit for taxfree profit by mining and stacking.
		/// </summary>
		public decimal OthersProfitLimit { get; set; }


		/// <summary>
		/// Limit for taxfree profit by sales.
		/// </summary>
		public decimal SellProfitLimit { get; set; }


		/// <summary>
		///	All data for pdf Generation
		///	</summary>
		public PdfDataViewModel PdfData { get; set; }


		/// <summary>
		/// readonly after-sale transactions 
		/// </summary>
		public readonly List<AfterSaleSellTransactionViewModel> Transactions;


		public List<WaterFlowChartDataViewModel> ChartData { get; set; }


		/// <summary>
		/// A list that contains all transactions grouped by crypto-currency for the dropdown list.
		/// </summary>
		public List<AfterSaleSellTransactionViewModel> CryptoCurrencyTransactions { get; set; }


		/// <summary>
		/// A list that contains all transactions grouped by year for the dropdown list.
		/// </summary>
		public List<AfterSaleSellTransactionViewModel> YearTransactions { get; set; }





		/// <summary>
		/// A constructor with no parameters for TaxEvaluationViewModel
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxEvaluationViewModel"/>
		/// </remarks>
		public TaxEvaluationViewModel() {

		}


		/// <summary>
		/// A constructor with with transactions parameter for TaxEvaluationViewModel
		/// </summary>
		/// <remarks>
		/// <param name="transactions">After sale transactions</param>
		/// <seealso cref="TaxEvaluationViewModel"/>
		/// </remarks>
		public TaxEvaluationViewModel(List<AfterSaleSellTransactionViewModel> transactions) {
			Transactions = transactions;
		}


		/// <summary>
		/// This function calculates the sum of profits, losses and fees. Call-by-Reference is used for the parameters.
		/// </summary>
		/// <remarks>
		/// <param name="sellTransactions">Calculated after-sale transactions</param>
		/// <param name="profit">a referenced attribute for summed up positiv gain</param>
		/// <param name="loss">a referenced attribute for summed up negativ gain</param>
		/// <param name="fee">a referenced attribute for summed up fees gain</param>
		/// <seealso cref="SumUp"/>
		/// </remarks>
		public static void SumUp(List<AfterSaleSellTransactionViewModel> sellTransactions, ref decimal profit, ref decimal loss, ref decimal fee) {
			if (sellTransactions != null) {
				foreach (var transaction in sellTransactions) {
					fee += transaction.TotalFee;
					if (transaction.Gain > 0) {
						profit += transaction.Gain;
					} else {
						loss += transaction.Gain * -1;
					}
				}

				profit = TaxEvaluationViewModel.FormatToEuro(profit);
				loss = TaxEvaluationViewModel.FormatToEuro(loss);
				fee = TaxEvaluationViewModel.FormatToEuro(fee);
			}
		}



		/// <summary>
		/// This function calculates the sum of transaction gains. Call-by-Reference is used for the parameter.
		/// </summary>
		/// <remarks>
		/// <param name="sellTransactions">Calculated after-sale transactions</param>
		/// <param name="profitFromSales">a referenced attribute for summed up total gain</param>
		/// <seealso cref="CalculateProfitFromSales"/>
		/// </remarks>
		public static void CalculateProfitFromSales(List<AfterSaleSellTransactionViewModel> sellTransactions, ref decimal profitFromSales) {
			if (sellTransactions != null) {
				foreach (var transaction in sellTransactions) {
					profitFromSales += transaction.Gain;
				}

				profitFromSales = TaxEvaluationViewModel.FormatToEuro(profitFromSales);
			}
		}



		/// <summary>
		/// This function counts the Amount of tax-free and tax-liable transactions.
		/// </summary>
		/// <remarks>
		/// <param name="transactions">calculated after-sale transactions</param>
		/// <param name="date">date for categorising the state of taxation</param>
		/// <param name="taxRule">the taxation rules that are used</param>
		/// <seealso cref="CountTaxFreeAndLiableSales"/>
		/// <returns>an integer Tuple object with: Item1=tax-liable amount; Item2=tax-free amount</returns>
		/// </remarks>
		public static Tuple<int, int> CountTaxFreeAndLiableSales(IEnumerable<AfterSaleSellTransactionViewModel> transactions, DateTime date, ITaxRule taxRule) {
			DateTime beginnDate = taxRule.CalculateTaxYearBeginnAndEndDate(date.Year).Item1;
			DateTime endDate = taxRule.CalculateTaxYearBeginnAndEndDate(date.Year).Item2;
			var newTransactions = transactions;

			int taxliable = newTransactions.Where(s => s.SellDate >= beginnDate && s.SellDate <= endDate).Count();
			int taxfree = newTransactions.Where(s => s.SellDate < beginnDate).Count();

			return Tuple.Create(taxliable, taxfree);
		}


		/// <summary>
		/// This function formats the given number to Euro with two decimals.
		/// </summary>
		/// <remarks>
		/// <param name="number">number for formatting</param>
		/// <seealso cref="FormatToEuro"/>
		/// <returns>number with max. two decimals</returns>
		/// </remarks>
		public static decimal FormatToEuro(decimal number) {
			number *= 100;
			number = Math.Round(number);
			return number /= 100;
		}

		/// <summary>
		/// This function combines all relevant data for a Waterflow chart generation.
		/// </summary>
		/// <remarks>
		/// <param name="transactions">transactions for chart generation</param>
		/// <seealso cref="GenerateChartData"/>
		/// <returns>List of ViewModels with chart data of every transaction</returns>
		/// </remarks>
		public static List<WaterFlowChartDataViewModel> GenerateChartData(List<AfterSaleSellTransactionViewModel> transactions) {

			return transactions.Select(t => new WaterFlowChartDataViewModel() {
				BeginDate = t.BuyDate,
				EndDate = t.SellDate,
				Amount = t.SoldAmount
			}).ToList();

		}


		/// <summary>
		/// This function filters the given transactions by year and crypto-currency.
		/// </summary>
		/// <remarks>
		/// <param name="transactions">number for formatting</param>
		/// <param name="year">number for formatting</param>
		/// <param name="cryptoCurrency">number for formatting</param>
		/// <seealso cref="Filter"/>
		/// <returns>filtered transactions</returns>
		/// </remarks>
		public static List<AfterSaleSellTransactionViewModel> Filter(List<AfterSaleSellTransactionViewModel> transactions, int year, string cryptoCurrency) {
			var newTransactions = transactions;
			return newTransactions.Where(t => ((t.SellDate.Year == year) && (t.CryptoCurrency == cryptoCurrency))).ToList<AfterSaleSellTransactionViewModel>();
		}


		/// <summary>
		/// This function filters the given transactions by year.
		/// </summary>
		/// <remarks>
		/// <param name="transactions">number for formatting</param>
		/// <param name="year">number for formatting</param>
		/// <seealso cref="Filter"/>
		/// <returns>filtered transactions</returns>
		/// </remarks>
		public static List<AfterSaleSellTransactionViewModel> Filter(List<AfterSaleSellTransactionViewModel> transactions, int year) {
			var newTransactions = transactions;
			return newTransactions.Where(t => (t.SellDate.Year == year)).ToList<AfterSaleSellTransactionViewModel>();
		}


		/// <summary>
		/// This function groups the transactions by crypto-currency.
		/// </summary>
		/// <remarks>
		/// <param name="transactions">after-sale transactions"/>
		/// <returns>grouped transactoins by crypto-currencie</returns>
		/// </remarks>
		public static List<AfterSaleSellTransactionViewModel> ListedCurrencies(List<AfterSaleSellTransactionViewModel> transactions) {
			var listedTransactions = transactions;
			return listedTransactions.GroupBy(t => t.CryptoCurrency).Select(t => t.First()).ToList<AfterSaleSellTransactionViewModel>();
		}


		/// <summary>
		/// This function groups the transactions by year.
		/// </summary>
		/// <remarks>
		/// <param name="transactions">after-sale transactions</param>
		/// <seealso cref="ListedYears"/>
		/// <returns>grouped transactoins by year</returns>
		/// </remarks>
		public static List<AfterSaleSellTransactionViewModel> ListedYears(List<AfterSaleSellTransactionViewModel> transactions) {
			var listedTransactions = transactions;
			return listedTransactions.GroupBy(t => t.SellDate.Year).Select(t => t.First()).ToList<AfterSaleSellTransactionViewModel>();
		}
	}
}