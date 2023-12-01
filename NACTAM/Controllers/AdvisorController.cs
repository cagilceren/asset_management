using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NACTAM.Identity.Data;
using NACTAM.Models;
using NACTAM.Models.API;
using NACTAM.Models.Repositories;
using NACTAM.Models.TaxRecommendation;
using NACTAM.ViewModels;
using NACTAM.ViewModels.TaxEvaluation;

namespace NACTAM.Controllers;

/// <summary>
/// Controller for everything Admin related
/// </summary>

[Authorize(Roles = "TaxAdvisor")]
public class AdvisorController : Controller {
	private readonly ILogger<AdvisorController> _logger;

	private readonly ITaxAdvisorRepository _taxAdvisorRepository;
	private readonly UserManager<User> _userManager;
	private readonly UserManager<PrivatePerson> _privatePersonManager;
	private readonly UserManager<TaxAdvisor> _taxAdvisorManager;
	private readonly INotificationRepository _notificationRepository;
	private readonly IUserRepository _userRepository;
	private readonly ITransactionRepository _transactionRepository;
	private readonly ICurrencyApi _api;
	private readonly ITaxCalculations _taxCalculations;

	/// <summary>
	/// <paramref name="db">Context of NACTAM</paramref>
	/// <paramref name="privatePersonManager">UserManager</paramref>
	/// <paramref name="userManager">UserManager</paramref>
	/// <paramref name="privatePersonManager">UserManager for private persons</paramref>
	/// <paramref name="taxAdvisorManager">UserManager for tax advisors</paramref>
	/// <paramref name="notificationRepository">interface for <c>NotificationContainer</c></paramref>
	/// <paramref name="taxAdvisorRepository">interface for <c>TaxAdvisorContainer</c></paramref>
	/// <paramref name="taxCalculations">Tax Calculation Service</paramref>
	/// <paramref name="userRepository">interface for <c>UserContainer</c></paramref>
	/// <paramref name="transactionRepository">interface for <c>Transaction Container</c></paramref>
	/// <paramref name="api">ICurrency API</paramref>
	/// <paramref name="logger">Logger</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>

	public AdvisorController(NACTAMContext db, UserManager<User> userManager, UserManager<PrivatePerson> privatePersonManager, UserManager<TaxAdvisor> taxAdvisorManager, INotificationRepository notificationRepository, ITaxAdvisorRepository taxAdvisorRepository, ITaxCalculations taxCalculations, IUserRepository userRepository, ITransactionRepository transactionRepository, ICurrencyApi api, ILogger<AdvisorController> logger) {
		_logger = logger;
		_userManager = userManager;
		_privatePersonManager = privatePersonManager;
		_taxAdvisorManager = taxAdvisorManager;
		_notificationRepository = notificationRepository;
		_taxAdvisorRepository = taxAdvisorRepository;
		_userRepository = userRepository;
		_transactionRepository = transactionRepository;
		_api = api;
		_taxCalculations = taxCalculations;
	}

	/// <summary>
	/// MyUsers page:
	///
	/// creates MyUsers view.
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>View: Overview over all assigned user</returns>

	[HttpGet("/Advisor/MyUsers")]
	public async Task<ViewResult> MyUsers() {
		// error Handling:
		if (TempData.ContainsKey("Error")) {
			string errorMessage = TempData["Error"] as string;
			TempData.Remove("Error");
			ViewBag.Error = errorMessage;
		}

		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		return View(new MyUsersViewModel(myUser));
	}

	/// <summary>
	/// AllUsers page:
	///
	/// creates AllUsers view.
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>View: Overview over all unassigned user</returns>

	[HttpGet("/Advisor/AllUsers")]
	public async Task<IActionResult> AllUsers() {
		// error Handling:
		if (TempData.ContainsKey("Error")) {
			string errorMessage = TempData["Error"] as string;
			TempData.Remove("Error");
			ViewBag.Error = errorMessage;
		}

		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		return View(new AllUsersViewModel(
					_taxAdvisorRepository.GetMyCustomers(myUser).OrderBy(x => x.UserName),
					_taxAdvisorRepository.GetAllCustomers(myUser).OrderBy(x => x.UserName),
					myUser));
	}

	/// <summary>
	/// Function to assign a user
	///
	/// <paramref name="userName">username of the user to be assigned</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>Redirect to AllUsers</returns>

