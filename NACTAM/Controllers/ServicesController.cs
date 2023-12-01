using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NACTAM.Models;
using NACTAM.Models.API;
using NACTAM.Models.Repositories;
using NACTAM.Models.TaxRecommendation;
using NACTAM.ViewModels;
using NACTAM.ViewModels.TaxEvaluation;
using NACTAM.ViewModels.TaxEvaluationNew;
using NACTAM.ViewModels.TaxRecommendation;
using NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails;

namespace NACTAM.Controllers {

	[Route("[controller]")]
	public class ServicesController : Controller {

		private readonly ITransactionRepository _transactionRepository;

		private readonly ITaxCalculations _taxCalculations;
		private readonly ICurrencyApi _currencyApi;
		private readonly ITaxRule _taxRule;
		private readonly UserManager<User> _userManager;

		public ServicesController(ITransactionRepository transactionRepository, ITaxCalculations taxCalculations, ICurrencyApi currencyApi, ITaxRule taxRule, UserManager<User> userManager) {
			_transactionRepository = transactionRepository;
			_taxCalculations = taxCalculations;
			_currencyApi = currencyApi;
			_taxRule = taxRule;
			_userManager = userManager;
		}


		/// <summary>
		/// Controller for TaxEvaluation view.
		/// This function calculates after-sale-details with by using several functions of TaxCalculationsService and TaxEvaluationViewModel, which are shown on the view.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxEvaluation"/>
		/// <seealso cref="TaxCalculationsService"/>
		/// </remarks>
		/// <returns>TaxEvaluation view</returns>
		/// <author> Mervan Kilic </author>
		[Authorize(Roles = "PrivatePerson")]
		[HttpGet("/Services/TaxEvaluationOld")]
		public IActionResult TaxEvaluationOld() {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			try {
				var allTransactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _currencyApi).Transactions;
				var allTransactionsForTaxDetail = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _currencyApi).Transactions;
				var allTransactionsForMS = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _currencyApi).Transactions;

				var allSellTransactions = _taxCalculations.CalculateAfterSaleTaxDetails(allTransactions, DateTime.Now).AfterSaleSellTransactions;
				var saleTaxDetails = _taxCalculations.GenerateSaleTaxDetails(_taxCalculations.CalculateAfterSaleTaxDetails(allTransactionsForTaxDetail, DateTime.Now), DateTime.Now.Year);
				var profitMiningStaking = _taxCalculations.CalculateProfitFromMiningAndStaking(allTransactionsForMS, DateTime.Now.Year);


				decimal newProfit = 0, newLoss = 0, newFee = 0, newProfitFromSales = 0;
				TaxEvaluationViewModel.SumUp(allSellTransactions, ref newProfit, ref newLoss, ref newFee);
				TaxEvaluationViewModel.CalculateProfitFromSales(allSellTransactions, ref newProfitFromSales);

				var transactionsCopy1 = allSellTransactions;
				var cryptoCurrencyList = TaxEvaluationViewModel.ListedCurrencies(transactionsCopy1);
				var yearList = TaxEvaluationViewModel.ListedYears(transactionsCopy1);
				var taxCount = TaxEvaluationViewModel.CountTaxFreeAndLiableSales(allSellTransactions, DateTime.Now, new GermanTaxRule());
				var taxRule = new GermanTaxRule();


