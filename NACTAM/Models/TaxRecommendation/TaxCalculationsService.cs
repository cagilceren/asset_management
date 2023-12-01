using System.Globalization;

using NACTAM.Models.API;
using NACTAM.ViewModels;
using NACTAM.ViewModels.TaxEvaluationNew;
using NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails;
using NACTAM.ViewModels.TaxRecommendation.ProfitChart;
using NACTAM.ViewModels.TaxRecommendation.RecommendationData;
using NACTAM.ViewModels.TaxRecommendation.RecommendationData.RecommendationChart;



namespace NACTAM.Models.TaxRecommendation {
	/// <summary>
	/// Does all tax related calculations.
	///
	/// author: Cagil Ceren Aslan
	/// </summary>
	/// <remarks>
	/// This service can be used via dependency injection.
	/// This service implements <seealso cref="ITaxRule"/> and <seealso cref="ITaxCalculations"/>
	/// </remarks>

	public class TaxCalculationsService : ITaxCalculations {

		/// <summary>
		/// Represents the tax rule used for tax calculations. <seealso cref="ITaxRule"/>
		/// </summary>
		private readonly ITaxRule _taxRule;


		/// <summary>
		/// Represents the currency api is used to get current exchange rate. <seealso cref="ICurrencyApi"/>
		/// </summary>
		private readonly ICurrencyApi _currencyApi;

		/// <summary>
		/// Initializes a new instance of <seealso cref="TaxCalculationsService"/>.
		/// </summary>
		/// <param name="taxRule">The tax rule to be used for calculations.</param>
		/// <param name="currencyApi">To get the current rate of a currency</param>
		public TaxCalculationsService(ITaxRule taxRule, ICurrencyApi currencyApi) {
			_taxRule = taxRule;
			_currencyApi = currencyApi;
		}


		/// <inheritdoc/>
		public OthersProfitChartViewModel GenerateOthersProfitChart((decimal, decimal) cryptoOthersProfit, decimal NonCryptoOthersProfit = 0) {
			var miningProfit = Math.Ceiling(cryptoOthersProfit.Item1);
			var stakingProfit = Math.Ceiling(cryptoOthersProfit.Item2);
			var othersProfit = miningProfit + stakingProfit + NonCryptoOthersProfit;
			OthersProfitChartViewModel othersProfitChartViewModel = new() {
				NonCryptoOthersProfit = Math.Ceiling(NonCryptoOthersProfit),
				MiningProfit = miningProfit,
				StakingProfit = stakingProfit,
				FreeLimitOthersProfit = Math.Ceiling(_taxRule.CalculateOthersProfitAllowanceLimit(othersProfit))
			};
			return othersProfitChartViewModel;
		}


		/// <inheritdoc/>
		public (decimal, decimal) CalculateProfitFromMiningAndStaking(List<TransactionsViewModel> transactions, int year) {
			DateTime beginnDate = _taxRule.CalculateTaxYearBeginnAndEndDate(year).Item1;
			DateTime endDate = _taxRule.CalculateTaxYearBeginnAndEndDate(year).Item2;
			decimal miningProfit = transactions
				.Where(t => t.Date >= beginnDate && t.Date <= endDate)
				.Where(t => t.Type == "Mining")
				.Sum(t => t.Amount * t.Rate);
			decimal stakingProfit = transactions
				.Where(t => t.Date >= beginnDate && t.Date <= endDate)
				.Where(t => t.Type == "Staking")
				.Sum(t => t.Amount * t.Rate);
			return (miningProfit, stakingProfit);
		}

