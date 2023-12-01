using System.Security.Claims;
using System.Text.Json;

using CsvHelper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NACTAM.Exceptions;
using NACTAM.Identity.Data;
using NACTAM.Models;
using NACTAM.Models.API;
using NACTAM.Models.Repositories;
using NACTAM.ViewModels;

namespace NACTAM.Controllers {

	[Authorize(Roles = "PrivatePerson")]
	public class TransactionController : Controller {

		private readonly ITransactionRepository _transactionRepository;
		private readonly ICurrencyApi _api;

		/// <summary>
		/// Constructor for the TransactionController
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="transactionRepository"></param>
		/// <param name="api"> API for the currency exchange rates and resolving ids </param>
		public TransactionController(ITransactionRepository transactionRepository, ICurrencyApi api) {
			_transactionRepository = transactionRepository;
			_api = api;
		}

		/// <summary>
		/// This method is used to get the assets overview page
		/// It summarizes all transactions and groups them by their id to amount
		/// Authornames: Philipp Eckel, Marco Lembert
		/// </summary>
		[HttpGet("/Assets")]
		public async Task<IActionResult> Assets() {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			Dictionary<int, decimal> result = new Dictionary<int, decimal>();
			foreach (Transaction transaction in _transactionRepository.GetTransactions(userId)) {
				if (result.ContainsKey(transaction.CurrencyId)) {
					if (transaction.Type != TransactionType.Sell) {
						result[transaction.CurrencyId] += transaction.Amount;
					} else {
						result[transaction.CurrencyId] -= transaction.Amount;
					}
				} else {
					if (transaction.Type == TransactionType.Sell) {
						result.Add(transaction.CurrencyId, -(transaction.Amount));
					} else {
						result.Add(transaction.CurrencyId, transaction.Amount);
					}
				}
			}

			try {
				List<AssetsViewModel> assets =
					AssetsViewModel.ConvertTransactions(_transactionRepository.GetTransactions(userId), result, _api);
				AssetsOverviewModel overview = new AssetsOverviewModel(assets);
				ViewBag.Currencies = _api.GetCryptoCurrencies().ToList().Select(c => c.Name).ToList();
				return View(overview);

			} catch (Exception e) {
				List<AssetsViewModel> assets = new List<AssetsViewModel>();
				AssetsOverviewModel overview = new AssetsOverviewModel(assets);
				try {
					ViewBag.Currencies = _api.GetCryptoCurrencies().ToList().Select(c => c.Name).ToList();

				} catch {
					ViewBag.Currencies = new List<CryptoCurrency> { };
				}
				ViewBag.Error = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es später erneut.";
				return View(overview);
			}

		}



		/// <summary>
		/// This method is used to redirect to the transactions overview page
		/// Authornames: Philipp Eckel
		/// </summary>
		public async Task<IActionResult> Index() {
			return RedirectToAction("Transactions", "Transaction");
		}


		/// <summary>
		/// This method is used to get the transactions overview page
		/// Authornames: Philipp Eckel
		/// </summary>
		[HttpGet("/Transactions")]
		public async Task<IActionResult> Transactions() {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			TransactionsOverviewViewModel transactions =
				new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api);
			return View(transactions);
		}

		/// <summary>
		/// This method is used to get the create transaction page
		/// Authornames: Philipp Eckel, Marco Lembert
		/// </summary>
		public async Task<IActionResult> CreateTransaction() {
			ViewBag.Currencies = _api.GetCryptoCurrencies().ToList();
			return View();
		}

