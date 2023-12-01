using System.Net.Http.Headers;
using System.Text.Json;

using NACTAM.Models.Repositories;

namespace NACTAM.Models.API {

	/// <summary>
	/// Implementation of the ICurrencyApi interface for the Coingecko API.
	/// </summary>
	public class CoingeckoApi : ICurrencyApi {
		/// <summary>
		/// Returns a list of all coins from the Coingecko API.
		/// </summary>

		private static ICurrencyRepository _currencyRepository;
		/// <summary>
		/// Time tollerance before new data is fetched from the API when rate is requested
		/// </summary>
		private static readonly int _timeInterval = 24;
		/// <summary>
		/// Main currency used for the API requests
		/// </summary>
		private static readonly string _mainCurrency = "eur";
		/// <summary>
		/// Fallback currency used for the API requests, 1:1 with _mainCurrency
		/// </summary>
		private static readonly string _fallbackCurrency = "usd";

		private static HttpClient Client { get; } = new HttpClient {
			BaseAddress = new Uri("https://api.coingecko.com/api/v3/"),
			DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
		};


		/// <summary>
		/// Constructor for the CoingeckoApi class, injects the ICurrencyRepository.
		/// Autorname: Philipp Eckel
		/// </summary>
		/// <param name="currencyRepository"> ICurrencyRepository instance to be injected </param>
		public CoingeckoApi(ICurrencyRepository currencyRepository) {
			_currencyRepository = currencyRepository;
		}


		/// <inheritdoc />
		public async Task LoadCryptoCurrenciesAsync() {
			try {
				List<CryptoCurrency> currencies = await CoinsList();
				await _currencyRepository.UpdateCurrencyAsync(currencies);
			} catch (Exception e) {
				throw;
			}
		}


		/// <inheritdoc />
		public List<CryptoCurrency> GetCryptoCurrencies() {
			return _currencyRepository.GetCurrenciesAsync().Result.ToList();
		}

		/// <inheritdoc />
		public CryptoCurrency GetCryptoCurrency(int id) {
			return _currencyRepository.GetCurrencyAsync(id).Result;
		}

		/// <inheritdoc />
		public CryptoCurrency GetCryptoCurrencyByName(string name) {
			try {
				return _currencyRepository.GetCurrencyByNameAsync(name).Result;
			} catch (Exception e) {
				throw;
			}
		}


		/// <inheritdoc />
		public async Task UpdateRates() {
			List<CryptoCurrency> currencies = _currencyRepository.GetCurrenciesAsync().Result.ToList().ToList().Where(c => c.LastUpdated is not null).ToList();
			await UpdateRates(currencies.Select(c => c.Id).ToList());
		}

		/// <inheritdoc />
		public async Task UpdateRates(List<int> ids) {
			try {
				List<CryptoCurrency> currencies = _currencyRepository.GetCurrenciesAsync().Result.ToList().Where(c => ids.Contains(c.Id)).ToList();
				List<string> apiIds = currencies.Select(c => c.ApiId).ToList();
				// Get prices from API
				List<decimal> prices = await SimplePrice(apiIds);
				for (int i = 0; i < currencies.Count; i++) {
					currencies[i].Rate = prices[i];
					currencies[i].LastUpdated = DateTime.Now;
				}
				await _currencyRepository.UpdateCurrencyAsync(currencies);
			} catch (Exception e) {
				throw;
			}
		}

		/// <inheritdoc />
		public decimal GetExchangeRate(int id) {
			DateTime date = DateTime.Now;
			CryptoCurrency currency = _currencyRepository.GetCurrencyAsync(id).Result;
			try {
				//if last update is older then 24 hours
				if (currency.LastUpdated < DateTime.Now.AddHours(-_timeInterval) || currency.Rate == null) {
					decimal price = SimplePrice(new List<string>() { currency.ApiId }).Result.FirstOrDefault();
					currency.Rate = price;
					_currencyRepository.UpdateCurrencyAsync(currency);
				}
				return (decimal)currency.Rate;
			} catch (Exception e) {
				if (currency.Rate is not null) {
					return (decimal)currency.Rate;
				}
				throw;
			}
		}

		/// <inheritdoc />
		public List<decimal> GetExchangeRates(List<int> ids) {
			List<CryptoCurrency> currencies = _currencyRepository.GetCurrenciesAsync().Result.Where(c => ids.Contains(c.Id)).ToList();
			try {
				// request the rates from the API if older then 24 hours
				List<string> outdatedApiIds = currencies.Where(c => c.LastUpdated < DateTime.Now.AddHours(-_timeInterval)).Select(c => c.ApiId).ToList();
				foreach (CryptoCurrency currency in currencies) {
					if (currency.LastUpdated < DateTime.Now.AddHours(-_timeInterval) || currency.Rate == null) {
						outdatedApiIds.Add(currency.ApiId);
					}
				}
				List<decimal> rates = SimplePrice(outdatedApiIds).Result;
				foreach (CryptoCurrency currency in currencies) {
					if (outdatedApiIds.Contains(currency.ApiId)) {
						currency.Rate = rates[outdatedApiIds.IndexOf(currency.ApiId)];
						currency.LastUpdated = DateTime.Now;
					}
					_currencyRepository.UpdateCurrencyAsync(currency);
				}
				return currencies.Select(c => (decimal)c.Rate).ToList();
			} catch (Exception e) {
				if (currencies.Any(c => c.Rate is not null)) {
					return currencies.Select(c => (decimal)c.Rate).ToList();
				}
				throw;
			}
		}

