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
/// author: Philipp Eckel
/// </summary>
public class CurrencyContainerTest {
	public static readonly Fixture Fixture = new Fixture();

	public List<CryptoCurrency> CurrencyList;
	public ICurrencyRepository CurrencyRepository;

	public Mock<NACTAMContext> NactamContext;

	public UserContainer UserContainer;

	public CurrencyContainerTest() {
		DateTime lastWeek = DateTime.Now - TimeSpan.FromDays(7);
		NactamContext = new Mock<NACTAMContext>();
		CurrencyList = new List<CryptoCurrency> {
			new CryptoCurrency() {
				Id = 1, ShortName = "BTC", ApiId = "bitcoin", Name = "Bitcoin",
				Logo = "https://assets.coingecko.com/coins/images/1/large/bitcoin.png?1547033579", Rate = 1,
				LastUpdated = DateTime.Now
			},
			new CryptoCurrency() {
				Id = 2, ShortName = "ETH", ApiId = "ethereum", Name = "Ethereum",
				Logo = "https://assets.coingecko.com/coins/images/279/large/ethereum.png?1595348880", Rate = 1,
				LastUpdated = lastWeek
			},
			new CryptoCurrency() { Id = 3, ShortName = "XRP", ApiId = "ripple", Name = "XRP" },
		};
		NactamContext
			.Setup(x => x.CryptoCurrency)
			.ReturnsDbSet(CurrencyList);
		CurrencyRepository = new CurrencyContainer(NactamContext.Object);
	}

	[Fact]
	public async void TestGetAllCurrencies() {
		var currencies = await CurrencyRepository.GetCurrenciesAsync();
		Assert.Equal(3, currencies.Count());
	}

	[Fact]

	public async void TestGetCurrencyById() {
		var currency = await CurrencyRepository.GetCurrencyAsync(1);
		Assert.Equal("Bitcoin", currency.Name);
	}

	[Fact]

	public async void TestUpdateCurrencyName() {
		var currency = await CurrencyRepository.GetCurrencyAsync(1);
		currency.Name = "BitCoin";
		await CurrencyRepository.UpdateCurrencyAsync(currency);
		Assert.Equal("BitCoin", CurrencyList.FirstOrDefault(x => x.Id == 1).Name);
	}


	[Fact]
	public async void TestUpdateCurrencyRate() {
		var currency = await CurrencyRepository.GetCurrencyAsync(1);
		currency.Rate = 2;
		await CurrencyRepository.UpdateCurrencyAsync(currency);
		Assert.Equal(2, CurrencyList.FirstOrDefault(x => x.Id == 1).Rate);
	}

	[Fact]

	public async void TestUpdateAddCurrency() {
		var currency = new CryptoCurrency() { Id = 4, ShortName = "XLM", ApiId = "stellar", Name = "Stellar" };
		await CurrencyRepository.UpdateCurrencyAsync(currency);
		Assert.Equal(4, CurrencyList.Count());
		Assert.Equal("Stellar", CurrencyList.FirstOrDefault(x => x.Id == 4).Name);
	}

	[Fact]
	public async void TestUpdateInvalidCurrencyOutdatet() {
		var currency = new CryptoCurrency() {
			Id = 1,
			ShortName = "BTC",
			ApiId = "bitcoin",
			Name = "Bitcoin",
			Logo = "https://assets.coingecko.com/coins/images/1/large/bitcoin.png?1547033579",
			Rate = 42,
			LastUpdated = DateTime.Now - TimeSpan.FromDays(7)
		};
		await CurrencyRepository.UpdateCurrencyAsync(currency);
		Assert.Equal(3, CurrencyList.Count());
		Assert.Equal(1, CurrencyList.FirstOrDefault(x => x.Id == 1).Rate);
	}

	[Fact]