				if (allSellTransactions != null) {
					TaxEvaluationViewModel viewModel = new(allSellTransactions) {
						Profit = newProfit,
						Loss = newLoss,
						Fee = newFee,
						ProfitFromSales = newProfitFromSales,
						Year = 0,
						CryptoCurrency = " ",
						CryptoCurrencyTransactions = cryptoCurrencyList,
						YearTransactions = yearList,
						TaxationCount = taxCount,
						ProfitMiningStaking = profitMiningStaking.Item1 + profitMiningStaking.Item2,
						SellProfitLimit = taxRule.CalculateSaleProfitAllowanceLimit(saleTaxDetails.TaxFreeSaleProfit),
						OthersProfitLimit = taxRule.CalculateOthersProfitAllowanceLimit(profitMiningStaking.Item1 + profitMiningStaking.Item2),
						ChartData = TaxEvaluationViewModel.GenerateChartData(allSellTransactions),
						PdfData = new PdfDataViewModel(0, 0, 0, 0, 0, new Tuple<decimal, decimal>(profitMiningStaking.Item1, profitMiningStaking.Item2), taxRule.CalculateSaleProfitAllowanceLimit(saleTaxDetails.TaxLiableSaleProfit), taxRule.CalculateOthersProfitAllowanceLimit(profitMiningStaking.Item1 + profitMiningStaking.Item2), saleTaxDetails)
					};
					return View("~/Views/Services/TaxEvaluation/TaxEvaluationOld.cshtml", viewModel);
				} else {
					TaxEvaluationViewModel viewModel = new TaxEvaluationViewModel();
					return View("~/Views/Services/TaxEvaluation/TaxEvaluationOld.cshtml", viewModel);
				}
			} catch (Exception) {
				return View("TooManyRequestsError");
			}
		}



		/// <summary>
		/// Controller for TaxEvaluation view.
		/// This function is a complete rewrite of the previous Tax Evaluation
		///
		/// author: Tuan Bui
		/// </summary>
		[Authorize(Roles = "PrivatePerson")]
		[HttpGet("/Services/TaxEvaluation")]
		public async Task<IActionResult> TaxEvaluation(int? year) {
			var user = await _userManager.GetUserAsync(User);
			int certainYear = year ?? DateTime.Now.Year;
			List<TransactionsViewModel> transactions;
			try {
				transactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(user.Id), _currencyApi).Transactions;
			} catch (Exception) {
				return View("TooManyRequestsError");
			}
			try {
				TaxEvaluationViewModelNew viewModel = _taxCalculations.GenerateNewTaxEvaluationViewModel(transactions, certainYear);
				viewModel.User = user;
				return View(viewModel);
			} catch (Exception) {
				return View("InconsistentDatasetError");
			}
		}







		/// <summary>
		/// Controller for TaxEvaluation view.
		/// This function calculates after-sale-details with by using several functions of TaxCalculationsService and TaxEvaluationViewModel, which are shown on the view.
		/// By getting inputs by the user, this function updates the displayed data on the view.
		/// </summary>
		/// <remarks>
		/// <seealso cref="FilterTransactions"/>
		/// <seealso cref="TaxCalculationsService"/>
		/// </remarks>
		/// <param name="year">filter value for transactions</param>
		/// <param name="cryptoCurrency">filter value for transactions</param>
		/// <returns>TaxEvaluation view</returns>
		/// <author> Mervan Kilic </author>
		[Authorize(Roles = "PrivatePerson")]
		[HttpPost("ChangeByYear")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> FilterTransactions(int year, string cryptoCurrency) {

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			try {
				var allTransactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _currencyApi).Transactions;
				var allTransactionsForTaxDetail = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _currencyApi).Transactions;
				var allTransactionsForMS = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _currencyApi).Transactions.Where(t => t.Date.Year == year);

				var allSellTransactions = _taxCalculations.CalculateAfterSaleTaxDetails(allTransactions, DateTime.Now).AfterSaleSellTransactions;
				var taxDetailSellTransactions = _taxCalculations.CalculateAfterSaleTaxDetails(allTransactionsForTaxDetail, DateTime.Now);
				taxDetailSellTransactions.AfterSaleSellTransactions = TaxEvaluationViewModel.Filter(taxDetailSellTransactions.AfterSaleSellTransactions, year);
				var saleTaxDetails = _taxCalculations.GenerateSaleTaxDetails(taxDetailSellTransactions, DateTime.Now.Year);
				var pdfProfitMiningStaking = _taxCalculations.CalculateProfitFromMiningAndStaking(allTransactionsForMS.ToList(), year);
				var profitMiningStaking = _taxCalculations.CalculateProfitFromMiningAndStaking(allTransactionsForMS.Where(t => t.CryptoCurrency == cryptoCurrency).ToList(), year);


				decimal newProfit = 0, newLoss = 0, newFee = 0, newProfitFromSales = 0;
				decimal pdfProfit = 0, pdfLoss = 0, pdfFee = 0;
				var sellTransactionsCopy = allSellTransactions;
				var yearCryptoTransactions = TaxEvaluationViewModel.Filter(sellTransactionsCopy, year, cryptoCurrency);
				var yearTransactions = TaxEvaluationViewModel.Filter(sellTransactionsCopy, year);

				TaxEvaluationViewModel.SumUp(yearCryptoTransactions, ref newProfit, ref newLoss, ref newFee);
				TaxEvaluationViewModel.SumUp(yearTransactions, ref pdfProfit, ref pdfLoss, ref pdfFee);
				TaxEvaluationViewModel.CalculateProfitFromSales(yearTransactions, ref newProfitFromSales);

				var cryptoCurrencyList = TaxEvaluationViewModel.ListedCurrencies(sellTransactionsCopy);
				var yearList = TaxEvaluationViewModel.ListedYears(sellTransactionsCopy);
				var taxCount = TaxEvaluationViewModel.CountTaxFreeAndLiableSales(yearCryptoTransactions, DateTime.Now, new GermanTaxRule());
				var taxRule = new GermanTaxRule();

				if (yearCryptoTransactions != null) {
					TaxEvaluationViewModel viewModel = new(allSellTransactions) {
						Profit = newProfit,
						Loss = newLoss,
						Fee = newFee,
						ProfitFromSales = newProfitFromSales,
						Year = year,
						CryptoCurrency = cryptoCurrency,
						CryptoCurrencyTransactions = cryptoCurrencyList,
						YearTransactions = yearList,
						TaxationCount = taxCount,
						ProfitMiningStaking = profitMiningStaking.Item1 + profitMiningStaking.Item2,
						SellProfitLimit = taxRule.CalculateSaleProfitAllowanceLimit(saleTaxDetails.TaxLiableSaleProfit),
						OthersProfitLimit = taxRule.CalculateOthersProfitAllowanceLimit(profitMiningStaking.Item1 + profitMiningStaking.Item2),
						ChartData = TaxEvaluationViewModel.GenerateChartData(allSellTransactions.Where(t => t.CryptoCurrency == cryptoCurrency).ToList()),
						PdfData = new PdfDataViewModel(year, pdfProfit, pdfLoss, pdfFee, newProfitFromSales, new Tuple<decimal, decimal>(pdfProfitMiningStaking.Item1, pdfProfitMiningStaking.Item2), taxRule.CalculateSaleProfitAllowanceLimit(saleTaxDetails.TaxLiableSaleProfit), taxRule.CalculateOthersProfitAllowanceLimit(pdfProfitMiningStaking.Item1 + pdfProfitMiningStaking.Item2), saleTaxDetails)
					};
					return View("~/Views/Services/TaxEvaluation/TaxEvaluation.cshtml", viewModel);
				} else {
					TaxEvaluationViewModel viewModel = new();
					return View("~/Views/Services/TaxEvaluation/TaxEvaluation.cshtml", viewModel);
				}
			} catch (Exception) {
				return View("TooManyRequestsError");
			}
		}



		/// <summary>
		/// Controller for TaxRecommendation view.
		/// It calls transactions from database and creates related objects/view models with the help of TaxCalculationsService.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxRecommendation"/>
		/// <seealso cref="TaxCalculationsService"/>
		/// </remarks>
		/// <returns>TaxRecommendation view</returns>
		/// <author> Cagil Ceren Aslan </author>
		[Authorize(Roles = "PrivatePerson")]
		[HttpGet("/Services/TaxRecommendation")]
		public IActionResult TaxRecommendation() {
			try {
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var transactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _currencyApi).Transactions;
				var nonCryptoSaleProfitCookie = Convert.ToDecimal(Request.Cookies["NonCryptoSaleProfit"]);
				var nonCryptoOthersProfitCookie = Convert.ToDecimal(Request.Cookies["NonCryptoOthersProfit"]);

				var taxDetailsForSaleProfit = _taxCalculations.GenerateSaleTaxDetails(_taxCalculations.CalculateAfterSaleTaxDetails(transactions, DateTime.Now), DateTime.Now.Year);
				var cryptoOthersProfit = _taxCalculations.CalculateProfitFromMiningAndStaking(transactions, DateTime.Now.Year);

				TaxRecommendationViewModel taxrecommendation = new() {
					SaleProfitChart = _taxCalculations.GenerateSaleProfitChart(taxDetailsForSaleProfit, nonCryptoSaleProfitCookie),
					OthersProfitChart = _taxCalculations.GenerateOthersProfitChart(cryptoOthersProfit, nonCryptoOthersProfitCookie),
					RecommendationData = _taxCalculations.GenerateRecommendationData(transactions, DateTime.Now),
					NonCryptoSaleProfit = nonCryptoSaleProfitCookie,
					NonCryptoOthersProfit = nonCryptoOthersProfitCookie,

				};
				return View(taxrecommendation);
			} catch (Exception) {
				return View("TooManyRequestsError");
			}
		}


		/// <summary>
		/// Controller for TaxRecommendation view.
		/// Gets user inputs for the profit charts, and then
		/// calls transactions from database and creates related objects/view models with the help of TaxCalculationsService.
		/// </summary>
		/// <remarks>
		/// <seealso cref="TaxRecommendation"/>
		/// <seealso cref="TaxCalculationsService"/>
		/// </remarks>
		/// <param name="nonCryptoSaleProfit"></param>
		/// <param name="nonCryptoOthersProfit"></param>
		/// <returns>TaxRecommendation view</returns>
		/// <author> Cagil Ceren Aslan </author>
		[Authorize(Roles = "PrivatePerson")]
		[HttpPost("/Services/TaxRecommendationSaleProfit")]
		public IActionResult TaxRecommendationSaleProfit(decimal nonCryptoSaleProfit, decimal nonCryptoOthersProfit) {
			try {
				var cookieOptions = new CookieOptions {
					Expires = DateTime.Now.AddDays(30),
					Path = "/"
				};
				Response.Cookies.Append("NonCryptoSaleProfit", nonCryptoSaleProfit.ToString(), cookieOptions);
				Response.Cookies.Append("NonCryptoOthersProfit", nonCryptoOthersProfit.ToString(), cookieOptions);


				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var transactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _currencyApi).Transactions;
				var taxDetailsForSaleProfit = _taxCalculations.GenerateSaleTaxDetails(_taxCalculations.CalculateAfterSaleTaxDetails(transactions, DateTime.Now), DateTime.Now.Year);
				var cryptoOthersProfit = _taxCalculations.CalculateProfitFromMiningAndStaking(transactions, DateTime.Now.Year);

				TaxRecommendationViewModel taxrecommendation = new() {
					SaleProfitChart = _taxCalculations.GenerateSaleProfitChart(taxDetailsForSaleProfit, nonCryptoSaleProfit),
					OthersProfitChart = _taxCalculations.GenerateOthersProfitChart(cryptoOthersProfit, nonCryptoOthersProfit),
					RecommendationData = _taxCalculations.GenerateRecommendationData(transactions, DateTime.Now),
					NonCryptoSaleProfit = nonCryptoSaleProfit,
					NonCryptoOthersProfit = nonCryptoOthersProfit,

				};
				return View("TaxRecommendation", taxrecommendation);
			} catch (Exception) {
				return View("TooManyRequestsError");
			}
		}
	}
}