		/// <inheritdoc />
		public decimal GetExchangeRate(int id, DateTime date) {
			try {
				CryptoCurrency currency = _currencyRepository.GetCurrencyAsync(id).Result;
				//if last update is older then 24 hour
				CryptoCurrency historyCurrency = CoinsHistory(currency.ApiId, date).Result;
				if (currency.LastUpdated < DateTime.Now.AddHours(-_timeInterval) || currency.Rate == null) {
					_currencyRepository.UpdateCurrencyAsync(historyCurrency);
				}

				return (decimal)historyCurrency.Rate;
			} catch (Exception e) {
				throw;
			}
		}

		/// <inheritdoc />
		public List<CryptoCurrency> SearchCryptoCurrency(string query) {
			try {
				List<string> queries = new List<string>() { query };
				List<CryptoCurrency> currencies = Search(queries).Result;
				foreach (CryptoCurrency currency in currencies) {
					_currencyRepository.UpdateCurrencyAsync(currency);
				}
				return currencies;
			} catch (Exception e) {
				throw;
			}
		}

		/// <inheritdoc />
		public List<CryptoCurrency> SearchCryptoCurrency(List<string> query) {

			try {
				List<CryptoCurrency> currencies = Search(query).Result;
				_currencyRepository.UpdateCurrencyAsync(currencies);
				return currencies;
			} catch (Exception e) {
				throw;
			}
		}

		/// <inheritdoc />
		public List<decimal> GetCoinMarketChart(int id, int days, string interval, string precision) {
			try {
				CryptoCurrency currency = _currencyRepository.GetCurrencyAsync(id).Result;
				return CoinMarketChart(currency.ApiId, days, interval, precision).Result;
			} catch (Exception e) {
				throw;
			}
		}



		/// <summary>
		/// Runs the API call and returns the result as a JsonElement
		/// </summary>
		/// <param name="request">The request to the API</param>
		/// <returns> JsonElement of the API response</returns>
		/// Author: Philipp Eckel

		private static async Task<JsonElement> RunAsync(string request) {
			HttpResponseMessage response = await Client.GetAsync(request);
			response.EnsureSuccessStatusCode();
			string responseBody = await response.Content.ReadAsStringAsync();
			JsonDocument document = JsonDocument.Parse(responseBody);
			return document.RootElement;
		}

		/// <summary>
		/// Requests the Endpoint /simple/list and returns a list of CryptoCurrencies
		/// Autorname: Philipp Eckel
		/// </summary>
		/// <returns> List of all available CryptoCurrencies</returns>
		/// Authorname: Philipp Eckel
		private async Task<List<CryptoCurrency>> CoinsList() {
			try {
				string request = $"coins/list";
				JsonElement root = await RunAsync(request);
				List<CryptoCurrency> cryptocurrencies = new List<CryptoCurrency>();
				foreach (JsonElement coin in root.EnumerateArray()) {
					string name = coin.GetProperty("name").GetString();
					string symbol = coin.GetProperty("symbol").GetString();
					string coingeckoId = coin.GetProperty("id").GetString();

					CryptoCurrency cryptocurrency = new CryptoCurrency {
						Name = name,
						ShortName = symbol,
						ApiId = coingeckoId,
					};

					cryptocurrencies.Add(cryptocurrency);
				}

				return cryptocurrencies;
			} catch (Exception e) {
				Console.WriteLine(e);
				throw;
			}
		}

		/// <summary>
		/// Requests the Endpoint /simple/history and returns a cryptoCurrency with the current price
		/// Autorname: Philipp Eckel
		/// </summary>
		/// <param name="apiId"> The apiId of the CryptoCurrency</param>
		/// <param name="date"> The date of the price</param>
		/// <returns> CryptoCurrency with the current price</returns>
		/// Authorname: Marco Lembert, modified by Philipp Eckel to return a CryptoCurrency

