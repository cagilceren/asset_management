using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using NACTAM.Controllers;
using NACTAM.Models.API;
using NACTAM.Models.Repositories;
using NACTAM.ViewModels;

using Xunit;

namespace NACTAM.UnitTests {
	public class AssetsTest {

		public TransactionController TransControllerUser1;
		public TransactionController TransControllerUser2;
		public TransactionController TransControllerUser3;
		public IEnumerable<Transaction> Trans1;

		public AssetsTest() {
			// Mock attributes for Transaction Controller:
			var transRep = new MockTransactionRepository();
			var api = new MockICurrencyApi();

			// Create Controller User 1:
			TransControllerUser1 = new TransactionController(transRep, api) {
				ControllerContext = new ControllerContext() {
					HttpContext = new DefaultHttpContext()
				}
			};
			var mockUser1 = new Mock<ClaimsPrincipal>();
			var userIdClaim1 = new Claim(ClaimTypes.NameIdentifier, "1");
			mockUser1.Setup(x => x.FindFirst(It.IsAny<string>())).Returns(userIdClaim1);
			TransControllerUser1.ControllerContext.HttpContext.User = mockUser1.Object;

			// Create Controller User 2:
			TransControllerUser2 = new TransactionController(transRep, api) {
				ControllerContext = new ControllerContext() {
					HttpContext = new DefaultHttpContext()
				}
			};
			var mockUser2 = new Mock<ClaimsPrincipal>();
			var userIdClaim2 = new Claim(ClaimTypes.NameIdentifier, "2");
			mockUser2.Setup(x => x.FindFirst(It.IsAny<string>())).Returns(userIdClaim2);
			TransControllerUser2.ControllerContext.HttpContext.User = mockUser2.Object;

			// Create Controller User 3:
			TransControllerUser3 = new TransactionController(transRep, api) {
				ControllerContext = new ControllerContext() {
					HttpContext = new DefaultHttpContext()
				}
			};
			var mockUser3 = new Mock<ClaimsPrincipal>();
			var userIdClaim3 = new Claim(ClaimTypes.NameIdentifier, "3");
			mockUser3.Setup(x => x.FindFirst(It.IsAny<string>())).Returns(userIdClaim3);
			TransControllerUser3.ControllerContext.HttpContext.User = mockUser3.Object;
		}

		[Fact]
		public async Task AssetCalulationTestAsync() {
			// Act User1
			var resultUser1 = await TransControllerUser1.Assets() as ViewResult;
			var modelUser1 = resultUser1.Model as AssetsOverviewModel;
			var assetsUser1 = modelUser1.Assets;

			// Act User2
			var resultUser2 = await TransControllerUser2.Assets() as ViewResult;
			var modelUser2 = resultUser2.Model as AssetsOverviewModel;
			var assetsUser2 = modelUser2.Assets;

			// Act User3
			var resultUser3 = await TransControllerUser3.Assets() as ViewResult;
			var modelUser3 = resultUser3.Model as AssetsOverviewModel;
			var assetsUser3 = modelUser3.Assets;

			// Assert
			var assetUser1BTC = assetsUser1.FirstOrDefault(x => x.ShortName == "BTC");
			var assetUser1ETH = assetsUser1.FirstOrDefault(x => x.ShortName == "ETH");
			Assert.Equal(5, assetUser1BTC.Amount);
			Assert.Equal(75, assetUser1BTC.Value);
			Assert.Equal(6, assetUser1ETH.Amount);
			Assert.Equal(60, assetUser1ETH.Value);

			var assetUser2BTC = assetsUser2.FirstOrDefault(x => x.ShortName == "BTC");
			Assert.Equal(1, assetUser2BTC.Amount);
			Assert.Equal(15, assetUser2BTC.Value);

			var assetUser3BTC = assetsUser3.FirstOrDefault(x => x.ShortName == "BTC");
			Assert.Equal(14, assetUser3BTC.Amount);
			Assert.Equal(210, assetUser3BTC.Value);
		}
	}

	internal class MockICurrencyApi : ICurrencyApi {

		public List<CryptoCurrency> Currencies = new List<CryptoCurrency>();