	[HttpPost]
	public async Task<ActionResult> AssignUser(string userName) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);

		// Check if username is valid
		if (await _taxAdvisorRepository.CheckUserName(userName)) {
			// check if user is not assigned already
			if (await _taxAdvisorRepository.CheckIsUnassigned(myUser.UserName, userName)) {
				await _taxAdvisorRepository.AssignUser(myUser, userName);
				return RedirectToAction("AllUsers");
			} else {
				TempData["Error"] = "Dieser Account ist Ihnen bereits zugeteilt!";
				return RedirectToAction("AllUsers");
			}
		} else {
			TempData["Error"] = "Dieser Account existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("AllUsers");
		}
	}

	/// <summary>
	/// Function to revoke a user
	///
	/// <paramref name="userName">username of the user to be revoked</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>Redirect to AllUsers</returns>

	[HttpPost]
	public async Task<ActionResult> RevokeUser(string userName) {

		// Check if username is valid
		if (await _taxAdvisorRepository.CheckUserName(userName)) {
			var myUser = await _taxAdvisorManager.GetUserAsync(User);
			// check if user is unassigned already
			if (await _taxAdvisorRepository.CheckIsAssigned(myUser.UserName, userName)) {
				await _taxAdvisorRepository.RevokeUser(myUser, userName);
				return RedirectToAction("AllUsers");
			} else {
				TempData["Error"] = "Dieser Account ist Ihnen nicht zugeteilt!";
				return RedirectToAction("AllUsers");
			}
		} else {
			TempData["Error"] = "Dieser Account existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("AllUsers");
		}

	}

	/// <summary>
	/// Function to request a simple insight for an user
	///
	/// <paramref name="userName">username of the user to be requested</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>Redirect to MyUsers (AllUsers if not assigned to user)</returns>

	[HttpPost]
	public async Task<ActionResult> RequestSimpleInsight(string userName) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		// Check if username is valid
		if (await _taxAdvisorRepository.CheckUserName(userName)) {
			// check if user is assigned
			if (await _taxAdvisorRepository.CheckInsightStatus(myUser.UserName, userName, InsightStatus.Assigned)) {
				await _taxAdvisorRepository.ChangeAllowanceStatus(myUser, userName, InsightStatus.SimpleUnaccepted);
				return RedirectToAction("MyUsers");
			} else {
				TempData["Error"] = "Sie müssen sich diese Person erst zuteilen, bevor Sie sich eine Einsicht anfordern können!";
				return RedirectToAction("AllUsers");
			}
		} else {
			TempData["Error"] = "Dieser User existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("MyUsers");
		}
	}

	/// <summary>
	/// Function to undo a simple insight request
	///
	/// <paramref name="userName">username of the user</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>Redirect to MyUsers</returns>

	[HttpPost]
	public async Task<ActionResult> UndoRequestSimpleInsight(string userName) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		// Check if username is valid
		if (await _taxAdvisorRepository.CheckUserName(userName)) {
			// check if user is simple unassigned
			if (await _taxAdvisorRepository.CheckInsightStatus(myUser.UserName, userName, InsightStatus.SimpleUnaccepted)) {
				var toUser = await _privatePersonManager.FindByNameAsync(userName);
				await _notificationRepository.UndoInsightRequest(myUser, toUser, false);
				await _taxAdvisorRepository.ChangeAllowanceStatus(myUser, userName, InsightStatus.Assigned);
				return RedirectToAction("MyUsers");
			} else {
				TempData["Error"] = "Sie müssen sich diese Person erst zuteilen und eine Einsicht anfordern um diese Aktion auszuführen!";
				return RedirectToAction("MyUsers");
			}
		} else {
			TempData["Error"] = "Dieser User existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("MyUsers");
		}
	}

	/// <summary>
	/// Function to request a extended insight
	///
	/// <paramref name="userName">username of the user</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>Redirect to MyUsers</returns>

	[HttpPost]
	public async Task<ActionResult> RequestExtended(string userName) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		// Check if username is valid
		if (await _taxAdvisorRepository.CheckUserName(userName)) {
			// check if user has simple insight
			if (await _taxAdvisorRepository.CheckInsightStatus(myUser.UserName, userName, InsightStatus.Simple)) {
				await _taxAdvisorRepository.ChangeAllowanceStatus(myUser, userName, InsightStatus.ExtendedUnaccepted);
				return RedirectToAction("MyUsers");
			} else {
				TempData["Error"] = "Sie müssen sich diese Person erst zuteilen und eine einfache Einsicht erhalten, bevor Sie diese Aktion ausführen können!";
				return RedirectToAction("MyUsers");
			}
		} else {
			TempData["Error"] = "Dieser User existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("MyUsers");
		}



	}

	/// <summary>
	/// Function to undo a extended insight request
	///
	/// <paramref name="userName">username of the user</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>Redirect to MyUsers</returns>

	[HttpPost]
	public async Task<ActionResult> UndoRequestExtended(string userName) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		// Check if username is valid
		if (await _taxAdvisorRepository.CheckUserName(userName)) {
			// check if user has extended unaccepted insight
			if (await _taxAdvisorRepository.CheckInsightStatus(myUser.UserName, userName, InsightStatus.ExtendedUnaccepted)) {
				var toUser = await _privatePersonManager.FindByNameAsync(userName);
				await _notificationRepository.UndoInsightRequest(myUser, toUser, true);
				await _taxAdvisorRepository.ChangeAllowanceStatus(myUser, userName, InsightStatus.Simple);
				return RedirectToAction("MyUsers");
			} else {
				TempData["Error"] = "Sie müssen sich diese Person erst zuteilen und eine erwiterte Einsicht anfordern um diese Aktion auszuführen!";
				return RedirectToAction("MyUsers");
			}
		} else {
			TempData["Error"] = "Dieser User existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("MyUsers");
		}
	}

	/// <summary>
	/// Function to get a partial view containing all other taxadvisors
	///
	/// <paramref name="userName">username of the user</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>PartialView: Containing all other taxadvisors (Error: AllUsers)</returns>

	[HttpGet]
	public async Task<ActionResult> OtherTaxAdvisors(string userName) {
		// Check if username is valid
		if (await _taxAdvisorRepository.CheckUserName(userName)) {
			var selectedUser = await _privatePersonManager.FindByNameAsync(userName);
			var myUser = await _taxAdvisorManager.GetUserAsync(User);
			await _userRepository.LoadAdvisors(selectedUser);
			var assignedAdvisors = selectedUser.Advisors;
			assignedAdvisors.Remove(myUser);
			var allOtherAdvisors = _userRepository.GetAllTaxAdvisors().Except(assignedAdvisors).ToList();
			allOtherAdvisors.Remove(myUser);
			return PartialView("OtherAdvisorsPopUpView", new OtherTaxAdvisorsViewModel(assignedAdvisors, allOtherAdvisors, userName));
		} else {
			TempData["Error"] = "Dieser Account existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("AllUsers");
		}

	}

	/// <summary>
	/// Function to remove other advisor from user
	///
	/// <paramref name="userName">username of the user</paramref>
	/// <paramref name="advisorUserName">username of the advisor</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>PartialView: Containing all remaining taxadvisors (Error: AllUsers)</returns>

	[HttpGet]
	public async Task<ActionResult> RemoveOtherAdvisor(String userName, string advisorUserName) {
		// Check if usernames are valid
		if (await _taxAdvisorRepository.CheckUserName(userName) && await _taxAdvisorRepository.CheckUserName(advisorUserName)) {
			// check if user is assigned
			if (await _taxAdvisorRepository.CheckIsAssigned(advisorUserName, userName)) {
				var taxAdvisor = await _taxAdvisorManager.FindByNameAsync(advisorUserName);
				await _taxAdvisorRepository.RevokeUser(taxAdvisor, userName);

				var selectedUser = await _privatePersonManager.FindByNameAsync(userName);
				var myUser = await _taxAdvisorManager.GetUserAsync(User);
				await _userRepository.LoadAdvisors(selectedUser);
				var assignedAdvisors = selectedUser.Advisors;
				assignedAdvisors.Remove(myUser);
				var allOtherAdvisors = _userRepository.GetAllTaxAdvisors().Except(assignedAdvisors).ToList();
				allOtherAdvisors.Remove(myUser);
				return PartialView("OtherAdvisorsPopUpView", new OtherTaxAdvisorsViewModel(assignedAdvisors, allOtherAdvisors, userName));
			} else {
				TempData["Error"] = "Diese Aktion ist nicht möglich, da der Account diesem Steuerberater nicht zugewiesen ist!";
				return RedirectToAction("AllUsers");
			}
		} else {
			TempData["Error"] = "Dieser Account existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("AllUsers");
		}
	}

	/// <summary>
	/// Function to add other advisor to user
	///
	/// <paramref name="userName">username of the user</paramref>
	/// <paramref name="advisorUserName">username of the advisor</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>PartialView: Containing all taxadvisors and the new one(Error: AllUsers)</returns>

	[HttpGet]
	public async Task<ActionResult> AddOtherAdvisor(String userName, string advisorUserName) {
		// Check if usernames are valid
		if (await _taxAdvisorRepository.CheckUserName(userName) && await _taxAdvisorRepository.CheckUserName(advisorUserName)) {
			// Check if user is unassigned
			if (await _taxAdvisorRepository.CheckIsUnassigned(advisorUserName, userName)) {
				var taxAdvisor = await _taxAdvisorManager.FindByNameAsync(advisorUserName);
				await _taxAdvisorRepository.AssignUser(taxAdvisor, userName);

				var selectedUser = await _privatePersonManager.FindByNameAsync(userName);
				var myUser = await _taxAdvisorManager.GetUserAsync(User);
				await _userRepository.LoadAdvisors(selectedUser);
				var assignedAdvisors = selectedUser.Advisors;
				assignedAdvisors.Remove(myUser);
				var allOtherAdvisors = _userRepository.GetAllTaxAdvisors().Except(assignedAdvisors).ToList();
				allOtherAdvisors.Remove(myUser);
				return PartialView("OtherAdvisorsPopUpView", new OtherTaxAdvisorsViewModel(assignedAdvisors, allOtherAdvisors, userName));
			} else {
				TempData["Error"] = "Diese Aktion ist nicht möglich, da der Account diesem Steuerberater bereits zugewiesen ist!";
				return RedirectToAction("AllUsers");
			}
		} else {
			TempData["Error"] = "Dieser User existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("AllUsers");
		}
	}

	/// <summary>
	/// Extended Insight View of Assets from user
	///
	/// <paramref name="userId">id of the user</paramref>
	///
	/// author: Niklas Thuer (-> code to get assets copyed and modified from assets page)
	/// </summary>
	/// <returns>View: Containing all Assets of the user (Error: No Permission View)</returns>

	public async Task<IActionResult> ExtendedInsightAssets(string userId) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		// Check for extended insight status
		if (_taxAdvisorRepository.CheckExtendedInsightStatus(myUser, userId)) {
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
			List<AssetsViewModel> assets =
				AssetsViewModel.ConvertTransactions(_transactionRepository.GetTransactions(userId), result, _api);
			AssetsOverviewModel overview = new AssetsOverviewModel(assets);
			ViewBag.Currencies = _api.GetCryptoCurrencies().ToList().Select(c => c.Name).ToList();
			return View(overview);
		} else {
			return View("NoPermission");
		}
	}

	/// <summary>
	/// Extended Insight View of Transactions from user
	///
	/// <paramref name="userId">id of the user</paramref>
	///
	/// author: Niklas Thuer (-> code to get assets copyed and modified from transactions page)
	/// </summary>
	/// <returns>View: Containing all Transactions of the user (Error: No Permission View)</returns>

	public async Task<IActionResult> ExtendedInsightTransactions(string userId) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		// Check for extended insight status
		if (_taxAdvisorRepository.CheckExtendedInsightStatus(myUser, userId)) {
			TransactionsOverviewViewModel transactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api);
			return View(transactions);
		} else {
			return View("NoPermission");
		}
	}

	/// <summary>
	/// Function to get the profilepicture of an user
	///
	/// <paramref name="userName">username of the user</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>FileContentResult: Representing the profile picture</returns>
	public async Task<FileContentResult> ProfilePicture(string userName) {
		var requestedUser = await _userManager.FindByNameAsync(userName);
		return new FileContentResult(requestedUser?.Icon ?? new byte[] { }, "image/jpeg");
	}

	/// <summary>
	/// Function to get partial view containing all data of an simple insight
	///
	/// <paramref name="userName">username of the user</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>PartialView: Containg all data of an simple insight (Error: MyView)</returns>
	[HttpGet]
	public async Task<ActionResult> SimpleInsight(string userName) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		//check username
		if (await _taxAdvisorRepository.CheckUserName(userName)) {
			// check status
			if (await _taxAdvisorRepository.CheckImplyStatus(myUser.UserName, userName, InsightStatus.Simple)) {
				var user = await _privatePersonManager.FindByNameAsync(userName);
				var userId = user.Id;

				var allTransactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api).Transactions;
				var allSellTransactions = _taxCalculations.CalculateAfterSaleTaxDetails(allTransactions, DateTime.Now).AfterSaleSellTransactions;

				var yearList = TaxEvaluationViewModel.ListedYears(allSellTransactions);

				var yearAsInts = new List<int>();
				var profitsPerYear = new List<decimal>();
				var miningStakingPerYear = new List<decimal>();


				foreach (var item in yearList) {
					var year = item.SellDate.Year;
					yearAsInts.Add(year);

					decimal newProfit = 0;
					var yearTransactions = TaxEvaluationViewModel.Filter(allSellTransactions, year);
					TaxEvaluationViewModel.CalculateProfitFromSales(yearTransactions, ref newProfit);
					profitsPerYear.Add(newProfit);

					var miningStakingTupel = _taxCalculations.CalculateProfitFromMiningAndStaking(allTransactions, year);
					miningStakingPerYear.Add(miningStakingTupel.Item1 + miningStakingTupel.Item2);
				}

				return PartialView("SimpleInsight", new SimpleInsightViewModel(yearAsInts, profitsPerYear, miningStakingPerYear));
			} else {
				TempData["Error"] = "Sie haben keine Einsichtserlaubnis für diesen Account! Fragen Sie eine Einsicht an und versuchen Sie es anschließend erneut.";
				return RedirectToAction("MyUsers");
			}
		} else {
			TempData["Error"] = "Dieser Account existiert nicht! Bitte versuchen Sie es erneut!";
			return RedirectToAction("MyUsers");
		}
	}

	/// <summary>
	/// Function to get all data of an simple insight as json to create a pdf
	///
	/// <paramref name="userName">username of the user</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>JSON: Containg all data of an simple insigh</returns>
	[HttpGet]
	public async Task<ActionResult> GetSimpleInsightData(string userName) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		if (await _taxAdvisorRepository.CheckUserName(userName) && await _taxAdvisorRepository.CheckImplyStatus(myUser.UserName, userName, InsightStatus.Simple)) {

			var user = await _privatePersonManager.FindByNameAsync(userName);

			// Personal Data:
			var dataList = new List<string>();

			dataList.Add(user.FirstName);
			dataList.Add(user.LastName);
			dataList.Add(user.Email);
			dataList.Add(user.PhoneNumber);
			dataList.Add(user.StreetName ?? "k. A.");
			dataList.Add(user.HouseNumber ?? "k. A.");
			dataList.Add(user.ZIP ?? "k. A.");
			dataList.Add(user.City ?? "k. A.");

			// Simple Insight Data:
			var userId = user.Id;

			var allTransactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api).Transactions;
			var allSellTransactions = _taxCalculations.CalculateAfterSaleTaxDetails(allTransactions, DateTime.Now).AfterSaleSellTransactions;

			var yearList = TaxEvaluationViewModel.ListedYears(allSellTransactions);

			var yearAsInts = new List<string>();
			var profitsPerYear = new List<string>();
			var miningStakingPerYear = new List<string>();


			foreach (var item in yearList) {
				var year = item.SellDate.Year;
				yearAsInts.Add(year.ToString());

				decimal newProfit = 0;
				var yearTransactions = TaxEvaluationViewModel.Filter(allSellTransactions, year);
				TaxEvaluationViewModel.CalculateProfitFromSales(yearTransactions, ref newProfit);
				profitsPerYear.Add(newProfit.ToString());

				var miningStakingTupel = _taxCalculations.CalculateProfitFromMiningAndStaking(allTransactions, year);
				miningStakingPerYear.Add((miningStakingTupel.Item1 + miningStakingTupel.Item2).ToString());
			}



			var daten = new { list1 = dataList, list2 = yearAsInts, list3 = profitsPerYear, list4 = miningStakingPerYear, error = "false" };

			// Die Daten in JSON umwandeln und zurückgeben
			return Json(daten);
		} else {
			var daten = new {
				error = "true"
			};
			// Die Daten in JSON umwandeln und zurückgeben
			return Json(daten);
		}
	}

	/// <summary>
	/// Function to get all data of an extened insight as json to create a pdf
	///
	/// <paramref name="userName">username of the user</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	/// <returns>JSON: Containg all data of an extended insight</returns>

	[HttpGet]
	public async Task<ActionResult> GetExtendedInsightData(string userName) {
		var myUser = await _taxAdvisorManager.GetUserAsync(User);
		if (await _taxAdvisorRepository.CheckUserName(userName) && await _taxAdvisorRepository.CheckInsightStatus(myUser.UserName, userName, InsightStatus.Extended)) {
			var user = await _privatePersonManager.FindByNameAsync(userName);

			// Personal Data:
			var dataList = new List<string>();

			dataList.Add(user.FirstName);
			dataList.Add(user.LastName);
			dataList.Add(user.Email);
			dataList.Add(user.PhoneNumber);
			dataList.Add(user.StreetName ?? "k. A.");
			dataList.Add(user.HouseNumber ?? "k. A.");
			dataList.Add(user.ZIP ?? "k. A.");
			dataList.Add(user.City ?? "k. A.");

			// Simple Insight Data:
			var userId = user.Id;

			var allTransactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api).Transactions;
			var allSellTransactions = _taxCalculations.CalculateAfterSaleTaxDetails(allTransactions, DateTime.Now).AfterSaleSellTransactions;

			var yearList = TaxEvaluationViewModel.ListedYears(allSellTransactions);

			var yearAsInts = new List<string>();
			var profitsPerYear = new List<string>();
			var miningStakingPerYear = new List<string>();


			foreach (var item in yearList) {
				var year = item.SellDate.Year;
				yearAsInts.Add(year.ToString());

				decimal newProfit = 0;
				var yearTransactions = TaxEvaluationViewModel.Filter(allSellTransactions, year);
				TaxEvaluationViewModel.CalculateProfitFromSales(yearTransactions, ref newProfit);
				profitsPerYear.Add(newProfit.ToString());

				var miningStakingTupel = _taxCalculations.CalculateProfitFromMiningAndStaking(allTransactions, year);
				miningStakingPerYear.Add((miningStakingTupel.Item1 + miningStakingTupel.Item2).ToString());
			}

			// Assets Data:
			Dictionary<int, decimal> result = new Dictionary<int, decimal>();

			foreach (Transaction transaction in _transactionRepository.GetTransactions(userId)) {
				if (result.ContainsKey(transaction.CurrencyId)) {
					if (transaction.Type != TransactionType.Sell) {
						result[transaction.CurrencyId] += (transaction.ExchangeRate * transaction.Amount);
					} else {
						result[transaction.CurrencyId] -= (transaction.ExchangeRate * transaction.Amount);
					}
				} else {
					if (transaction.Type == TransactionType.Sell) {
						result.Add(transaction.CurrencyId, -(transaction.ExchangeRate * transaction.Amount));
					} else {
						result.Add(transaction.CurrencyId, transaction.ExchangeRate * transaction.Amount);
					}
				}
			}

			List<AssetsViewModel> assets = AssetsViewModel.ConvertTransactions(_transactionRepository.GetTransactions(userId), result, _api);

			var assetCurrencyList = new List<string>();
			var assetValueList = new List<string>();
			var assetRateList = new List<string>();

			foreach (var item in assets) {
				assetCurrencyList.Add(item.Currency.ToString());
				assetValueList.Add(item.Value.ToString());
				assetRateList.Add(item.Rate.ToString());
			}

			// transaktions:
			TransactionsOverviewViewModel transactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api);

			var transactionCurrencyList = new List<string>();
			var transactionTypeList = new List<string>();
			var transactionDateList = new List<string>();
			var transactionAmountList = new List<string>();
			var transactionRateList = new List<string>();
			var transactionFeeList = new List<string>();

			foreach (var item in transactions.Transactions) {
				transactionCurrencyList.Add(item.CryptoCurrency.ToString());
				transactionTypeList.Add(item.Type.ToString());
				transactionDateList.Add(item.Date.ToString());
				transactionAmountList.Add(item.Amount.ToString());
				transactionRateList.Add(item.Rate.ToString());
				transactionFeeList.Add(item.Fee.ToString());
			}

			var daten = new {
				list1 = dataList,
				list2 = yearAsInts,
				list3 = profitsPerYear,
				list4 = miningStakingPerYear,
				list5 = assetCurrencyList,
				list6 = assetValueList,
				list7 = assetRateList,
				list8 = transactionCurrencyList,
				list9 = transactionTypeList,
				list10 = transactionDateList,
				list11 = transactionAmountList,
				list12 = transactionRateList,
				list13 = transactionFeeList,
				error = "false"
			};

			// Die Daten in JSON umwandeln und zurückgeben
			return Json(daten);
		} else {
			var daten = new {
				error = "true"
			};
			// Die Daten in JSON umwandeln und zurückgeben
			return Json(daten);
		}
	}
}