		/// <inheritdoc/>
		public (decimal, decimal) CalculateIncomeAndExpenditures(List<TransactionsViewModel> transactions, int year) {
			DateTime beginnDate = _taxRule.CalculateTaxYearBeginnAndEndDate(year).Item1;
			DateTime endDate = _taxRule.CalculateTaxYearBeginnAndEndDate(year).Item2;
			decimal income = transactions
				.Where(t => t.Date >= beginnDate && t.Date <= endDate)
				.Where(t => t.Type == "Sell")
				.Sum(t => t.Amount * t.Rate);
			decimal expenditures = transactions
				.Where(t => t.Date >= beginnDate && t.Date <= endDate)
				.Where(t => t.Type == "Buy")
				.Sum(t => t.Amount * t.Rate);
			return (income, expenditures);
		}

		/// <inheritdoc/>
		public (decimal, decimal) CalculateTaxRelevantIncomeAndExpenditures(AfterSaleTaxDetailsViewModel saleTaxDetails) {
			decimal income = saleTaxDetails.AfterSaleSellTransactions
				.Sum(t => t.SoldAmount * t.SellRate);
			decimal expenditures = saleTaxDetails.AfterSaleSellTransactions
				.Sum(t => t.SoldAmount * t.BuyRate);
			return (income, expenditures);
		}

		/// <inheritdoc/>
		public SaleProfitChartViewModel GenerateSaleProfitChart(SaleTaxDetailsViewModel saleTaxDetails, decimal NonCryptoSaleProfit = 0) {
			var sellProfit = Math.Ceiling(saleTaxDetails.TaxLiableSaleProfit + saleTaxDetails.TaxLiableSaleLoss + NonCryptoSaleProfit);
			SaleProfitChartViewModel saleProfitChartViewModel = new() {
				NonCryptoSaleProfit = Math.Ceiling(NonCryptoSaleProfit),
				CryptoSaleProfit = Math.Ceiling(saleTaxDetails.TaxLiableSaleProfit + saleTaxDetails.TaxLiableSaleLoss),
				FreeLimitSaleProfit = _taxRule.CalculateSaleProfitAllowanceLimit(sellProfit)
			};
			return saleProfitChartViewModel;
		}


		/// <inheritdoc/>
		public SaleTaxDetailsViewModel GenerateSaleTaxDetails(AfterSaleTaxDetailsViewModel afterSaleTaxDetails, int year) {
			DateTime beginnDate = _taxRule.CalculateTaxYearBeginnAndEndDate(year).Item1;
			DateTime endDate = _taxRule.CalculateTaxYearBeginnAndEndDate(year).Item2;
			List<AfterSaleSellTransactionViewModel> afterSaleSellTaxDetails = afterSaleTaxDetails.AfterSaleSellTransactions;

			// Groups a list of AfterSaleSellTransactionViewModel objects according to "isTaxFree" and "gain < 0".
			var groupedByIsTaxFreeAndGain = afterSaleSellTaxDetails
				.AsEnumerable()
				.Where(s => s.SellDate >= beginnDate && s.SellDate <= endDate)
				.GroupBy(s => (s.IsTaxFree, s.Gain < 0))
				.ToDictionary(g => g.Key, g => g);

			SaleTaxDetailsViewModel saleTaxDetails = new() {

				TaxFreeSaleLoss = groupedByIsTaxFreeAndGain
					.GetValueOrDefault((true, true)) // gets the objects that are tax-free and their gain is smaller than zero.
					?.Sum(s => s.Gain) ?? 0M,

				TaxLiableSaleLoss = groupedByIsTaxFreeAndGain
					.GetValueOrDefault((false, true)) // gets the objects that are tax-liable and their gain is smaller than zero.
					?.Sum(s => s.Gain) ?? 0M,

				TaxFreeSaleProfit = groupedByIsTaxFreeAndGain
					.GetValueOrDefault((true, false)) // gets the objects that are tax-free and their gain is bigger than and equal to zero.
					?.Sum(s => s.Gain) ?? 0M,

				TaxLiableSaleProfit = groupedByIsTaxFreeAndGain
					.GetValueOrDefault((false, false)) // gets the objects that are tax-liable and their gain is bigger than and equal to zero.
					?.Sum(s => s.Gain) ?? 0M,

				TaxLiableFee = afterSaleSellTaxDetails
					.Where(s => s.IsTaxFree == false) // gets all tax-liable objects
					.Sum(s => s.TotalFee),

				TaxFreeFee = afterSaleSellTaxDetails
					.Where(s => s.IsTaxFree == true) // gets all tax-free objects
					.Sum(s => s.TotalFee),
			};
			return saleTaxDetails;
		}

