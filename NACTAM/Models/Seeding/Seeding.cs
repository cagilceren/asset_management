using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NACTAM.Identity.Data;
using NACTAM.Models;
using NACTAM.Models.API;
using NACTAM.Models.Repositories;

namespace NACTAM.Seeding {
	/// <summary>
	/// Class for seeding data
	///
	/// author: Tuan Bui, Phillipp Eckel
	/// </summary>
	public class Seeding : ISeeding {
		NACTAMContext _dbContext;
		RoleManager<IdentityRole> _roleManager;
		UserManager<User> _userManager;
		UserManager<PrivatePerson> _privatePersonManager;
		UserManager<TaxAdvisor> _taxAdvisorManager;
		UserManager<Admin> _adminManager;
		IUserRepository _userRep;
		ITaxAdvisorRepository _taxRep;
		INotificationRepository _notRep;
		ITransactionRepository _transRep;
		ICurrencyApi _currencyApi;
		ICurrencyRepository _curRep;

		public Seeding(NACTAMContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, UserManager<PrivatePerson> privatePersonManager, UserManager<TaxAdvisor> taxAdvisorManager, UserManager<Admin> adminManager, IUserRepository userRep, ITaxAdvisorRepository taxRep, INotificationRepository notRep, ITransactionRepository transRep, ICurrencyApi api, ICurrencyRepository curRep) {
			_dbContext = dbContext;
			_roleManager = roleManager;
			_userManager = userManager;
			_userRep = userRep;
			_notRep = notRep;
			_taxRep = taxRep;
			_transRep = transRep;
			_privatePersonManager = privatePersonManager;
			_taxAdvisorManager = taxAdvisorManager;
			_adminManager = adminManager;
			_currencyApi = api;
			_curRep = curRep;
		}

