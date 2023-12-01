using NACTAM.Models.API;
using NACTAM.Models.TaxRecommendation;
using NACTAM.ViewModels;
using NACTAM.ViewModels.TaxRecommendation;
using NACTAM.ViewModels.TaxRecommendation.RecommendationData;

namespace NACTAM.UnitTests;

public class TaxCalculationsTest {
	/// <summary>
	/// Unit Tests for the functions in <see cref="TaxCalculationsService.cs"/>
	/// </summary>
	/// <author>Cagil Ceren Aslan</author>

	public List<Transaction> TransactionsList1;
	public List<Transaction> TransactionsList2;
	public List<Transaction> TransactionsList3;
	public List<Transaction> TransactionsList4;
	public List<TransactionsViewModel> TransactionsList1VM = new();
	public List<TransactionsViewModel> TransactionsList2VM = new();
	public List<TransactionsViewModel> TransactionsList3VM = new();
	public List<TransactionsViewModel> TransactionsList4VM = new();
	public List<PrivatePerson> PrivatePersonList;
	public TaxCalculationsService CalculationService;
	public List<CryptoCurrency> CryptoCurrencyList;

	public TaxCalculationsTest() {

		TransactionsList1 = new List<Transaction> {
			new Transaction(id: 1, date: new DateTime(year: 2021, month: 1, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 1, amount: 20000, exchangeRate: 0.009M, fee: 1.80M),
			new Transaction(id: 2, date: new DateTime(year: 2022, month: 4, day: 1),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 12000, exchangeRate: 0.126M, fee: 15.12M),
			new Transaction(id: 3, date: new DateTime(year: 2022, month: 8, day: 15),  userId: "coolid", type: TransactionType.Buy, currencyId: 1, amount: 10000, exchangeRate: 0.071M, fee: 7.1M),
			new Transaction(id: 4, date: new DateTime(year: 2023, month: 1, day: 20),  userId: "coolid", type: TransactionType.Buy, currencyId: 1, amount: 10000, exchangeRate: 0.075M, fee: 7.5M),
			new Transaction(id: 5, date: new DateTime(year: 2023, month: 4, day: 4),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 19000, exchangeRate: 0.089M, fee: 16.91M),
		};

		TransactionsList2 = new List<Transaction> {
			new Transaction(id: 1, date: new DateTime(year: 2019, month: 1, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 1, amount: 40, exchangeRate: 20M, fee: 20M), // Rest 0
			new Transaction(id: 2, date: new DateTime(year: 2019, month: 4, day: 1),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 10, exchangeRate: 25M, fee: 10M), // 10*5 -10 - 5 = 35 (tax laible)
			new Transaction(id: 3, date: new DateTime(year: 2019, month: 7, day: 15),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 10, exchangeRate: 15M, fee: 20M), // -10*5 -20- 5 = -75 (tax laible)

			new Transaction(id: 4, date: new DateTime(year: 2020, month: 4, day: 3),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 10, exchangeRate: 25M, fee: 10M), // 10*5 -10 - 5 = 35 (tax free)
			new Transaction(id: 5, date: new DateTime(year: 2020, month: 7, day: 17),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 10, exchangeRate: 15M, fee: 20M), // -10*5 -20- 5 = -75 (tax free)

			new Transaction(id: 17, date: new DateTime(year: 2020, month: 8, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 1, amount: 20, exchangeRate: 20M, fee: 0M),
			new Transaction(id: 17, date: new DateTime(year: 2023, month: 1, day: 1),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 20, exchangeRate: 10M, fee: 0M),

			new Transaction(id: 6, date: new DateTime(year: 2021, month: 1, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 1, amount: 10, exchangeRate: 20M, fee: 20M), // Rest 0
			new Transaction(id: 7, date: new DateTime(year: 2023, month: 1, day: 2),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 10, exchangeRate: 40M, fee: 10M), // 10*20 - 30 = 170 (tax free) ****

			new Transaction(id: 14, date: new DateTime(year: 2021, month: 1, day: 2),  userId: "coolid", type: TransactionType.Mining, currencyId: 1, amount: 15, exchangeRate: 5M, fee: 0M), // Rest 0
			new Transaction(id: 15, date: new DateTime(year: 2021, month: 1, day: 3),  userId: "coolid", type: TransactionType.Staking, currencyId: 1, amount: 20, exchangeRate: 10M, fee: 0M), // Rest 0
			new Transaction(id: 8, date: new DateTime(year: 2023, month: 1, day: 3),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 35, exchangeRate: 15M, fee: 10M), // 15*10 + 20*5 - 10 = 240 (tax-free) ****

			new Transaction(id: 9, date: new DateTime(year: 2023, month: 1, day: 4),  userId: "coolid", type: TransactionType.Buy, currencyId: 1, amount: 20, exchangeRate: 30M, fee: 40M), //Rest 0
			new Transaction(id: 10, date: new DateTime(year: 2023, month: 1, day: 4),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 10, exchangeRate: 35M, fee: 15M), // 50 - 15 - 20 = 15 (tax liable) ****
			new Transaction(id: 11, date: new DateTime(year: 2023, month: 1, day: 5),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 10, exchangeRate: 20M, fee: 5M), // -10*10 -25 = -125 (tax liable) ****

			new Transaction(id: 12, date: new DateTime(year: 2023, month: 1, day: 6),  userId: "coolid", type: TransactionType.Staking, currencyId: 1, amount: 10, exchangeRate: 10M, fee: 0M), // Rest 0
			new Transaction(id: 18, date: new DateTime(year: 2023, month: 1, day: 7),  userId: "coolid", type: TransactionType.Sell, currencyId: 1, amount: 10, exchangeRate: 12M, fee: 20M), // 10*2 - 20 = 0

			new Transaction(id: 13, date: new DateTime(year: 2023, month: 1, day: 8),  userId: "coolid", type: TransactionType.Mining, currencyId: 1, amount: 5, exchangeRate: 10M, fee: 0M),
			new Transaction(id: 16, date: new DateTime(year: 2023, month: 3, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 1, amount: 20, exchangeRate: 30M, fee: 40M),
			new Transaction(id: 17, date: new DateTime(year: 2023, month: 4, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 1, amount: 20, exchangeRate: 20M, fee: 0M),

		};

		TransactionsList3 = new List<Transaction> {
			new Transaction(id: 19, date: new DateTime(year: 2022, month: 1, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 2, amount: 15, exchangeRate: 20M, fee: 30M),
			new Transaction(id: 20, date: new DateTime(year: 2022, month: 2, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 2, amount: 20, exchangeRate: 30M, fee: 10M),
			new Transaction(id: 21, date: new DateTime(year: 2023, month: 4, day: 1),  userId: "coolid", type: TransactionType.Sell, currencyId: 2, amount: 10, exchangeRate: 30M, fee: 20M),
		};

		TransactionsList4 = new List<Transaction> {
			new Transaction(id: 22, date: new DateTime(year: 2022, month: 1, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 2, amount: 10, exchangeRate: 20M, fee: 30M),
			new Transaction(id: 23, date: new DateTime(year: 2022, month: 2, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 2, amount: 20, exchangeRate: 30M, fee: 10M),

			new Transaction(id: 24, date: new DateTime(year: 2023, month: 3, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 2, amount: 30, exchangeRate: 30M, fee: 20M),
			new Transaction(id: 25, date: new DateTime(year: 2023, month: 4, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 2, amount: 40, exchangeRate: 30M, fee: 20M),
			new Transaction(id: 26, date: new DateTime(year: 2023, month: 5, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 2, amount: 50, exchangeRate: 30M, fee: 20M),
			new Transaction(id: 27, date: new DateTime(year: 2023, month: 6, day: 1),  userId: "coolid", type: TransactionType.Buy, currencyId: 2, amount: 60, exchangeRate: 30M, fee: 20M),
		};

		TransactionsList1.ForEach((item) => {
			TransactionsList1VM.Add(new TransactionsViewModel(item, "DOGE"));
		});

		TransactionsList2.ForEach((item) => {
			TransactionsList2VM.Add(new TransactionsViewModel(item, "DOGE"));
		});

		TransactionsList2.ForEach((item) => {
			TransactionsList3VM.Add(new TransactionsViewModel(item, "DOGE"));
		});

		TransactionsList3.ForEach((item) => {
			TransactionsList3VM.Add(new TransactionsViewModel(item, "BTC"));
		});

		TransactionsList4.ForEach((item) => {
			TransactionsList4VM.Add(new TransactionsViewModel(item, "BTC"));
		});

		Mock<ICurrencyApi> currencyApi = new();
		currencyApi.Setup(x => x.GetExchangeRate(It.IsAny<int>())).Returns(25);
		currencyApi.Setup(x => x.GetCryptoCurrencyByName(It.IsAny<string>())).Returns(new CryptoCurrency() {
			Id = 1
		});

		CalculationService = new TaxCalculationsService(new GermanTaxRule(), currencyApi.Object);
	}


	/// <summary>
	/// Tests different aspects of the <see cref="CalculateAfterSaleTaxDetails"/> function in <see cref="TaxCalculationsService.cs"/> file.
	/// </summary>
	/// <remarks>
	/// This tests with the given example.
	/// It checks
	/// if the profit after reducing of sell-transactions from buy-transactions are same like in the example,
	/// if the coins of sell-transaction are reduced from the right buy-transactions,
	/// if the rest of the coins are same like in the example.
	/// </remarks>
	[Fact]
	public void TestCalculateAfterSaleTaxDetails() {
		var date = new DateTime(year: 2023, month: 06, day: 30);
		var afterSaleTaxDetails = CalculationService.CalculateAfterSaleTaxDetails(TransactionsList1VM, date);
		var taxDetailsForSaleProfit = CalculationService.GenerateSaleTaxDetails(afterSaleTaxDetails, 2023);
		TaxRecommendationViewModel taxrecommendation = new() {
			RecommendationData = CalculationService.GenerateRecommendationData(TransactionsList1VM, date),
		};

		var taxLiableAfterSaleSellTransactions = afterSaleTaxDetails.AfterSaleSellTransactions.Where(x => x.IsTaxFree == false).ToList();

		Assert.Equal(2, taxLiableAfterSaleSellTransactions.Count());
		Assert.Equal(164, taxLiableAfterSaleSellTransactions[0].Gain);
		Assert.Equal(8.90M + 7.10M, taxLiableAfterSaleSellTransactions[0].TotalFee);

		Assert.Equal(12.36M, taxLiableAfterSaleSellTransactions[1].Gain);
		Assert.Equal(0.89M + 0.75M, taxLiableAfterSaleSellTransactions[1].TotalFee);

		Assert.Equal(9000, taxrecommendation.RecommendationData.AssetsForTable.First().AvaliableCoinAmount);
		Assert.Equal(176.36M, taxDetailsForSaleProfit.TaxLiableSaleProfit);
	}

	/// <summary>
	/// Tests different aspects of the <see cref="GenerateSaleTaxDetails"/> function in <see cref="TaxCalculationsService"/> file.
	/// </summary>
	/// <remarks>
	/// It tests:
	/// if the tax-free and tax-liable loss and profit from sales of a given year are calculated error-free.
	/// </remarks>

	[Fact]
	public void TestGenerateSaleTaxDetails() {
		var date = new DateTime(year: 2023, month: 07, day: 30);
		var afterSaleTaxDetails = CalculationService.CalculateAfterSaleTaxDetails(TransactionsList2VM, date);
		var taxDetails = CalculationService.GenerateSaleTaxDetails(afterSaleTaxDetails, 2023);

		Assert.Equal(-200M, taxDetails.TaxFreeSaleLoss);
		Assert.Equal(410M, taxDetails.TaxFreeSaleProfit);
		Assert.Equal(-125M, taxDetails.TaxLiableSaleLoss);
		Assert.Equal(15M, taxDetails.TaxLiableSaleProfit);
	}

	/// <summary>
	/// Tests different aspects of the <see cref="CalculateProfitFromMiningAndStaking"/> function in <see cref="TaxCalculationsService.cs"/> file.
	/// </summary>
	/// <remarks>
	/// It tests:
	/// if the mining and staking profits of a given year are calculated error-free.
	/// </remarks>
	[Fact]
	public void TestCalculateProfitFromMiningAndStaking() {
		var profitFromMiningAndStaking = CalculationService.CalculateProfitFromMiningAndStaking(TransactionsList2VM, 2021);

		Assert.Equal(75M, profitFromMiningAndStaking.Item1);
		Assert.Equal(200M, profitFromMiningAndStaking.Item2);
	}

	/// <summary>
	/// Tests different aspects of the <see cref="GenerateOthersProfitChart"/> function in <see cref="TaxCalculationsService.cs"/> file.
	/// </summary>
	/// <remarks>
	/// It tests if the data to create others profit pie chart in View <see cref="TaxRecommendation"/> is calculated error free,
	/// both when <see cref="NonCryptoOthersProfit"/> is given and not given into the function. Default value must be zero.
	/// </remarks>
	[Fact]
	public void TestGenerateOthersProfitChart() {
		var othersCryptoProfit = (140M, 50M);
		var othersProfitChart = CalculationService.GenerateOthersProfitChart(othersCryptoProfit, 6M);

		Assert.Equal(6M, othersProfitChart.NonCryptoOthersProfit);
		Assert.Equal(140M, othersProfitChart.MiningProfit);
		Assert.Equal(50M, othersProfitChart.StakingProfit);
		Assert.Equal(60M, othersProfitChart.FreeLimitOthersProfit);

		var othersProfitChartWithNoProfitInput = CalculationService.GenerateOthersProfitChart(othersCryptoProfit);

		Assert.Equal(0M, othersProfitChartWithNoProfitInput.NonCryptoOthersProfit);
		Assert.Equal(140M, othersProfitChartWithNoProfitInput.MiningProfit);
		Assert.Equal(50M, othersProfitChartWithNoProfitInput.StakingProfit);
		Assert.Equal(66M, othersProfitChartWithNoProfitInput.FreeLimitOthersProfit);
	}

	/// <summary>
	/// Tests different aspects of the <see cref="GenerateSaleProfitChart"/> function in <see cref="TaxCalculationsService.cs"/> file.
	/// </summary>
	/// <remarks>
	/// It tests if the data to create sale profit pie chart in View <see cref="TaxRecommendation"/> is calculated error free,
	/// both when <see cref="NonCryptoSaleProfit"/> is given and not given into the function. Default value must be zero.
	/// </remarks>
	[Fact]
	public void TestGenerateSaleProfitChart() {
		var date = new DateTime(year: 2023, month: 07, day: 30);
		var afterSaleTaxDetails = CalculationService.CalculateAfterSaleTaxDetails(TransactionsList2VM, date);
		var taxDetails = CalculationService.GenerateSaleTaxDetails(afterSaleTaxDetails, 2023);
		var saleProfitChart = CalculationService.GenerateSaleProfitChart(taxDetails, 10M);

		Assert.Equal(10M, saleProfitChart.NonCryptoSaleProfit);
		Assert.Equal(-110M, saleProfitChart.CryptoSaleProfit);
		Assert.Equal(700M, saleProfitChart.FreeLimitSaleProfit);

		var saleProfitChartWithNoProfitInput = CalculationService.GenerateSaleProfitChart(taxDetails);

		Assert.Equal(0M, saleProfitChartWithNoProfitInput.NonCryptoSaleProfit);
		Assert.Equal(-110M, saleProfitChartWithNoProfitInput.CryptoSaleProfit);
		Assert.Equal(710M, saleProfitChartWithNoProfitInput.FreeLimitSaleProfit);
	}

	/// <summary>
	/// Tests different aspects of the <see cref="CalculateEarningsPerMonth"/> function in <see cref="TaxCalculationsService.cs"/> file.
	/// </summary>
	/// <remarks>
	/// It tests if the data to create dashboard bar chart in View <see cref="Dashboard"/> is calculated error-free.
	/// </remarks>
	[Fact]
	public void TestCalculateEarningsPerMonth() {
		var dashboardData = CalculationService.CalculateEarningsPerMonth(TransactionsList2VM, 2023);

		Assert.Equal(1740M, dashboardData.TotalExpenses);
		Assert.Equal(1945M, dashboardData.TotalIncome);
	}

	/// <summary>
	/// Tests different aspects of the <see cref="GenerateRecommendationAssetsTable"/> function in <see cref="TaxCalculationsService.cs"/> file.
	/// </summary>
	/// /// <remarks>
	/// It tests if the data for the recommendation table in View <see cref="TaxRecommendation"/> is calculated error-free.
	/// </remarks>
	[Fact]
	public void TestGenerateRecommendationAssetsTable() {
		var date = new DateTime(year: 2023, month: 07, day: 30);
		var afterSaleTaxDetails = CalculationService.CalculateAfterSaleTaxDetails(TransactionsList3VM, date);
		var assetsTableData = CalculationService.GenerateRecommendationAssetsTable(afterSaleTaxDetails);

		Assert.Equal(2, assetsTableData.Count());

		Assert.Contains("BTC", assetsTableData.ConvertAll(x => x.CryptoCurrency));
		Assert.Contains("DOGE", assetsTableData.ConvertAll(x => x.CryptoCurrency));

		Assert.Equal(25M, assetsTableData.Where(x => x.CryptoCurrency == "BTC").First().AvaliableCoinAmount);
		Assert.Equal(45M, assetsTableData.Where(x => x.CryptoCurrency == "DOGE").First().AvaliableCoinAmount);

		Assert.True(assetsTableData.Where(x => x.CryptoCurrency == "BTC").First().IsTaxFree);
		Assert.False(assetsTableData.Where(x => x.CryptoCurrency == "DOGE").First().IsTaxFree);

		Assert.Equal(15M, assetsTableData.Where(x => x.CryptoCurrency == "BTC").First().Transactions.Where(x => x.Tipps == TippsEnum.profitTaxFreeTipp).First().ExpectedGain);
		Assert.Equal(-110M, assetsTableData.Where(x => x.CryptoCurrency == "BTC").First().Transactions.Where(x => x.Tipps == TippsEnum.lossTaxFreeTipp).First().ExpectedGain);

		Assert.Equal(75M, assetsTableData.Where(x => x.CryptoCurrency == "DOGE").First().Transactions.Where(x => x.Tipps == TippsEnum.profitTaxLiableTipp).First().ExpectedGain);
		Assert.Equal(-140M, assetsTableData.Where(x => x.CryptoCurrency == "DOGE").First().Transactions.Where(x => x.Tipps == TippsEnum.lossTaxLiableTipp).First().ExpectedGain);

	}

	/// <summary>
	/// Tests different aspects of the <see cref="GenerateRecommendationAssetsChart"/> function in <see cref="TaxCalculationsService.cs"/> file.
	/// </summary>
	/// /// <remarks>
	/// It tests if the data for the overview of assets with due dates in View <see cref="TaxRecommendation"/> is calculated error-free.
	/// </remarks>
	[Fact]
	public void TestGenerateRecommendationAssetsChart() {
		var date = new DateTime(year: 2023, month: 07, day: 30);
		var afterSaleTaxDetails = CalculationService.CalculateAfterSaleTaxDetails(TransactionsList4VM, date);
		var assetsTableData = CalculationService.GenerateRecommendationAssetsTable(afterSaleTaxDetails);
		var assetsChartData = CalculationService.GenerateRecommendationAssetsChart(assetsTableData);

		Assert.Equal(30, assetsChartData[0].DataSets[0].Data[0]);
		Assert.Equal(30, assetsChartData[0].DataSets[0].Data[1]);
		Assert.Equal(40, assetsChartData[0].DataSets[0].Data[2]);
		Assert.Equal(110, assetsChartData[0].DataSets[0].Data[3]);

		Assert.Equal("Steuerfrei", assetsChartData[0].Labels[0]);
		Assert.Equal("Restliche Coins", assetsChartData[0].Labels[3]);
	}
}