		/// <inheritdoc/>
		public SaleTaxDetailsViewModel GenerateAllTaxDetails(AfterSaleTaxDetailsViewModel afterSaleTaxDetails, int year) {
			var tmpTaxDetails = GenerateSaleTaxDetails(afterSaleTaxDetails, year);

			tmpTaxDetails.TaxFreeFee += afterSaleTaxDetails.AfterSaleBuyTransactions
				.Where(s => s.IsTaxFree)
				.Sum(s => s.RemainingFee);
			return tmpTaxDetails;
		}


		/// <inheritdoc/>
		public AfterSaleTaxDetailsViewModel CalculateAfterSaleTaxDetails(List<TransactionsViewModel> transactionList, DateTime date) {
			List<TransactionsViewModel> transactions = transactionList.ConvertAll(x => (TransactionsViewModel)x.Clone());
			List<TransactionsViewModel> buyTransactions = transactions.Where(t => t.Type != "Sell").OrderBy(transaction => transaction.Date).ToList();
			List<TransactionsViewModel> sellTransactions = transactions.Where(t => t.Type == "Sell").OrderBy(transaction => transaction.Date).ToList();
			List<AfterSaleBuyTransactionViewModel> afterSaleBuyTaxDetails = new List<AfterSaleBuyTransactionViewModel>();
			List<AfterSaleSellTransactionViewModel> afterSaleSellTaxDetails = new();

			//Group all transactions based on their Crypto Currencies.
			var buyTransactionCurrencyEnumerators = buyTransactions
				.AsEnumerable()
				.GroupBy(x => x.CryptoCurrency)
				.ToDictionary(x => x.Key, x => x.GetEnumerator());

			foreach (var sellTransaction in sellTransactions) {
				while (sellTransaction.Amount != 0) {
					var currencyEnumerator = buyTransactionCurrencyEnumerators[sellTransaction.CryptoCurrency];

					// skip the buy transactions that has no coins.
					while (currencyEnumerator.Current != null && currencyEnumerator.Current.Amount == 0 && currencyEnumerator.MoveNext()) {
					}

					if (!(
						(currencyEnumerator.Current == null && !currencyEnumerator.MoveNext()) ||
						currencyEnumerator.Current == null ||
						currencyEnumerator.Current.Amount == 0 ||
						currencyEnumerator.Current.Date > sellTransaction.Date)
						) {

						var buyTransaction = currencyEnumerator.Current;
						var amountToReduce = Math.Min(sellTransaction.Amount, buyTransaction.Amount);
						var buyFeeToReduce = amountToReduce * buyTransaction.Fee / buyTransaction.Amount;
						var sellFeeToReduce = amountToReduce * sellTransaction.Fee / sellTransaction.Amount;

						// Create a new AfterSaleSellTransactionViewModel object for each buy transaction whose coins are used for a sell transaction.
						// For ex: If a sell transaction sells 3 coins and there are 3 different buy transactions, each of them has only 1 coin, there will be created 3 AfterSaleSellTransactionViewModel
						// objects for each buy transactions, because the 3 coins sold by the sell transactions come from 3 different buy transactions.
						AfterSaleSellTransactionViewModel sellTaxDetail = new() {
							SellDate = sellTransaction.Date,
							BuyDate = buyTransaction.Date,
							SellRate = sellTransaction.Rate,
							BuyRate = buyTransaction.Rate,
							Type = sellTransaction.Type,
							CryptoCurrency = sellTransaction.CryptoCurrency,
							TotalFee = buyFeeToReduce + sellFeeToReduce,
							SoldAmount = amountToReduce,
							IsTaxFree = _taxRule.IsTaxFreeByDateLimit(buyTransaction.Date, sellTransaction.Date),
							Gain = amountToReduce * (sellTransaction.Rate - buyTransaction.Rate) - (buyFeeToReduce + sellFeeToReduce),
						};

						sellTransaction.Fee -= sellFeeToReduce;
						buyTransaction.Fee -= buyFeeToReduce;
						buyTransaction.Amount -= amountToReduce;
						sellTransaction.Amount -= amountToReduce;
						afterSaleSellTaxDetails.Add(sellTaxDetail);
					}

				}
			}

			// Creates a AfterSaleBuyTransactionViewModel object for each buy transaction that has still coins after sale (after reducing sell transactions).
			foreach (var buyTransaction in buyTransactions) {
				if (buyTransaction.Amount != 0) {
					var cryptoCurrency = _currencyApi.GetCryptoCurrencyByName(buyTransaction.CryptoCurrency);
					if (cryptoCurrency != null) {
						AfterSaleBuyTransactionViewModel afterSaleBuyTransaction = new() {
							Date = buyTransaction.Date,
							Rate = buyTransaction.Rate,
							Type = buyTransaction.Type,
							CryptoCurrency = buyTransaction.CryptoCurrency,
							RemainingFee = buyTransaction.Fee,
							RemainingAmount = buyTransaction.Amount,
							DueDate = _taxRule.CalculateDueDateByDateLimit(buyTransaction.Date),
							IsTaxFree = _taxRule.IsTaxFreeByDateLimit(buyTransaction.Date, date),
							CurrentRate = _currencyApi.GetExchangeRate(cryptoCurrency.Id),
							ExpectedGain = buyTransaction.Amount * (_currencyApi.GetExchangeRate(cryptoCurrency.Id) - buyTransaction.Rate) - buyTransaction.Fee,
						};
						afterSaleBuyTaxDetails.Add(afterSaleBuyTransaction);
					}
				}
			}


			AfterSaleTaxDetailsViewModel afterSaleTaxDetails = new() {
				AfterSaleBuyTransactions = afterSaleBuyTaxDetails,
				AfterSaleSellTransactions = afterSaleSellTaxDetails
			};
			return afterSaleTaxDetails;
		}


