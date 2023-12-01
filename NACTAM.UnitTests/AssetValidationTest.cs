using Microsoft.EntityFrameworkCore;

using NACTAM.Exceptions;
using NACTAM.Identity.Data;
using NACTAM.Models.Repositories;
using NACTAM.UnitTestExtension;

namespace NACTAM.UnitTests;

/// <summary>
/// Tests validation methods in TransactionContainer
/// Authornames: Marco Lembert
/// </summary>
public class AssetValidationTest {

	public static readonly Fixture Fixture = new Fixture();
	public List<Transaction> TransactionList;
	public ITransactionRepository TransactionRepository;
	public Mock<NACTAMContext> NactamContext;
	public UserContainer UserContainer;

	public AssetValidationTest() {
		NactamContext = new Mock<NACTAMContext>();
		TransactionList = new List<Transaction> {
			new Transaction(1,  "coolid", 20000M, 1.80M, 0.009M, TransactionType.Buy, new DateTime(2021, 1, 1), 1),
			new Transaction(2,  "coolid", 0.1M, 30.56M, 30562M, TransactionType.Buy, new DateTime(2022, 5, 1), 2),
			new Transaction(3,  "coolid", 6M, 173.82M, 2897M, TransactionType.Buy, new DateTime(2022, 2, 9), 3),
			new Transaction(4,  "coolid", 1000M, 1.80M, 0.009M, TransactionType.Sell, new DateTime(2021, 1, 1), 1),
			new Transaction(5,  "coolid", 3M, 173.82M, 2897M, TransactionType.Buy, new DateTime(2022, 2, 9), 3),
			new Transaction(6,  "coolid", 5M, 173.82M, 2897M, TransactionType.Sell, new DateTime(2022, 2, 9), 3),
		};
		NactamContext
			.Setup(x => x.Transaction)
			.ReturnsDbSet(TransactionList);
		TransactionRepository = new TransactionContainer(NactamContext.Object);
	}

	[Fact]
	public async void TestTransaction_InvalidDate() {
		var validate = TransactionRepository.ValidateTransactionDate(new Transaction(10, "coolid", 1m, 0.2m, 1000m, TransactionType.Buy, new DateTime(2024, 1, 1), 1));
		Assert.False(validate);
	}

	[Fact]
	public async void TestTransaction_ValidDate() {
		var validate = TransactionRepository.ValidateTransactionDate(new Transaction(10, "coolid", 1m, 0.2m, 1000m, TransactionType.Buy, new DateTime(2023, 1, 1), 1));
		Assert.True(validate);
	}

	[Fact]
	public async void TestTransaction_InvalidBalance() {
		// only 6 avaible, trying to sell 7
		var validate = TransactionRepository.ValidateTransactionBalance(new Transaction(10, "coolid", 7m, 0.2m, 1000m, TransactionType.Sell, new DateTime(2023, 1, 1), 3), "coolid");
		Assert.False(validate);
	}

	[Fact]
	public async void TestTransaction_ValidBalance() {
		var validate = TransactionRepository.ValidateTransactionBalance(new Transaction(10, "coolid", 5m, 0.2m, 1000m, TransactionType.Sell, new DateTime(2023, 1, 1), 1), "coolid");
		Assert.True(validate);
	}

	[Fact]
	public async void TestTransaction_InvalidDeleteBalance() {
		var validate = TransactionRepository.ValidateTransactionBalanceDelete(new Transaction(1, "coolid", 20000M, 1.80M, 0.009M, TransactionType.Buy, new DateTime(2021, 1, 1), 1), "coolid");
		Assert.False(validate);
	}

	[Fact]
	public async void TestTransaction_ValidDeleteBalance() {
		var validate = TransactionRepository.ValidateTransactionBalanceDelete(new Transaction(5, "coolid", 3M, 173.82M, 2897M, TransactionType.Buy, new DateTime(2022, 2, 9), 3), "coolid");
		Assert.True(validate);
	}
}