		/// <summary>
		/// This method is used to create a transaction with a post request
		/// Authornames: Philipp Eckel, Marco Lembert
		/// </summary>
		/// <param name="transactionViewModel"> The transaction view model submitted by the user </param>
		[HttpPost("CreateTransaction")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateTransactionPost(TransactionsViewModel transactionViewModel) {
			ViewBag.Currencies = _api.GetCryptoCurrencies().ToList();
			if (!ModelState.IsValid) {
				return View("CreateTransaction", transactionViewModel);
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			try {
				Transaction transaction = transactionViewModel.ToTransaction(userId, _api);
				// Update the exchange rate and make sure a rate is in the database
				_api.GetExchangeRate(transaction.CurrencyId);
				await _transactionRepository.AddTransaction(userId, transaction);
				return Redirect("/Transaction");

			} catch (Exception e) when (e is HttpRequestException || e is JsonException ||
										e is AggregateException | e is NullReferenceException) {
				ViewData["RateLimitError"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es später erneut.";
				return View("CreateTransaction", transactionViewModel);
			} catch (TransactionBalanceException e) {
				// validation went wrong
				decimal balance =
					_transactionRepository.GetTransactions(userId).Where(c =>
						c.CurrencyId == _api.GetCryptoCurrencyByName(transactionViewModel.CryptoCurrency).Id &&
						c.Type != TransactionType.Sell).Sum(c => c.Amount) - _transactionRepository
						.GetTransactions(userId).Where(c =>
							c.CurrencyId == _api.GetCryptoCurrencyByName(transactionViewModel.CryptoCurrency).Id &&
							c.Type == TransactionType.Sell).Sum(c => c.Amount);
				ViewData["BalanceError"] =
					$"Sie können {transactionViewModel.Amount} von {transactionViewModel.CryptoCurrency} nicht verkaufen, da sie nur {balance} besitzen.";
				return View("CreateTransaction", transactionViewModel);
			} catch (IllegalDateException e) {
				ViewData["DateError"] = "Das Datum darf nicht in der Zukunft liegen.";
				return View("CreateTransaction", transactionViewModel);
			}

		}

		/// <summary>
		/// This method is used to edit a transaction
		/// Authornames: Marco Lembert, Philipp Eckel
		/// </summary>
		/// <param name="id"> The id of the transaction to edit</param>

		[HttpGet("EditTransaction")]
		public async Task<IActionResult> EditTransaction(int id) {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			try {
				Transaction transaction = _transactionRepository.GetTransaction(id, userId);
				if (transaction.Deleted == true) {
					ViewData["Error"] = "Die Transaktion die Sie bearbeiten wollen wurde gelöscht.";
					TransactionsOverviewViewModel tvm =
						new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api);
					return View("Transactions", tvm);
				}
				TransactionsViewModel transactionsViewModel =
					new TransactionsViewModel(transaction, _api.GetCryptoCurrency(transaction.CurrencyId).Name);
				ViewBag.Currencies = _api.GetCryptoCurrencies().ToList();
				return View("EditTransaction", transactionsViewModel);
			} catch (IllegalUserOperationException e) {
				return View("AccesDenied");
			} catch (TransactionNotFoundException e) {
				return View("404");
			} catch (NullReferenceException e) {
				return View("404");
			}
			return Redirect("/Transactions");
		}


		/// <summary>
		/// This method is used to edit a transaction with a post request
		/// Authornames: Marco Lembert, Philipp Eckel
		/// </summary>
		/// <param name="transactionViewModel"> The transaction view model submitted by the user </param>
		[HttpPost("EditTransaction")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditTransactionPost(TransactionsViewModel transactionViewModel) {
			ViewBag.Currencies = _api.GetCryptoCurrencies().ToList();
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!ModelState.IsValid) {
				return View("EditTransaction", transactionViewModel);
			}
			try {
				Transaction transaction = transactionViewModel.ToTransaction(userId, _api);
				// Update the exchange rate and make sure a rate is in the database
				_api.GetExchangeRate(transaction.CurrencyId);
				await _transactionRepository.UpdateTransaction(userId, transaction);
				return Redirect("/Transactions");
			} catch (TransactionBalanceException e) {
				// validation went wrong
				decimal balance =
					_transactionRepository.GetTransactions(userId).Where(c =>
						c.CurrencyId == _api.GetCryptoCurrencyByName(transactionViewModel.CryptoCurrency).Id &&
						c.Type != TransactionType.Sell).Sum(c => c.Amount) - _transactionRepository
						.GetTransactions(userId).Where(c =>
							c.CurrencyId == _api.GetCryptoCurrencyByName(transactionViewModel.CryptoCurrency).Id &&
							c.Type == TransactionType.Sell).Sum(c => c.Amount);
				ViewData["BalanceError"] =
					$"Sie können {transactionViewModel.Amount} von {transactionViewModel.CryptoCurrency} nicht verkaufen, da sie nur {balance} besitzen.";
				return View("EditTransaction", transactionViewModel);
			} catch (IllegalDateException) {
				ViewData["DateError"] = "Das Datum darf nicht in der Zukunft liegen.";
				return View("EditTransaction", transactionViewModel);
			} catch (Exception e) when (e is HttpRequestException || e is JsonException ||
										e is AggregateException | e is NullReferenceException) {
				ViewData["RateLimitError"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es später erneut.";
				return View("EditTransaction", transactionViewModel);

			}
		}

		/// <summary>
		/// This method is used to remove a transaction
		/// Authornames: Marco Lembert, Philipp Eckel
		/// </summary>
		/// <param name="id"> The id of the transaction to remove</param>
		[HttpPost("RemoveTransaction")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveTransaction(int id) {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			try {
				await _transactionRepository.RemoveTransaction(userId, id);
			} catch (TransactionBalanceException e) {
				ViewData["Error"] =
					"Sie können diese Transaktion nicht löschen, da sie sonst ein negatives Guthaben hätten.";
				TransactionsOverviewViewModel tom =
					new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api);
				return View("Transactions", tom);
			} catch (IllegalUserOperationException e) {
				ViewData["Error"] =
					"Sie haben dazu keine Berechtigung.";
				TransactionsOverviewViewModel tom =
					new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api);
				return View("Transactions", tom);
			} catch (TransactionNotFoundException e) {
				return NotFound();
			}
			return Redirect("/Transactions");
		}



		/// <summary>
		/// This method is used to export transactions to a csv file
		/// Authornames: Philipp Eckel
		/// </summary>
		[HttpGet("ExportTransactions")]
		public IActionResult ExportTransactions() {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			List<TransactionCsv> transactions = _transactionRepository.GetTransactions(userId).Select(t => new TransactionCsv(t, _api)).ToList();
			string csvFileName = "transactions_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			HttpContext.Response.ContentType = "text/csv";
			HttpContext.Response.Headers.Add("content-disposition", $"attachment; filename={csvFileName}");
			using (var memoryStream = new MemoryStream()) {
				// Write data to the MemoryStream using CSVHelper
				TransactionCsv.WriteTransactionsToCsv(transactions, memoryStream);
				return File(memoryStream.ToArray(), HttpContext.Response.ContentType, csvFileName);
			}
		}

		/// <summary>
		/// This method is used to import transactions from a csv file
		/// Authornames: Philipp Eckel
		/// </summary>
		/// <param name="csvFile"> The csv file to import</param>
		[HttpPost("ImportTransactions")]
		public IActionResult ImportTransactions(IFormFile csvFile) {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			try {
				List<TransactionCsv> importedTransactions = TransactionCsv.ReadTransactionsFromCsv(csvFile);
				importedTransactions = importedTransactions.OrderBy(t => t.Date).ToList();
				List<int> currencies = _api.GetCryptoCurrencies().Select(c => c.Id).ToList();
				int success = 0;
				foreach (var transaction in importedTransactions) {
					try {
						_transactionRepository.AddTransaction(userId, transaction.ToTransaction(userId, _api));
						success++;
					} catch (Exception e) when (e is TransactionBalanceException || e is IllegalDateException || e is HttpRequestException || e is JsonException || e is AggregateException || e is System.NullReferenceException) {
						Console.WriteLine(e);
					}
				}
				if (success == importedTransactions.Count) {
					return Redirect("/Transactions");
				} else {
					ViewData["Error"] =
						$"Von {importedTransactions.Count.ToString()} Transaktionen konnten nur {success} erfolgreich importiert werden.";
					return View("Transactions",
						new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api));
				}
			} catch (HeaderValidationException e) {
				TransactionsOverviewViewModel tom =
					new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api);
				ViewData["Error"] = "Es ist ein Fehler aufgetreten. Bitte überprüfen Sie die Daten in der Datei.";
				return View("Transactions", tom);
			} catch (Exception e) when (e is HttpRequestException || e is JsonException ||
										e is AggregateException | e is NullReferenceException) {
				TransactionsOverviewViewModel tom =
					new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api);
				ViewData["Error"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es später erneut.";
				return View("Transactions", tom);
			}
		}