		/// <inheritdoc/>
		public List<RecommendationDataAssetTableViewModel> GenerateRecommendationAssetsTable(AfterSaleTaxDetailsViewModel afterSaleBuyTransaction) {
			List<AfterSaleBuyTransactionViewModel> afterSaleBuyTransactions = afterSaleBuyTransaction.AfterSaleBuyTransactions;
			List<RecommendationDataAssetTableViewModel> assets = new();

			// assigns tipps according to IsTaxFree and ExpectedGain properties of remaning buy transactions (AfterSaleBuyTransactionViewModel).
			if (afterSaleBuyTransactions != null) {
				foreach (var transaction in afterSaleBuyTransactions) {
					if (transaction.ExpectedGain < 0 && transaction.IsTaxFree == false) {
						transaction.Tipps = TippsEnum.lossTaxLiableTipp;
					} else if (transaction.ExpectedGain > 0 && transaction.IsTaxFree == false) {
						transaction.Tipps = TippsEnum.profitTaxLiableTipp;
					} else if (transaction.ExpectedGain < 0 && transaction.IsTaxFree == true) {
						transaction.Tipps = TippsEnum.lossTaxFreeTipp;
					} else if (transaction.ExpectedGain > 0 && transaction.IsTaxFree == true) {
						transaction.Tipps = TippsEnum.profitTaxFreeTipp;
					}

					// If there is an asset for the currency of the transaction, increases AvaliableCoinAmount and adds the transaction to the transactions list of the related asset.
					if (assets.Any(a => a.CryptoCurrency == transaction.CryptoCurrency)) {
						var existingAsset = assets.Find(a => a.CryptoCurrency == transaction.CryptoCurrency);
						existingAsset.AvaliableCoinAmount += transaction.RemainingAmount;
						existingAsset.Transactions.Add(transaction);
					} else {
						// If there is no asset for the currency of the transaction, creates an asset.
						RecommendationDataAssetTableViewModel assetToAdd = new() {
							CryptoCurrency = transaction.CryptoCurrency,
							AvaliableCoinAmount = transaction.RemainingAmount
						};
						assetToAdd.Transactions.Add(transaction);
						assets.Add(assetToAdd);
					}
				}
			}
			foreach (var asset in assets) {
				if (asset.Transactions.Any(x => x.IsTaxFree)) {
					asset.IsTaxFree = true;
				}
			}
			return assets;
		}