	public async void TestUpdateInvalidCurrencyDontRemoveIcon() {
		var currency = new CryptoCurrency() { Id = 1, ShortName = "BTC", ApiId = "bitcoin", Name = "Bitcoin", Rate = 42, LastUpdated = DateTime.Now };
		await CurrencyRepository.UpdateCurrencyAsync(currency);
		Assert.Equal(3, CurrencyList.Count());
		Assert.NotNull(CurrencyList.FirstOrDefault(x => x.Id == 1).Logo);
	}

	[Fact]

	public async void TestUpdateInvalidCurrencyDontRemoveRateAtInit() {
		var currency = new CryptoCurrency() { Id = 1, ShortName = "BTC", ApiId = "bitcoin", Name = "Bitcoin", Rate = 42, LastUpdated = DateTime.Now };
		List<CryptoCurrency> initlist = new List<CryptoCurrency> {
			new CryptoCurrency() { Id = 1, ShortName = "BTC", ApiId = "bitcoin", Name = "Bitcoin" },
			new CryptoCurrency() { Id = 2, ShortName = "ETH", ApiId = "ethereum", Name = "Ethereum" },
		};
		await CurrencyRepository.UpdateCurrencyAsync(initlist);
		Assert.NotNull(CurrencyList.FirstOrDefault(x => x.Id == 1).Rate);
		//Assert.NotNull(CurrencyList.FirstOrDefault(x => x.Id == 2).Rate);
	}

	[Fact]

	public async void TestInitCurrency() {
		CurrencyList.Clear();
		List<CryptoCurrency> initlist = new List<CryptoCurrency> {
			new CryptoCurrency() { Id = 1, ShortName = "ICO", ApiId = "isse", Name = "issecoin" },
			new CryptoCurrency() { Id = 2, ShortName = "NTM", ApiId = "nactam", Name = "Nactamcoin" },
		};
		Assert.Empty(CurrencyList);
		await CurrencyRepository.UpdateCurrencyAsync(initlist);
		Assert.Equal(2, CurrencyList.Count());
		Assert.Equal("issecoin", CurrencyList.FirstOrDefault(x => x.Id == 1).Name);
		Assert.Equal("Nactamcoin", CurrencyList.FirstOrDefault(x => x.Id == 2).Name);

	}

	[Fact]

	public async void TestUpdateMixed() {
		List<CryptoCurrency> initlist = new List<CryptoCurrency> {
			new CryptoCurrency() { Id = 1, ShortName = "BTC", ApiId = "bitcoin", Name = "Bitcoin"},
			new CryptoCurrency() { Id = 4, ShortName = "NTM", ApiId = "nactam", Name = "Nactamcoin" },
		};
		await CurrencyRepository.UpdateCurrencyAsync(initlist);
		Assert.Equal(4, CurrencyList.Count());
		Assert.Equal("Bitcoin", CurrencyList.FirstOrDefault(x => x.Id == 1).Name);
		Assert.Equal("Nactamcoin", CurrencyList.FirstOrDefault(x => x.Id == 4).Name);
	}

	[Fact]
	public async void GetCurrencyByNameInvalid() {
		//wrong names throws InvalidCryptoCurrencyException
		await Assert.ThrowsAsync<CurrencyNotFoundException>(async () => await CurrencyRepository.GetCurrencyByNameAsync("bkjbhvhjvhvhvgjvcjgcvgjvhjgvjgvghjcvhgchgxfhzxfzxfzhx"));
	}

	[Fact]
	public async void TestDontUpdateDateIfThereIsNoRate() {
		CurrencyList.Clear();
		CurrencyList = new List<CryptoCurrency> {
			new CryptoCurrency() {
				Id = 2, ShortName = "ETH", ApiId = "ethereum", Name = "Ethereum"
			},
		};
		CryptoCurrency currency = new CryptoCurrency() {
			Id = 2,
			ShortName = "ETH",
			ApiId = "ethereum",
			Name = "Ethereum",
			Logo = "https://assets.coingecko.com/coins/images/279/large/ethereum.png?1595348880"
		};
		await CurrencyRepository.UpdateCurrencyAsync(currency);
		Assert.Null(CurrencyList.FirstOrDefault(x => x.Id == 2).LastUpdated);
	}

}