		/// <summary>
		/// This method is used to update the rates of all currencies
		/// Authornames: Philipp Eckel
		/// </summary>
		[HttpPost("UpdateCurrencies")]
		public async Task<IActionResult> UpdateCurrencies() {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			//get list of currency which user has
			List<CryptoCurrency> currencies = _transactionRepository.GetTransactions(userId).Select(t => _api.GetCryptoCurrency(t.CurrencyId)).ToList();
			List<int> ids = currencies.Select(c => c.Id).ToList();
			try {
				await _api.UpdateRates(ids);
			} catch (Exception e) when (e is HttpRequestException || e is JsonException ||
										e is AggregateException | e is NullReferenceException) {
				Console.WriteLine(e);
				// nothing to do if it fails
			}

			return Redirect(Request.Headers["Referer"].ToString());
		}

		/// <summary>
		/// This method is used to get the rate of a currency at a specific date for ajax calls
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="currency"> name of the currency</param>
		/// <param name="date"> date of the rate</param>
		[HttpGet]
		public IActionResult CurrencyRate(string currency, DateTime date) {
			if (string.IsNullOrEmpty(currency)) {
				return Json(0m);
			}
			int cryptoId = _api.GetCryptoCurrencyByName(currency).Id;
			decimal rate;
			try {
				if (date == DateTime.Parse("01.01.0001 00:00:00")) {
					// date is null, get rate from now
					rate = _api.GetExchangeRate(cryptoId);
				} else {
					rate = _api.GetExchangeRate(cryptoId, date);
				}

				return Json(rate);
			} catch (Exception e) when (e is HttpRequestException || e is JsonException || e is AggregateException || e is System.NullReferenceException) {
				Console.WriteLine(e);
				return Json("Error");
			}
		}

		/// <summary>
		/// This method is used to get the chart data for a currency for ajax calls
		/// Authornames: Marco Lembert
		/// </summary>
		/// <param name="currency"> name of the currency</param>
		[HttpGet]
		public async Task<IActionResult> CurrencyChart(string currency) {
			CryptoCurrency crypto = _api.GetCryptoCurrencyByName(currency);
			try {
				List<decimal> rates = _api.GetCoinMarketChart(crypto.Id, 11, "daily", "");
				return Json(rates);
			} catch (Exception e) {
				return StatusCode(500, new { error = "Konnte die API nicht erreichen" });
			}
		}
	}
}
