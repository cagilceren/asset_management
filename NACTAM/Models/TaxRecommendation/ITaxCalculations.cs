using NACTAM.ViewModels;
using NACTAM.ViewModels.TaxEvaluationNew;
using NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails;
using NACTAM.ViewModels.TaxRecommendation.ProfitChart;
using NACTAM.ViewModels.TaxRecommendation.RecommendationData;
using NACTAM.ViewModels.TaxRecommendation.RecommendationData.RecommendationChart;

namespace NACTAM.Models.TaxRecommendation {

	/// <summary>
	/// Interface that does all tax related calculations.
	///
	/// author: Cagil Ceren Aslan
	/// </summary>
	/// <remarks>
	/// <seealso cref="TaxCalculationsService"/>
	/// </remarks>

	public interface ITaxCalculations {

		/// <summary>
		/// Generates data for the chart of profit from other services (german: andere Leistungen).
		/// </summary>
		/// <remarks>
		/// Calculates all profit from staking and mining of desired year. <seealso cref="CalculateProfitFromMiningAndStaking"/>
		/// Includes non crypto profit from other services, that user gives as an input.
		/// Calculates the remaining tax-free profit limit.
		/// <seealso cref="GermanTaxRule.CalculateOthersProfitAllowanceLimit"/>
		/// </remarks>
		/// <param name="cryptoOthersProfit"></param>
		/// <param name="NonCryptoOthersProfit">It is an user input, if not given, its value is 0.</param>
		/// <returns>A ViewModel <seealso cref="OthersProfitChartViewModel"/> that has profit from mining and staking and non crypto profits (user input) and the remaining tax-free profit limit.</returns>
		public OthersProfitChartViewModel GenerateOthersProfitChart((decimal, decimal) cryptoOthersProfit, decimal NonCryptoOthersProfit = 0);

		/// <summary>
		/// Calculates mining and staking profit of a desired year
		///
		/// author: Tuan Bui
		/// </summary>
		/// <param name="transactions">All relevant transactions</param>
		/// <param name="year"></param>
		/// <returns>A tuple of profit as miningProfit and stakingProfit</returns>
		public (decimal, decimal) CalculateProfitFromMiningAndStaking(List<TransactionsViewModel> transactions, int year);

		/// <summary>
		/// Calculates the income and expenditures of a desired year
		///
		/// author: Tuan Bui
		/// </summary>
		/// <param name="transactions">All relevant transactions</param>
		/// <param name="year"></param>
		/// <returns>A tuple of income and expenditures</returns>
		public (decimal, decimal) CalculateIncomeAndExpenditures(List<TransactionsViewModel> transactions, int year);

		/// <summary>
		/// Calculates the tax relevant income and expenditures of a desired year
		/// </summary>
		/// <param name="aftersaletaxdetails">All relevant transactions</param>
		/// <returns>A tuple of income and expenditures</returns>
		public (decimal, decimal) CalculateTaxRelevantIncomeAndExpenditures(AfterSaleTaxDetailsViewModel aftersaletaxdetails);


		/// <summary>
		/// Generates data for the chart of profit from sales (german: Verkäufe).
		/// </summary>
		/// <remarks>
		/// Calculates all profit earned from sales of desired year. <seealso cref="GenerateSaleTaxDetails"/>
		/// Includes non crypto profit from sales, that user gives as an input.
		/// Calculates the remaining tax-free profit limit.
		/// <seealso cref="GermanTaxRule.CalculateSaleProfitAllowanceLimit"/>
		/// </remarks>
		/// <param name="saleTaxDetails"></param>
		/// <param name="NonCryptoSaleProfit">It is an user input, if not given, its value is 0.</param>
		/// <returns>A ViewModel <seealso cref="SaleProfitChartViewModel"/> that has profit from sales and non crypto profits (user input) and the remaining tax-free profit limit.</returns>
		public SaleProfitChartViewModel GenerateSaleProfitChart(SaleTaxDetailsViewModel saleTaxDetails, decimal NonCryptoSaleProfit = 0);

		/// <summary>
		/// Generates a viewmodel <seealso cref="SaleTaxDetailsViewModel"/> representing sale related tax details of a desired year.
		/// </summary>
		/// <remarks>
		/// Sale related tax details consists of all sale related tax-free and tax-liable loss and profit, and total amount of fee existing in sales of a desired year and
		/// <seealso cref="GermanTaxRule.CalculateTaxYearBeginnAndEndDate"/>
		/// <seealso cref="CalculateAfterSaleTaxDetails"/>
		/// </remarks>
		/// <param name="afterSaleTaxDetails"></param>
		/// <param name="year"></param>
		/// <returns>a <seealso cref="SaleTaxDetailsViewModel"/> object.</returns>
		public SaleTaxDetailsViewModel GenerateSaleTaxDetails(AfterSaleTaxDetailsViewModel afterSaleTaxDetails, int year);