		/// <inheritdoc/>
		public List<RecommendationAssetChartViewModel> GenerateRecommendationAssetsChart(List<RecommendationDataAssetTableViewModel> recommendationAssets) {

			CultureInfo germanTime = new CultureInfo("de-DE");
			List<RecommendationAssetChartViewModel> chartDataSets = new();

			foreach (var asset in recommendationAssets) {
				RecommendationAssetChartViewModel chartData = new() {
					ChartName = asset.CryptoCurrency,
					DataSets = new() { new RecommendationAssetChartDataSet() },
				};

				var taxFreeAmount = asset.Transactions.Where(t => t.IsTaxFree == true && t.RemainingAmount != 0).Sum(t => t.RemainingAmount);
				if (taxFreeAmount > 0) {
					chartData.DataSets[0].Data.Add(taxFreeAmount);
					chartData.Labels.Add("Steuerfrei");
				}

				decimal allCoinAmount = asset.Transactions.Where(x => x.IsTaxFree == false && x.RemainingAmount != 0).Sum(x => x.RemainingAmount);
				List<DateAmountPair> transactionsGroupedByDateAndAmount = asset.Transactions
					.Where(x => x.IsTaxFree == false && x.RemainingAmount != 0)
					.GroupBy(x => x.DueDate.ToString("dd.MM.yyyy"))
					.Take(taxFreeAmount > 0 ? 2 : 3)
					.Select((x) => new DateAmountPair() { Date = x.Key, Amount = x.Sum(f => f.RemainingAmount) })
					.ToList();

				decimal restCoinAmount = allCoinAmount - transactionsGroupedByDateAndAmount.Sum(x => x.Amount);

				foreach (var transaction in transactionsGroupedByDateAndAmount) {
					chartData.DataSets[0].Data.Add(transaction.Amount);
					chartData.Labels.Add("Steuerfrei ab " + transaction.Date);
				}

				if (restCoinAmount != 0) {
					chartData.DataSets[0].Data.Add(restCoinAmount);
					chartData.Labels.Add("Restliche Coins");
				}

				chartDataSets.Add(chartData);
			}
			return chartDataSets;
		}


		/// <inheritdoc/>
		public RecommendationDataViewModel GenerateRecommendationData(List<TransactionsViewModel> transactions, DateTime date) {
			RecommendationDataViewModel recommendationData = new() {
				AssetsForTable = GenerateRecommendationAssetsTable(CalculateAfterSaleTaxDetails(transactions, date))
			};
			recommendationData.ChartDataSets = GenerateRecommendationAssetsChart(recommendationData.AssetsForTable);
			return recommendationData;
		}

