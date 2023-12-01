using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

using NACTAM.Exceptions;
using NACTAM.Identity.Data;
using NACTAM.Models.Repositories;

namespace NACTAM.Models {
	/// <summary>
	/// This class implements the ICurrencyRepository interface and is used to save currency in order to resolve Ids and Names, as well as cache the Rate, Logo and ApiId. It should only be accessed through the ICurrencyAPI interface.
	/// </summary>
	public class CurrencyContainer : ICurrencyRepository {
		private readonly NACTAMContext _db;

		// TODO
		/// <summary>
		/// This injects the database context into the class
		/// Authornames: Marco Lembert, Philipp Eckel
		/// </summary>
		/// <param name="db"> DB to use </param>
		public CurrencyContainer(NACTAMContext db) {
			_db = db;
		}

		/// <inheritdoc />
		public async Task<List<CryptoCurrency>> GetCurrenciesAsync() {
			try {
				return await EntityFrameworkQueryableExtensions.ToListAsync(_db.CryptoCurrency);
			} catch (Exception e) {
				throw new CurrencyNotFoundException("Currency not found", e);
			}
		}


		/// <inheritdoc />
		public async Task<CryptoCurrency> GetCurrencyAsync(int id) {
			try {
				return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<CryptoCurrency>(_db.CryptoCurrency, c => c.Id == id);
			} catch (Exception e) {
				throw new CurrencyNotFoundException("Currency not found", e);
			}
		}


		/// <inheritdoc />
		public async Task<CryptoCurrency> GetCurrencyByNameAsync(string name) {
			try {
				Task<CryptoCurrency> currency = EntityFrameworkQueryableExtensions.FirstAsync<CryptoCurrency>(_db.CryptoCurrency, c => c.Name == name);
				if (currency.Result != null) {
					return await currency;
				} else {
					return await EntityFrameworkQueryableExtensions.FirstAsync<CryptoCurrency>(_db.CryptoCurrency, c => c.Name.Contains(name));
				}
			} catch (Exception e) {
				throw new CurrencyNotFoundException("Currency not found", e);
			}
		}


		/// <inheritdoc />
		public async Task UpdateCurrencyAsync(CryptoCurrency currency) {
			try {
				// Check if the currency already exists in the database
				var existingCurrency =
					await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<CryptoCurrency>(_db.CryptoCurrency,
						c => c.Id == currency.Id);
				if (existingCurrency == null && currency.ApiId != null) {
					existingCurrency =
						await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<CryptoCurrency>(_db.CryptoCurrency,
							c => c.ApiId == currency.ApiId);
					if (existingCurrency == null) {
						// If the currency does not exist, add it to the database
						await _db.CryptoCurrency.AddAsync(currency);
						await _db.SaveChangesAsync();
						return;
					}
				}
				// If the provided currency is older than the one in the database, skip it
				if (existingCurrency.LastUpdated >= currency.LastUpdated) {
					return;
				}

				// If the currency does exist, update it
				if (currency.Rate != null) {
					existingCurrency.Rate = currency.Rate;
				}

				if (currency.Logo != null) {
					existingCurrency.Logo = currency.Logo;
				}

				if (currency.LastUpdated != null) {
					existingCurrency.LastUpdated = currency.LastUpdated;
				} else if (currency.Rate != null) {
					existingCurrency.LastUpdated = DateTime.Now;
				}
				if (currency.ApiId != null) {
					existingCurrency.ApiId = currency.ApiId;
				}
				if (currency.Name != null) {
					existingCurrency.Name = currency.Name;
				}
				if (currency.ShortName != null) {
					existingCurrency.ShortName = currency.ShortName;
				}

				await _db.SaveChangesAsync();
			} catch (Exception e) {
				throw;
			}
		}





		/// <inheritdoc />
		public async Task UpdateCurrencyAsync(List<CryptoCurrency> currencies) {
			try {
				if (_db.CryptoCurrency == null) {
					await _db.AddRangeAsync(currencies);
				}
				List<CryptoCurrency> bulkUpdate = new List<CryptoCurrency>();
				foreach (var currency in currencies) {
					// Check if the currency already exists in the database
					var existingCurrency =
						await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<CryptoCurrency>(_db.CryptoCurrency,
							c => c.Id == currency.Id);
					if (existingCurrency == null && currency.ApiId != null) {
						existingCurrency =
							await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<CryptoCurrency>(_db.CryptoCurrency,
								c => c.ApiId == currency.ApiId);
						if (existingCurrency == null) {
							// If the currency does not exist, add it to the database
							await _db.CryptoCurrency.AddAsync(currency);
							continue;
						}
					}
					// If the provided currency is older than the one in the database, skip it

					if (existingCurrency.LastUpdated >= currency.LastUpdated) {
						continue;
					}

					// If the currency does exist, update it
					if (currency.Rate != null) {
						existingCurrency.Rate = currency.Rate;
					}

					if (currency.Logo != null) {
						existingCurrency.Logo = currency.Logo;
					}

					if (currency.LastUpdated != null) {
						existingCurrency.LastUpdated = currency.LastUpdated;
					} else if (currency.Rate != null) {
						existingCurrency.LastUpdated = DateTime.Now;
					}
					if (currency.ApiId != null) {
						existingCurrency.ApiId = currency.ApiId;
					}
					if (currency.Name != null) {
						existingCurrency.Name = currency.Name;
					}
					if (currency.ShortName != null) {
						existingCurrency.ShortName = currency.ShortName;
					}

				}
				await _db.BulkUpdateAsync(bulkUpdate);
				await _db.SaveChangesAsync();
			} catch (Exception e) {
				throw;
			}
		}

	}
}