		/// <summary>
		/// Generates a viewmodel <seealso cref="SaleTaxDetailsViewModel"/> representing sale related tax details of a desired year. Unlike <seealso cref="GenerateSaleTaxDetails"/>, it includes the fee of unsold coins.
		///
		/// author: Tuan Bui
		/// </summary>
		/// <remarks>
		/// <seealso cref="GenerateSaleTaxDetails"/>
		/// </remarks>
		/// <param name="afterSaleTaxDetails"></param>
		/// <param name="year"></param>
		/// <returns>a <seealso cref="SaleTaxDetailsViewModel"/> object.</returns>
		public SaleTaxDetailsViewModel GenerateAllTaxDetails(AfterSaleTaxDetailsViewModel afterSaleTaxDetails, int year);

		/// <summary>
		/// Calculates tax details after sale.
		/// </summary>
		/// <remarks>
		/// Seperates sell and buy transactions.
		/// Reduces the amount of sold coins and their fee relatively from buy transactions based on FIFO principle, and saves them as <seealso cref="AfterSaleBuyTransactionViewModel"/>.
		/// At the same time creates new pseudo "sell transactions", <seealso cref="AfterSaleSellTransactionViewModel"/> containing gain, isTaxFree, soldAmount, Currency and Fee informations.
		/// If a sell transaction sells coins from more than one buy transactions, an <seealso cref="AfterSaleSellTransactionViewModel"/> will be created for each buy transaction.
		/// soldAmount, isTaxFree, gain and fee will be calculated relatively.
		/// </remarks>
		/// <param name="transactions">All transaction existing in database.</param>
		/// <param name="date"></param>
		/// <returns>a <see cref="AfterSaleTaxDetailsViewModel"/> object.</returns>
		public AfterSaleTaxDetailsViewModel CalculateAfterSaleTaxDetails(List<TransactionsViewModel> transactions, DateTime date);


		/// <summary>
		/// Generates a list of assets that contains a list of buy transactions (after sell transactions).
		/// Each asset hat the informations of number of available coins, and crypto currency and if it is tax-free or not.
		/// </summary>
		/// <param name="afterSaleBuyTransaction"></param>
		/// <returns>A list of <see cref="RecommendationDataAssetTableViewModel"/></returns>
		public List<RecommendationDataAssetTableViewModel> GenerateRecommendationAssetsTable(AfterSaleTaxDetailsViewModel afterSaleBuyTransaction);

		/// <summary>
		/// Generates a set of data which will be used for chart.js, that shows how many assets are present and when which transactions are tax-free.
		/// </summary>
		/// <param name="recommendationAssets"></param>
		/// <returns>a list of <see cref="RecommendationAssetChartViewModel"/></returns>
		public List<RecommendationAssetChartViewModel> GenerateRecommendationAssetsChart(List<RecommendationDataAssetTableViewModel> recommendationAssets);

		/// <summary>
		/// Generates an object <see cref="RecommendationDataViewModel"/> that contains the assets for recommendation table and chart data
		/// </summary>
		/// <param name="transactions"></param>
		/// <param name="date"></param>
		/// <returns>a <see cref="RecommendationDataViewModel"/> object</returns>
		public RecommendationDataViewModel GenerateRecommendationData(List<TransactionsViewModel> transactions, DateTime date);

		/// <summary>
		/// Calculates portfolio data for the chart in Dashboard
		/// </summary>
		/// <param name="transactions"></param>
		/// <param name="year"></param>
		/// <returns>a DashboardViewModel for Dashboard View/></returns>
		public DashboardViewModel CalculateEarningsPerMonth(List<TransactionsViewModel> transactions, int year);


		/// <summary>
		/// Generates a single summary viewmodel object with the data
		/// given. It's basically an aggregation view model.
		///
		/// author: Tuan Bui
		/// </summary>
		/// <param name="transactions">list of transactions</param>
		/// <param name="year">year to be filtered</param>
		public EvaluationPerCurrency GenerateSingleSummary(List<TransactionsViewModel> transactions, int year);


		/// <summary>
		/// Genenrates a new tax evaluation view model
		///
		/// author: Tuan Bui
		/// </summary>
		/// <param name="year">year to be filtered</param>
		/// <param name="transactions"> list of transactions</param>
		public TaxEvaluationViewModelNew GenerateNewTaxEvaluationViewModel(List<TransactionsViewModel> transactions, int year);

	}
}
