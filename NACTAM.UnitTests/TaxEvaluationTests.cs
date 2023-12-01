using NACTAM.ViewModels.TaxEvaluation;
using NACTAM.ViewModels.TaxRecommendation.AfterSaleTaxDetails;

namespace NACTAM.UnitTests;

public class TaxEvaluationTests {
	/// <summary>
	/// Unit Tests for relevant functions in <see cref="TaxEvaluationViewModel.cs"/>
	/// </summary>
	/// <author> Mervan Kilic </author>
	public List<AfterSaleSellTransactionViewModel> Transactions;

	public TaxEvaluationTests() {
		Transactions = new() {
			new() {
				BuyDate = new DateTime(2022, 5, 5),
				SellDate = new DateTime(2022, 9, 5),
				SoldAmount = 7,
				CryptoCurrency = "Bitcoin",
				Gain = 10,
				TotalFee = 1,
			},
			new() {
				BuyDate = new DateTime(2021, 5, 5),
				SellDate = new DateTime(2022, 9, 5),
				SoldAmount = 3,
				CryptoCurrency = "Dogecoin",
				Gain = 40,
				TotalFee = 2,
			},
			new() {
				BuyDate = new DateTime(2023, 5, 5),
				SellDate = new DateTime(2022, 9, 5),
				SoldAmount = 10,
				CryptoCurrency = "Bitcoin",
				Gain = -5,
				TotalFee = 3,
			},
		};
	}

	[Fact]
	public async void TestSumUp() {
		decimal profit = 0, loss = 0, fee = 0;

		TaxEvaluationViewModel.SumUp(Transactions, ref profit, ref loss, ref fee);

		Assert.Equal(50, profit);
		Assert.Equal(5, loss);
		Assert.Equal(6, fee);
	}

	[Fact]
	public async void TestFormatToEuro() {
		decimal number = 5.03750M;

		var newNumber = TaxEvaluationViewModel.FormatToEuro(number);

		Assert.Equal(5.04M, newNumber);
	}
}