		private async Task<CryptoCurrency> CoinsHistory(string apiId, DateTime date) {
			try {
				//convert date to UTC as the API requires UTC and using local time would result in wrong data or no data at all if the local time is ahead of UTC
				DateTime utcDate = date.ToUniversalTime();
				string formattedDate = utcDate.ToString("dd-MM-yyyy");
				string request = $"coins/{apiId}/history?date={formattedDate}&localization=false";
				JsonElement root = await RunAsync(request);

				if (root.TryGetProperty("id", out JsonElement idElement) &&
					root.TryGetProperty("symbol", out JsonElement shortNameElement) &&
					root.TryGetProperty("name", out JsonElement nameElement) &&
					root.TryGetProperty("image", out JsonElement imageElement) &&
					root.TryGetProperty("market_data", out JsonElement marketDataElement) &&
					marketDataElement.TryGetProperty("current_price", out JsonElement currentPricesElement) &&
					currentPricesElement.TryGetProperty("eur", out JsonElement eurPriceElement)) {

					var cryptoCurrency = new CryptoCurrency {
						ApiId = idElement.GetString(),
						ShortName = shortNameElement.GetString(),
						Name = nameElement.GetString(),
						Logo = imageElement.GetProperty("thumb").GetString(),
						Rate = eurPriceElement.GetDecimal(),
						LastUpdated = date // Assuming the current time is the last update time
					};

					return cryptoCurrency;
				}
			} catch (Exception e) {
				throw;
			}

			return null; // Return null if the required properties are not found in the JSON response.
		}

		/// <summary>
		/// Requests the Endpoint /simple/price and returns a list of decimal values
		/// Use this method if you want to get the price of multiple currencies
		/// Autorname: Philipp Eckel
		/// </summary>
		/// <param name="apiIds"> The apiIds of the CryptoCurrencies</param>
		/// <returns> List of decimal values</returns>
		private async Task<List<decimal>> SimplePrice(List<string> apiIds) {
			try {
				string request =
					$"simple/price?ids={string.Join(",", apiIds)}&vs_currencies={string.Join(",", _mainCurrency, _fallbackCurrency)}&precision=full'";
				JsonElement root = await RunAsync(request);
				List<decimal> conversions = new List<decimal>();

				foreach (JsonProperty property in root.EnumerateObject()) {

					JsonElement currencyData = property.Value;

					decimal conversion;
					if (currencyData.TryGetProperty(_mainCurrency, out JsonElement vsCurrencyValue)) {
						conversion = vsCurrencyValue.GetDecimal();
					} else if (currencyData.TryGetProperty(_fallbackCurrency,
								   out JsonElement vsCurrencyValueFallback)) {
						// If the vsCurrency value does not exist, take the second value from vsCurrencies array
						conversion = vsCurrencyValueFallback.GetDecimal();
					} else {
						// If the vsCurrency value does not exist, take the second value from vsCurrencies array
						//TODO: is this the best way to handle this?
						conversion = 0m;
					}

					conversions.Add(conversion);
				}

				return conversions;
			} catch (Exception e) {
				throw;
			}

		}

		/// <summary>
		/// This endpoint requires the endpoint /search
		/// Authorname: Philipp Eckel
		/// </summary>
		/// <param name="query"> The query to search for</param>
		/// <returns> List of CryptoCurrencies</returns>
		private async Task<List<CryptoCurrency>> Search(List<string> query) {
			try {
				string request = $"search?query={string.Join(",", query)}";
				JsonElement root = await RunAsync(request);
				JsonElement coins = root.GetProperty("coins");

				List<CryptoCurrency> cryptocurrencies = new List<CryptoCurrency>();
				foreach (JsonElement coin in coins.EnumerateArray()) {
					if (coin.TryGetProperty("id", out JsonElement idElement) &&
					   coin.TryGetProperty("symbol", out JsonElement shortNameElement) &&
					   coin.TryGetProperty("name", out JsonElement nameElement) &&
					   coin.TryGetProperty("large", out JsonElement logoElement)) {

						CryptoCurrency cryptocurrency = new CryptoCurrency {
							ApiId = idElement.GetString(),
							ShortName = shortNameElement.GetString(),
							Name = nameElement.GetString(),
							Logo = logoElement.GetString()
						};

						cryptocurrencies.Add(cryptocurrency);
					}


				}

				return cryptocurrencies;
			} catch (Exception e) {
				throw;
			}

		}

		/// <summary>
		/// This endpoint requires the endpoint /coins/{id}/market_chart
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="apiId"> The apiId of the CryptoCurrency</param>
		/// <param name="days"> The number of days to get the market chart for</param>
		/// <param name="interval"> The interval of the market chart</param>
		/// <param name="precision"> The precision of the market chart</param>
		/// <returns> List of decimal values of the market chart</returns>

		private async Task<List<decimal>> CoinMarketChart(string apiId, int days, string interval, string precision) {
			try {
				string request =
					$"coins/{apiId}/market_chart?vs_currency={_mainCurrency}&days={days}&interval={interval}&precision={precision}";
				JsonElement root = await RunAsync(request);
				JsonElement pricesArray = root.GetProperty("prices");
				List<decimal> marketChart = new List<decimal>();
				foreach (JsonElement coin in pricesArray.EnumerateArray()) {
					marketChart.Add(coin[1].GetDecimal());
				}

				return marketChart;
			} catch (Exception e) {
				throw;
			}
		}
	}
}