		/// <inheritdoc/>
		public async Task Init() {
			// Here is the migration executed
			_dbContext.Database.Migrate();

			string[] roleNames = { "Admin", "PrivatePerson", "TaxAdvisor" };
			IdentityResult roleResult;

			foreach (var roleName in roleNames) {
				var roleExist = await _roleManager.RoleExistsAsync(roleName);
				if (!roleExist) {
					//create the roles and seed them to the database: Question 1
					roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			var privatePersons = new (PrivatePerson, string)[]{
				(new PrivatePerson { UserName = "User1", Email = "dreaugs@gmail.com", PhoneNumber = "+11111111", FirstName = "Somewon", LastName = "Newtest", EmailNotification = true }, "Test$123"),
				(new PrivatePerson { UserName = "User2", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "LEo", LastName = "Poellele", EmailNotification = false }, "Test$123"),
				(new PrivatePerson { UserName = "User3", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Yeetus", LastName = "Deletus", EmailNotification = false }, "Test$123"),
				(new PrivatePerson { UserName = "User4", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Irgendein", LastName = "HackerOderSo", EmailNotification = false }, "Test$123"),
				(new PrivatePerson { UserName = "empfehlung", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Steuerliche", LastName = "Empfehlung", EmailNotification = false }, "Test$123"),
				(new PrivatePerson { UserName = "mazars", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Bespiel", LastName = "Mazars", EmailNotification = false }, "Test$123"),
				(new PrivatePerson { UserName = "bergmann", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Stefan", LastName = "Bergmann", EmailNotification = false }, "Bergmann123!\"§"),
				(new PrivatePerson { UserName = "spekulatius", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Tamara", LastName = "Spekulatius", EmailNotification = false }, "Spekulatius123!\"§"),
				(new PrivatePerson { UserName = "neuberg", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Susi", LastName = "Neuberg", EmailNotification = false }, "Neuberg123!\"§"),
			};
			foreach (var (person, password) in privatePersons) {
				if (await _privatePersonManager.FindByNameAsync(person.UserName) == null) {
					var result = await _userRep.AddPrivatePerson(person, password);
					if (!result.Succeeded)
						Console.WriteLine(string.Join(", ", result.Errors
							.Select(e => e.Description)));
				}
			}

			var admins = new (Admin, string)[]{
				(new Admin { UserName = "Admin1", Email = "test@test.com", PhoneNumber = "+11111111", FirstName = "Somewon", LastName = "Newadmin", EmailNotification = false }, "SuperSecurePassword$123"),
			};

			foreach (var (person, password) in admins) {
				if (await _adminManager.FindByNameAsync(person.UserName) == null) {
					var result = await _userRep.AddAdmin(person, password);
					if (!result.Succeeded)
						Console.WriteLine(string.Join(", ", result.Errors
							.Select(e => e.Description)));
				}
			}

			var taxAdvisors = new (TaxAdvisor, string)[] {
				(new TaxAdvisor { UserName = "taxadvisor1", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Markus", LastName = "Mert", EmailNotification = false  }, "Test$123"),
				(new TaxAdvisor { UserName = "taxadvisor2", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Domnique", LastName = "Gregori", EmailNotification = false  }, "Test$123"),
				(new TaxAdvisor { UserName = "taxadvisor3", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Aggro", LastName = "Andreas", EmailNotification = false  }, "Test$123"),
				(new TaxAdvisor { UserName = "taxadvisor4", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Thomas", LastName = "Lok", EmailNotification = false  }, "Test$123"),
				(new TaxAdvisor { UserName = "fiskus", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Jelena", LastName = "Fiskus", EmailNotification = false  }, "Fiskus123!\"§"),
				(new TaxAdvisor { UserName = "geier", Email = "test@test.de", PhoneNumber = "+11111111", FirstName = "Korbinian", LastName = "Geier", EmailNotification = false  }, "Geier123!\"§"),
			};
			foreach (var (person, password) in taxAdvisors) {
				if (await _taxAdvisorManager.FindByNameAsync(person.UserName) == null) {
					var result = await _userRep.AddTaxAdvisor(person, password);
					if (!result.Succeeded)
						Console.WriteLine(string.Join(", ", result.Errors
							.Select(e => e.Description)));

				}
			}

			var combinations = new (string, string, InsightStatus)[] {
				("User1", "taxadvisor1", InsightStatus.SimpleUnaccepted),
				("User1", "taxadvisor2", InsightStatus.SimpleUnaccepted),
				("bergmann", "fiskus", InsightStatus.Simple),
				("spekulatius", "geier", InsightStatus.Extended)
			};
			foreach (var (userName, taxAdvisorName, status) in combinations) {
				var user = await _privatePersonManager.FindByNameAsync(userName);
				var taxAdvisor = await _taxAdvisorManager.FindByNameAsync(taxAdvisorName);

				if (taxAdvisor != null && taxAdvisor.Customers.Count < 1) {
					await _taxRep.AssignUser(taxAdvisor, user.UserName);
					await _taxRep.ChangeAllowanceStatus(taxAdvisor, user.UserName, status);
				}
			}

			var initialCurrencyList = new List<CryptoCurrency> {
				new CryptoCurrency{ ApiId = "bitcoin", Id = 1122, ShortName = "", Name = "" , LastUpdated = DateTime.MinValue, Rate = 0m},
				new CryptoCurrency{ ApiId = "matic-network", Id = 5259, ShortName = "", Name = "", LastUpdated = DateTime.MinValue, Rate = 0m },
				new CryptoCurrency{ ApiId = "binancecoin", Id = 1068, ShortName = "", Name = "", LastUpdated = DateTime.MinValue, Rate = 0m },
				new CryptoCurrency{ ApiId = "ethereum", Id = 3025, ShortName = "", Name = "", LastUpdated = DateTime.MinValue, Rate = 0m },
				new CryptoCurrency{ ApiId = "polkadot", Id = 6809, ShortName = "", Name = "", LastUpdated = DateTime.MinValue, Rate = 0m },
				new CryptoCurrency{ ApiId = "cardano", Id = 1564, ShortName = "", Name = "", LastUpdated = DateTime.MinValue, Rate = 0m  },
				new CryptoCurrency{ ApiId = "terra-luna", Id = 8521, ShortName = "", Name = "", LastUpdated = DateTime.MinValue, Rate = 0m },
			};
			await _curRep.UpdateCurrencyAsync(initialCurrencyList);

			await _dbContext.SaveChangesAsync();
			var transactions = new (string, string, Transaction)[] {
				("bergmann", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 0.5M, CurrencyId = 1122, Date = new DateTime(2022, 9, 7), Fee = 10M, ExchangeRate = 19883.791597090312M }),
				("bergmann", "bitcoin", new Transaction { Type = TransactionType.Mining, Amount = 0.2M, CurrencyId = 1122, Date = new DateTime(2023, 1, 1), Fee = 0M, ExchangeRate = 15479.994747226247M }),
				("bergmann", "bitcoin", new Transaction { Type = TransactionType.Sell, Amount = 0.2M, CurrencyId = 1122, Date = new DateTime(2023, 1, 1), Fee = 3M , ExchangeRate = 15479.994747226247M}),
				("bergmann", "bitcoin",  new Transaction { Type = TransactionType.Sell, Amount = 0.15M, CurrencyId = 1122, Date = new DateTime(2023, 3, 20), Fee = 2M , ExchangeRate = 25156.871487849796M}),
				("spekulatius", "matic-network", new Transaction { Type = TransactionType.Buy, Amount = 100000M, CurrencyId = 5259, Date = new DateTime(2021, 01, 02), Fee = 12.35M, ExchangeRate = 0.014400903823066935M}),
				("spekulatius", "binancecoin", new Transaction { Type = TransactionType.Buy, Amount = 1.20M, CurrencyId = 1068, Date = new DateTime(2021, 01, 03), Fee = 0.23M, ExchangeRate = 31.13308496397931M}),
				("spekulatius", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 0.001664M, CurrencyId = 1122, Date = new DateTime(2021, 01, 18), Fee = 0.04M, ExchangeRate = 30013.42796807724M}),
				("spekulatius", "ethereum", new Transaction { Type = TransactionType.Buy, Amount = 0.03333M, CurrencyId = 3025, Date = new DateTime(2021, 02, 01), Fee = 0.04M, ExchangeRate = 1130.7331869530292M}),
				("spekulatius", "ethereum", new Transaction { Type = TransactionType.Buy, Amount = 0.03333M, CurrencyId = 3025, Date = new DateTime(2021, 02, 01), Fee = 0.05M, ExchangeRate = 1130.7331869530292M}),
				("spekulatius", "ethereum", new Transaction { Type = TransactionType.Buy, Amount = 0.2182M, CurrencyId = 3025, Date = new DateTime(2021, 02, 01), Fee = 0.56M, ExchangeRate = 1130.7331869530292M}),
				("spekulatius", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 0.00897M, CurrencyId = 1122, Date = new DateTime(2021, 02, 06), Fee = 0.04M, ExchangeRate = 30766.304042881016M}),
				("spekulatius", "ethereum", new Transaction { Type = TransactionType.Buy, Amount = 0.07133M, CurrencyId = 3025, Date = new DateTime(2021, 02, 06), Fee = 0.64M, ExchangeRate = 1326.8712262191125M}),
				("spekulatius", "binancecoin", new Transaction { Type = TransactionType.Buy, Amount = 0.258M, CurrencyId = 1068, Date = new DateTime(2021, 02, 09), Fee = 0.23M, ExchangeRate = 56.63945336364103M}),
				("spekulatius", "cardano", new Transaction { Type = TransactionType.Buy, Amount = 35.40M, CurrencyId = 1564, Date = new DateTime(2021, 02, 09), Fee = 0.01M, ExchangeRate = 56.63945336364103M}),
				("spekulatius", "polkadot", new Transaction { Type = TransactionType.Buy, Amount = 0.337M, CurrencyId = 6809, Date = new DateTime(2021, 02, 09), Fee = 0.02M, ExchangeRate = 16.337533305110593M}),
				("spekulatius", "polkadot", new Transaction { Type = TransactionType.Buy, Amount = 0.033M, CurrencyId = 6809, Date = new DateTime(2021, 02, 09), Fee = 0.01M, ExchangeRate = 16.337533305110593M}),
				("spekulatius", "polkadot", new Transaction { Type = TransactionType.Buy, Amount = 0.675M, CurrencyId = 6809, Date = new DateTime(2021, 02, 09), Fee = 0.02M, ExchangeRate = 16.337533305110593M}),
				("spekulatius", "binancecoin", new Transaction { Type = TransactionType.Buy, Amount = 0.48M, CurrencyId = 1068, Date = new DateTime(2021, 02, 09), Fee = 0.34M, ExchangeRate = 56.63945336364103M}),
				("spekulatius", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 0.001569M, CurrencyId = 1122, Date = new DateTime(2021, 02, 09), Fee = 0.23M, ExchangeRate = 32235.555753607303M}),
				("spekulatius", "cardano", new Transaction { Type = TransactionType.Buy, Amount = 67.00M, CurrencyId = 1564, Date = new DateTime(2021, 02, 14), Fee = 0.01M, ExchangeRate = 0.7617685194105724M}),
				("spekulatius", "polkadot", new Transaction { Type = TransactionType.Buy, Amount = 2.09M, CurrencyId = 6809, Date = new DateTime(2021, 02, 14), Fee = 0.06M, ExchangeRate = 23.627686702551028M}),
				("spekulatius", "polkadot", new Transaction { Type = TransactionType.Sell, Amount = 2M, CurrencyId = 6809, Date = new DateTime(2021, 04, 10), Fee = 0.20M, ExchangeRate = 34.74929633021666M}),
				("spekulatius", "binancecoin", new Transaction { Type = TransactionType.Sell, Amount = 0.3M, CurrencyId = 1068, Date = new DateTime(2021, 04, 12), Fee = 0.34M, ExchangeRate = 396.467600022064M}),
				("spekulatius", "cardano", new Transaction { Type = TransactionType.Sell, Amount = 50M, CurrencyId = 1564, Date = new DateTime(2021, 05, 20), Fee = 0.23M, ExchangeRate = 1.6464025198013816M}),
				("spekulatius", "bitcoin", new Transaction { Type = TransactionType.Sell, Amount = 0.001M, CurrencyId = 1122, Date = new DateTime(2021, 05, 20), Fee = 2.31M, ExchangeRate = 35248.29953994585M}),
				("spekulatius", "matic-network", new Transaction { Type = TransactionType.Sell, Amount = 50000M, CurrencyId = 5259, Date = new DateTime(2021, 06, 04), Fee = 621.23M, ExchangeRate = 1.4773202101226985M}),
				("spekulatius", "binancecoin", new Transaction { Type = TransactionType.Sell, Amount = 1.0M, CurrencyId = 1068, Date = new DateTime(2022, 03, 05), Fee = 3.11M, ExchangeRate = 364.16278320714184M}),
				("spekulatius", "terra-luna", new Transaction { Type = TransactionType.Buy, Amount = 1000M, CurrencyId = 8521, Date = new DateTime(2022, 06, 04), Fee = 0M, ExchangeRate = 0.00009362441611969436M}),
				("spekulatius", "terra-luna", new Transaction { Type = TransactionType.Sell, Amount = 500M, CurrencyId = 8521, Date = new DateTime(2023, 04, 16), Fee = 0M, ExchangeRate = 0.00011602671082574758M}),
				("empfehlung", "ethereum", new Transaction { Type = TransactionType.Buy, Amount = 150M, CurrencyId = 3025, Date = new DateTime(2020, 05, 20), Fee = 0M, ExchangeRate = 0M}),
				("empfehlung", "ethereum", new Transaction { Type = TransactionType.Buy, Amount = 10M, CurrencyId = 3025, Date = new DateTime(2021, 03, 2), Fee = 1.7M, ExchangeRate = 1M}),
				("empfehlung", "ethereum", new Transaction { Type = TransactionType.Buy, Amount = 10M, CurrencyId = 3025, Date = DateTime.Now, Fee = 1.7M, ExchangeRate = 1M}),
				("empfehlung", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 150M, CurrencyId = 1122, Date = new DateTime(2023, 05, 20), Fee = 2.31M, ExchangeRate = 11M}),
				("empfehlung", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 230M, CurrencyId = 1122, Date = new DateTime(2023, 04, 30), Fee = 0M, ExchangeRate = 0M}),
				("empfehlung", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 7M, CurrencyId = 1122, Date = new DateTime(2023, 07, 19), Fee = 3M, ExchangeRate = 5M}),
				("empfehlung", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 70M, CurrencyId = 1122, Date = new DateTime(2023, 06, 22), Fee = 3M, ExchangeRate = 5M}),
				("empfehlung", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 10M, CurrencyId = 1122, Date = new DateTime(2022, 12, 2), Fee = 1M, ExchangeRate = 1M}),
				("empfehlung", "bitcoin", new Transaction { Type = TransactionType.Sell, Amount = 50M, CurrencyId = 1122, Date = new DateTime(2023, 07, 13), Fee = 2M, ExchangeRate = 5M}),
				("empfehlung", "bitcoin", new Transaction { Type = TransactionType.Sell, Amount = 40M, CurrencyId = 1122, Date = new DateTime(2023, 07, 20), Fee = 2M, ExchangeRate = 1M}),
				("empfehlung", "bitcoin", new Transaction { Type = TransactionType.Mining, Amount = 3M, CurrencyId = 1122, Date = new DateTime(2023, 01, 25), Fee = 2M, ExchangeRate = 7M}),
				("empfehlung", "bitcoin", new Transaction { Type = TransactionType.Staking, Amount = 7M, CurrencyId = 1122, Date = new DateTime(2023, 03, 25), Fee = 2M, ExchangeRate = 8M}),
				("mazars", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 20000M, CurrencyId = 1122, Date = new DateTime(2021, 01, 01), Fee = 1.8M, ExchangeRate = 0.009M}),
				("mazars", "bitcoin", new Transaction { Type = TransactionType.Sell, Amount = 12000M, CurrencyId = 1122, Date = new DateTime(2022, 04, 01), Fee = 15.12M, ExchangeRate = 0.126M}),
				("mazars", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 10000M, CurrencyId = 1122, Date = new DateTime(2022, 08, 15), Fee = 7.1M, ExchangeRate = 0.071M}),
				("mazars", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 10000M, CurrencyId = 1122, Date = new DateTime(2023, 01, 20), Fee = 7.5M, ExchangeRate = 0.075M}),
				("mazars", "bitcoin", new Transaction { Type = TransactionType.Sell, Amount = 19000M, CurrencyId = 1122, Date = new DateTime(2023, 04, 04), Fee = 16.91M, ExchangeRate = 0.089M}),
				("user3", "bitcoin", new Transaction { Type = TransactionType.Buy, Amount = 19000M, CurrencyId = 1122, Date = new DateTime(2023, 04, 04), Fee = 16.91M, ExchangeRate = 0.089M}),
			};

			foreach (var result in transactions.GroupBy(x => x.Item1)) {
				var user = await _privatePersonManager.FindByNameAsync(result.Key);
				if (_transRep.GetTransactions(user.Id).Count() != 0)
					continue;
				foreach (var (userName, coinname, status) in result) {
					await _transRep.AddTransaction(user?.Id, status);
					// TODO: Seeding with coingecko data, as well as initial coingecko run
				}
			}

			await InitializeCoins();
		}

		/// <summary>
		/// Initializes the database with the list of coins, called by StartAsync
		/// Authornames: Philipp Eckel
		/// </summary>
		private async Task InitializeCoins() {
			try {
				// Load all coins from the API
				await _currencyApi.LoadCryptoCurrenciesAsync();
				// Update the rates
				await _currencyApi.UpdateRates();
			} catch (Exception ex) {
				Console.WriteLine($"An error occurred while initializing cryptocurrencies: {ex.Message}");
			}
		}
	}
}
