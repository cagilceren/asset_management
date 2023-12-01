using Microsoft.EntityFrameworkCore;

using NACTAM.Exceptions;
using NACTAM.Identity.Data;
using NACTAM.Models.Repositories;
// use this instead of Moq.EntityFrameworkCore
using NACTAM.UnitTestExtension;

namespace NACTAM.UnitTests;

/// <summary>
/// Class for testing some methods of the Transaction container
///
/// author: Tuan Bui
/// </summary>
public class TransactionContainerTest {
	public static readonly Fixture Fixture = new Fixture();

	public List<Transaction> TransactionList;
	public ITransactionRepository TransactionRepository;

	public Mock<NACTAMContext> NactamContext;

	public UserContainer UserContainer;

	public TransactionContainerTest() {
		NactamContext = new Mock<NACTAMContext>();
		TransactionList = new List<Transaction> {
			new Transaction(1,  "coolid", 20000M, 1.80M, 0.009M, TransactionType.Buy, new DateTime(2021, 1, 1), 1),
			new Transaction(2,  "coolid", 0.1M, 30.56M, 30562M, TransactionType.Buy, new DateTime(2022, 5, 1), 2),
			new Transaction(3,  "coolid", 6M, 173.82M, 2897M, TransactionType.Buy, new DateTime(2022, 2, 9), 3),
			new Transaction(4,  "coolid", 0.03M, 13.07M, 43574M, TransactionType.Sell, new DateTime(2022, 2, 10), 2),
			new Transaction(5,  "coolid", 12M, 15.12M, 0.126M, TransactionType.Sell, new DateTime(2022, 4, 1), 1),
			new Transaction(6,  "coolid", 0.02M, 3.69M, 18484M, TransactionType.Buy, new DateTime(2022, 7, 6), 2),
			new Transaction(7,  "coolid", 3M, 31.41M, 1047M, TransactionType.Buy, new DateTime(2022, 7, 6), 3),
			new Transaction(8,  "coolid", 10M, 7.10M, 0.071M, TransactionType.Buy, new DateTime(2022, 8, 15), 1),
			new Transaction(9,  "coolid", 10M, 115.90M, 1159M, TransactionType.Sell, new DateTime(2023, 1, 8), 3),
			new Transaction(10, "coolid", 10M, 7.50M, 0.075M, TransactionType.Buy, new DateTime(2023, 1, 13), 1),
			new Transaction(11, "coolid", 2M, 29.88M, 1494M, TransactionType.Sell, new DateTime(2023, 3, 13), 3),
			new Transaction(12, "coolid", 19M, 16.91M, 0.089M, TransactionType.Sell, new DateTime(2023, 4, 4), 1),
			new Transaction(13, "coolid", 0.03M, 7.62M, 24291M, TransactionType.Sell, new DateTime(2023, 5, 10), 2),
			new Transaction(14, "coolid", 0.01M, 2.42M, 24291M, TransactionType.Buy, new DateTime(2023, 5, 12), 2)
		};
		NactamContext
			.Setup(x => x.Transaction)
			.ReturnsDbSet(TransactionList);
		TransactionRepository = new TransactionContainer(NactamContext.Object);
	}


	[Fact]
	public async void TestCreateInvalidTransaction() {
		var ex = await Assert.ThrowsAsync<TransactionBalanceException>(async () =>
			await TransactionRepository.AddTransaction("coolid", new Transaction(15, "coolid", 20000M, 1.80M, 0.009M, TransactionType.Sell, new DateTime(2021, 1, 1), 2))
		);
		Assert.Equal("Transaction is not valid", ex.Message);
		Assert.Null(TransactionList.FirstOrDefault(x => x.Id == 15));
		Assert.Equal(14, TransactionList.Count());
	}

	[Fact]
	public async void TestCreateInvalidTransactionDate() {
		var ex = await Assert.ThrowsAsync<IllegalDateException>(async () =>
			await TransactionRepository.AddTransaction("coolid", new Transaction(15, "coolid", 20000M, 1.80M, 0.009M, TransactionType.Buy, DateTime.Now + TimeSpan.FromDays(5), 2))
		);
		Assert.Equal("Transaction is not valid", ex.Message);
		Assert.Null(TransactionList.FirstOrDefault(x => x.Id == 15));
		Assert.Equal(14, TransactionList.Count());
	}

	[Fact]
	public async void TestCreateValidTransaction() {
		await TransactionRepository.AddTransaction("coolid", new Transaction(15, "coolid", 20000M, 1.80M, 0.009M, TransactionType.Buy, DateTime.Now - TimeSpan.FromDays(5), 2));
		var queriedTransaction = TransactionList.FirstOrDefault(x => x.Id == 15);
		Assert.NotNull(queriedTransaction);
		Assert.Equal(15, TransactionList.Count());
		Assert.Equal(20000M, queriedTransaction.Amount);
		Assert.Equal(1.80M, queriedTransaction.Fee);
	}

	[Fact]
	public async void TestRemoveInvalidTransaction() {
		var ex = await Assert.ThrowsAsync<TransactionBalanceException>(async () =>
			await TransactionRepository.RemoveTransaction("coolid", 1)
		);
		Assert.Equal("Transaction is not valid", ex.Message);
		Assert.NotNull(TransactionList.FirstOrDefault(x => x.Id == 1));
		Assert.Equal(14, TransactionList.Count());
	}

	[Fact]
	public async void TestRemoveNonExistantTransaction() {
		var ex = await Assert.ThrowsAsync<TransactionNotFoundException>(async () =>
			await TransactionRepository.RemoveTransaction("coolid", 22)
		);
		Assert.Equal("Transaction not found", ex.Message);
		Assert.Equal(14, TransactionList.Count());
	}

	[Fact]
	public async void TestRemoveTransaction() {
		await TransactionRepository.RemoveTransaction("coolid", 13);
		Assert.True(TransactionList.First(x => x.Id == 13).Deleted);
		Assert.Equal(14, TransactionList.Count()); // Soft delete
	}
}