		public MockICurrencyApi() {
			Currencies.Add(new CryptoCurrency() {
				Id = 1,
				ShortName = "BTC",
				ApiId = "1",
				Name = "Bitcoin"
			});
			Currencies.Add(new CryptoCurrency() {
				Id = 2,
				ShortName = "ETH",
				ApiId = "2",
				Name = "Ethereum"
			});
		}

		List<decimal> ICurrencyApi.GetCoinMarketChart(int id, int days, string interval, string precision) {
			throw new NotImplementedException();
		}

		List<CryptoCurrency> ICurrencyApi.GetCryptoCurrencies() {
			return new List<CryptoCurrency>();
		}

		CryptoCurrency ICurrencyApi.GetCryptoCurrency(int id) {
			return Currencies.FirstOrDefault(x => x.Id == id);
		}

		CryptoCurrency ICurrencyApi.GetCryptoCurrencyByName(string name) {
			throw new NotImplementedException();
		}

		decimal ICurrencyApi.GetExchangeRate(int id) {
			if (id == 1) {
				return 15;
			} else {
				return 10;
			}
		}

		decimal ICurrencyApi.GetExchangeRate(int id, DateTime date) {
			throw new NotImplementedException();
		}

		List<decimal> ICurrencyApi.GetExchangeRates(List<int> ids) {
			return new List<decimal>();
		}

		Task ICurrencyApi.LoadCryptoCurrenciesAsync() {
			throw new NotImplementedException();
		}

		List<CryptoCurrency> ICurrencyApi.SearchCryptoCurrency(string query) {
			return new List<CryptoCurrency>();
		}

		List<CryptoCurrency> ICurrencyApi.SearchCryptoCurrency(List<string> query) {
			throw new NotImplementedException();
		}

		Task ICurrencyApi.UpdateRates() {
			throw new NotImplementedException();
		}

		Task ICurrencyApi.UpdateRates(List<int> ids) {
			throw new NotImplementedException();
		}
	}

	internal class MockTransactionRepository : ITransactionRepository {

		public List<Transaction> Transactions = new List<Transaction>();

		public MockTransactionRepository() {

			Transactions.Add(new Transaction("1", 10, 0, 10, TransactionType.Buy, DateTime.Now, 1));
			Transactions.Add(new Transaction("1", 5, 0, 12, TransactionType.Sell, DateTime.Now, 1));
			Transactions.Add(new Transaction("1", 10, 0, 5, TransactionType.Buy, DateTime.Now, 2));
			Transactions.Add(new Transaction("1", 4, 0, 8, TransactionType.Sell, DateTime.Now, 2));

			Transactions.Add(new Transaction("2", 10, 0, 10, TransactionType.Buy, DateTime.Now, 1));
			Transactions.Add(new Transaction("2", 9, 0, 30, TransactionType.Sell, DateTime.Now, 1));

			Transactions.Add(new Transaction("3", 10, 0, 10, TransactionType.Buy, DateTime.Now, 1));
			Transactions.Add(new Transaction("3", 2, 0, 10, TransactionType.Mining, DateTime.Now, 1));
			Transactions.Add(new Transaction("3", 2, 0, 10, TransactionType.Staking, DateTime.Now, 1));

		}

		Task ITransactionRepository.AddTransaction(string userid, Transaction transaction) {
			throw new NotImplementedException();
		}

		IEnumerable<Transaction> ITransactionRepository.GetDeletedTransactions(string userid) {
			throw new NotImplementedException();
		}

		Transaction ITransactionRepository.GetTransaction(int id, string userId) {
			throw new NotImplementedException();
		}

		IEnumerable<Transaction> ITransactionRepository.GetTransactions(string userid) {
			return Transactions.Where(x => x.UserId == userid);
		}

		Task ITransactionRepository.RemoveTransaction(string userid, int id) {
			throw new NotImplementedException();
		}

		Task ITransactionRepository.UpdateTransaction(string userid, Transaction transaction) {
			throw new NotImplementedException();
		}

		bool ITransactionRepository.ValidateTransactionBalance(Transaction transaction, string userid) {
			throw new NotImplementedException();
		}

		bool ITransactionRepository.ValidateTransactionBalanceDelete(Transaction transaction, string userid) {
			throw new NotImplementedException();
		}

		bool ITransactionRepository.ValidateTransactionDate(Transaction transaction) {
			throw new NotImplementedException();
		}
	}
}