		/// <inheritdoc/>
		public DashboardViewModel CalculateEarningsPerMonth(List<TransactionsViewModel> transactions, int year) {
			List<decimal> expenses = Enumerable.Repeat(0M, 12).ToList();
			List<decimal> income = Enumerable.Repeat(0M, 12).ToList();
			List<decimal> netEarnings = Enumerable.Repeat(0M, 12).ToList();

			foreach (var transaction in transactions) {
				var monthNumber = transaction.Date.Month - 1;
				if (transaction.Date.Year == year) {
					if (transaction.Type == "Buy") {
						expenses[monthNumber] += transaction.Amount * transaction.Rate + transaction.Fee;
					} else {
						income[monthNumber] += transaction.Amount * transaction.Rate;
						expenses[monthNumber] += transaction.Fee;
					}
				}
			}

			netEarnings = income.Zip(expenses, (x, y) => x - y).ToList();

			DashboardViewModel dashboardData = new() {
				NumberOfTransactions = transactions.Count(),
				ExpensesData = expenses,
				IncomeData = income,
				NetEarningsData = netEarnings,
				TotalExpenses = expenses.Sum(),
				TotalIncome = income.Sum(),
				TotalProfit = netEarnings.Sum(),
			};

			return dashboardData;
		}

		/// <inheritdoc/>
		public EvaluationPerCurrency GenerateSingleSummary(List<TransactionsViewModel> transactions, int year) {
			var filteredTransactions = transactions
				.Where(x => x.Date <= new DateTime(year, 12, 31).AddDays(1).AddTicks(-1)).ToList();
			if (filteredTransactions.Count == 0)
				return new EvaluationPerCurrency {
					Total = new SingleSummaryViewModel(_taxRule) { },
					TotalTaxed = new SingleSummaryViewModel(_taxRule) { },
					Unsold = new List<AfterSaleBuyTransactionViewModel> { },
					Sold = new List<AfterSaleSellTransactionViewModel> { },
				};
			var afterSaleResult = CalculateAfterSaleTaxDetails(filteredTransactions, new DateTime(year, 12, 31).AddDays(1).AddTicks(-1));
			var (profitMining, profitStaking) = CalculateProfitFromMiningAndStaking(filteredTransactions, year);
			SaleTaxDetailsViewModel taxDetails = GenerateAllTaxDetails(afterSaleResult, year);
			var (taxRelevantIncome, taxRelevantExpenditures) = CalculateTaxRelevantIncomeAndExpenditures(afterSaleResult);
			var (income, expenditures) = CalculateIncomeAndExpenditures(transactions, year);

			return new EvaluationPerCurrency {
				Total = new SingleSummaryViewModel(_taxRule) {
					ProfitsFromMining = profitMining,
					ProfitsFromStaking = profitStaking,
					Losses = taxDetails.TaxFreeSaleLoss + taxDetails.TaxLiableSaleLoss,
					Fees = taxDetails.TaxFreeFee + taxDetails.TaxLiableFee,
					ProfitsFromSelling = taxDetails.TaxFreeSaleProfit + taxDetails.TaxLiableSaleProfit,
					Logo = filteredTransactions[0].Logo,
					Income = income,
					Expenditures = expenditures,
				},
				TotalTaxed = new SingleSummaryViewModel(_taxRule) {
					ProfitsFromMining = profitMining,
					ProfitsFromStaking = profitStaking,
					Losses = taxDetails.TaxLiableSaleLoss,
					Fees = taxDetails.TaxLiableFee,
					ProfitsFromSelling = taxDetails.TaxLiableSaleProfit,
					Logo = filteredTransactions[0].Logo,
					Income = taxRelevantIncome,
					Expenditures = taxRelevantExpenditures
				},
				Unsold = afterSaleResult.AfterSaleBuyTransactions,
				Sold = afterSaleResult.AfterSaleSellTransactions,
			};
		}

		/// <inheritdoc/>
		public TaxEvaluationViewModelNew GenerateNewTaxEvaluationViewModel(List<TransactionsViewModel> transactions, int year) {
			var ordered = transactions.OrderBy(x => x.Date).ToList();
			return new TaxEvaluationViewModelNew {
				Total = GenerateSingleSummary(ordered, year),
				TotalByCurrency = ordered
					.GroupBy(x => x.CryptoCurrency)
					.ToDictionary(x => x.Key, x => GenerateSingleSummary(x.ToList(), year)),
				Year = year,
				Transactions = transactions
			};
		}

	}

	public class DateAmountPair {
		public string Date;
		public decimal Amount;
	};
}
