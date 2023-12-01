using System.Security.Authentication;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NACTAM.Models;
using NACTAM.Models.API;
using NACTAM.Models.Repositories;
using NACTAM.Models.TaxRecommendation;
using NACTAM.ViewModels;
namespace NACTAM.Controllers;

public class HomeController : Controller {
	private readonly ILogger<HomeController> _logger;
	private readonly SignInManager<User> _signInManager;
	private readonly UserManager<User> _userManager;
	private readonly UserManager<PrivatePerson> _privatePersonManager;
	private readonly UserManager<TaxAdvisor> _taxAdvisorManager;
	private readonly IUserRepository _rep;
	private readonly INotificationRepository _nrep;

	private readonly ITransactionRepository _transactionRepository;

	private readonly ITaxCalculations _taxCalculations;
	private readonly ICurrencyApi _api;

	/// <summary>
	/// <paramref name="api">Coinvalue API like Coingecko</paramref>
	/// <paramref name="logger">Logger</paramref>
	/// <paramref name="userManager">UserManager</paramref>
	/// <paramref name="taxCalculations">Tax Calculation Service</paramref>
	/// <paramref name="signInManager">sign in manager</paramref>
	/// <paramref name="notificationRepository">interface for <c>NotificationContainer</c></paramref>
	/// <paramref name="privatePersonManager">UserManager for private persons</paramref>
	/// <paramref name="taxAdvisorManager">UserManager for tax advisors</paramref>
	/// <paramref name="userRepository">interface for <c>UserContainer</c></paramref>
	/// <paramref name="transactionRepository">interface for <c>Transaction Container</c></paramref>
	///
	/// author: Tuan Bui
	/// </summary>
	public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager, UserManager<User> userManager, IUserRepository userRepository, INotificationRepository notificationRepository, UserManager<PrivatePerson> privatePersonManager, UserManager<TaxAdvisor> taxAdvisorManager, ITransactionRepository transactionRepository, ITaxCalculations taxCalculations, ICurrencyApi api) {
		_logger = logger;
		_signInManager = signInManager;
		_userManager = userManager;
		_taxAdvisorManager = taxAdvisorManager;
		_rep = userRepository;
		_nrep = notificationRepository;
		_privatePersonManager = privatePersonManager;
		_transactionRepository = transactionRepository;
		_taxCalculations = taxCalculations;
		_api = api;
	}

	/// <summary>
	/// Index page:
	///
	/// redirects to either the landing page, the dashboard or the overview
	/// of assigned customers, depending on the status (either
	/// unauthorized or signed in as <c>PrivatePerson</c>/<c>TaxAdvisor</c>)
	///
	/// creates Dashboard view.
	///
	/// author: Tuan Bui and Cagil Ceren Aslan
	/// </summary>
	/// <returns>view, that the user is authorized to</returns>
	public async Task<IActionResult> Index() {
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		var transactions = new TransactionsOverviewViewModel(_transactionRepository.GetTransactions(userId), _api).Transactions;
		DashboardViewModel dashboard = _taxCalculations.CalculateEarningsPerMonth(transactions, DateTime.Now.Year);
		if (_signInManager.IsSignedIn(User)) {
			var signedUser = await _userManager.GetUserAsync(User);
			if (signedUser == null)
				return View();

			if (signedUser.DiscriminatorValue == "PrivatePerson")
				return View("Dashboard", dashboard);
			else if (signedUser.DiscriminatorValue == "TaxAdvisor")
				return RedirectToAction("MyUsers", "Advisor");
			else if (signedUser.DiscriminatorValue == "Admin")
				return RedirectToAction("AllUsersAdmin", "Admin");
			throw new Exception("Unknown role");
		} else {
			return View();
		}
	}

	/// <summary>
	/// helper function, that redirects to the landing page with the login
	/// form opened
	///
	/// author: Tuan Bui
	/// </summary>
	/// <returns>Redirect to landing page</returns>
	[HttpGet("/Login")]
	public IActionResult Login() {
		return Redirect("/?login=true");
	}

	/// <summary>
	/// login logic for authentication
	///
	/// <paramref name="model"> VIewmodel of the login</paramref>
	/// author: Tuan Bui
	/// </summary>
	/// <returns>View</returns>
	[HttpPost("/Login")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginViewModel model) {
		if (ModelState.IsValid) {
			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout,
			// set lockoutOnFailure: true
			var result = await _signInManager.PasswordSignInAsync(model.UserName,
							   model.Password, model.RememberMe, lockoutOnFailure: false);
			if (result.Succeeded) {
				_logger.LogInformation("User logged in.");
				return RedirectToAction(nameof(Index));
			} else if (result.IsLockedOut) {
				_logger.LogWarning("User account locked out.");
				return RedirectToPage("./Lockout");
			} else {
				ModelState.AddModelError("", "Invalid login attempt.");
			}
		}

		// In this case something failed
		return View(nameof(Index));
	}

	/// <summary>
	/// Logout post request
	///
	/// author: Tuan Bui
	/// </summary>
	[HttpPost("/Logout")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Logout() {
		await _signInManager.SignOutAsync();
		_logger.LogInformation("User logged out");
		return RedirectToAction(nameof(Index));
	}

	/// <summary>
	/// Webpage for accessing the user settings
	///
	/// author: Tuan Bui
	/// </summary>
	[Authorize]
	[HttpGet("/Settings")]
	public async Task<IActionResult> Settings() {
		return View(new UserSettingsViewModel { CurrentUser = await _userManager.GetUserAsync(User) });
	}

	/// <summary>
	/// Backend for accepting the sent user settings
	///
	/// author: Tuan Bui
	/// </summary>
	[Authorize]
	[HttpPost("/Settings")]
	public async Task<IActionResult> Settings(UserSettingsViewModel viewmodel) {
		var authUser = await _userManager.GetUserAsync(User);
		if (viewmodel.CurrentUser.Id != authUser.Id && authUser.DiscriminatorValue != "Admin") { // unauthorized access
			ModelState.AddModelError("", "Unauthorized User ID access");
			return View(viewmodel);
		}
		var prevUser = await _userManager.FindByIdAsync(viewmodel.CurrentUser.Id);
		if (prevUser.UserName != viewmodel.CurrentUser.UserName) {
			var userFromName = await _userManager.FindByNameAsync(viewmodel.CurrentUser.UserName);
			if (userFromName != null && userFromName.Id != authUser.Id) {
				ModelState.AddModelError("CurrentUser.UserName", "UserName already exist.");
				return View(viewmodel);
			}
		}

		var allErrors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));
		Console.WriteLine(string.Join(" | ", ModelState.Values
			.SelectMany(v => v.Errors)
			.Select(e => e.ErrorMessage)));

		if (ModelState.IsValid && (viewmodel.CurrentUser.Id == authUser.Id || authUser.DiscriminatorValue == "Admin")) {
			await _rep.UpdateUser(prevUser, viewmodel.CurrentUser, viewmodel.IconBytes);
		}
		return View(viewmodel);
	}

	/// <summary>
	/// Overview of all the currently active insights into the account of a user
	///
	/// author: Niklas Thuer
	/// </summary>
	[Authorize(Roles = "PrivatePerson")]
	[HttpGet("Settings/Insights")]
	public async Task<IActionResult> Insights() {
		var user = await _privatePersonManager.GetUserAsync(User);
		return View(new InsightsViewModel(_rep.GetAllowances(user)));
	}

	/// <summary>
	/// Overview of all the currently unhandled requests of a user
	///
	/// author: Niklas Thuer
	/// </summary>
	[Authorize(Roles = "PrivatePerson")]
	[HttpGet("Settings/Insights/Requests")]
	public async Task<ViewResult> Requests() {
		var user = await _privatePersonManager.GetUserAsync(User);
		return View(new RequestsViewModel(_rep.GetAllowances(user)));
	}

	/// <summary>
	/// Function to accept a request
	///
	/// <paramref name="userName">username of user</paramref>
	/// <paramref name="isExtended">true if extended</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	[Authorize(Roles = "PrivatePerson")]
	[HttpPost]
	public async Task<ActionResult> AcceptAdvisor(string userName, bool isExtended) {
		var advisor = await _taxAdvisorManager.FindByNameAsync(userName);
		var user = await _privatePersonManager.GetUserAsync(User);
		await _rep.AcceptRequest(user, advisor, isExtended);
		return RedirectToAction(nameof(Insights));
	}

	/// <summary>
	/// Function to deny a request
	///
	/// <paramref name="userName">username of user</paramref>
	/// <paramref name="isExtended">true if extended</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	[Authorize(Roles = "PrivatePerson")]
	[HttpPost]
	public async Task<ActionResult> DenyAdvisor(string userName, bool isExtended) {
		var advisor = await _taxAdvisorManager.FindByNameAsync(userName);
		var user = await _privatePersonManager.GetUserAsync(User);
		try {
			await _rep.DenyRequest(user, advisor, isExtended);
		} catch (InvalidOperationException) {
			ModelState.AddModelError("", "Die Einsicht wurde in der Zwischenzeit gelöscht");
		}
		return RedirectToAction(nameof(Insights));
	}

	/// <summary>
	/// Function to Remove a insight
	///
	/// <paramref name="userName">username of taxadvisor</paramref>
	///
	/// author: Niklas Thuer
	/// </summary>
	[Authorize(Roles = "PrivatePerson")]
	[HttpPost]
	public async Task<ActionResult> RemoveInsight(string userName) {
		var advisor = await _taxAdvisorManager.FindByNameAsync(userName);
		var user = await _privatePersonManager.GetUserAsync(User);
		await _rep.RemoveInsight(user, advisor);
		return RedirectToAction(nameof(Insights));
	}

	/// <summary>
	/// Gives back the profile picture, if either the user requesting or requested is
	/// a Taxadvisor or the same person as the person being authenticated
	///
	/// <param name="userName"> username as input </param>
	/// author: Tuan Bui
	/// </summary>
	[Authorize]
	public async Task<FileContentResult> ProfilePicture(string userName) {
		var user = await _userManager.GetUserAsync(User);
		var requestedUser = user.UserName == userName ? user : await _userManager.FindByNameAsync(userName);
		if (user.DiscriminatorValue == "Admin" || requestedUser?.DiscriminatorValue == "TaxAdvisor" || user.DiscriminatorValue == "TaxAdvisor" || requestedUser?.DiscriminatorValue == "PrivatePerson" && requestedUser.Id == user.Id) {
			return new FileContentResult(requestedUser?.Icon ?? new byte[] { }, "image/jpeg");
		}

		return new FileContentResult(new byte[] { }, "image/jpeg");
	}

	/// <summary>
	/// Displays all notifications, even handeled or undone ones
	///
	/// author: Tuan Bui
	/// </summary>
	[Authorize]
	[HttpGet("/NotificationList")]
	public ViewResult NotificationList() {
		return View();
	}

	/// <summary>
	/// Update password view
	///
	/// author: Tuan Bui
	/// </summary>
	[Authorize]
	[HttpGet("/PasswordUpdate")]
	public ViewResult PasswordUpdate() {
		return View(new PasswordChangeViewModel());
	}

	/// <summary>
	/// Reset password view
	///
	/// author: Tuan Bui
	/// </summary>
	[HttpGet("/ResetPassword")]
	public ViewResult ResetPassword(string token, string name) {
		return View(new PasswordResetViewModel(token, name));
	}

	/// <summary>
	/// Resetting the password
	///
	/// author: Tuan Bui
	/// </summary>
	[HttpPost("/ResetPassword")]
	public async Task<IActionResult> ResetPassword(PasswordResetViewModel viewmodel) {
		if (!ModelState.IsValid) {
			return View(viewmodel);
		}
		var user = await _userManager.FindByNameAsync(viewmodel.UserName);
		var result = await _rep.ResetPassword(user, viewmodel.PasswordConfirmation, viewmodel.Token);

		if (!result.Succeeded) {
			ModelState.AddModelError("Password", string.Join(", ", result.Errors
				.Select(e => e.Description)));
			return View(viewmodel);
		}

		return RedirectToAction(nameof(Index));
	}

	/// <summary>
	/// Update password view
	///
	/// <param name="viewmodel">viewmodel to be checked</param>
	/// author: Tuan Bui
	/// </summary>
	[Authorize]
	[HttpPost("/PasswordUpdate")]
	public async Task<IActionResult> PasswordUpdate(PasswordChangeViewModel viewmodel) {
		if (!ModelState.IsValid) {
			return View(viewmodel);
		}
		var user = await _userManager.GetUserAsync(User);
		try {
			var result = await _rep.UpdatePassword(user, viewmodel.OldPassword, viewmodel.PasswordConfirmation);
			if (!result.Succeeded) {
				ModelState.AddModelError("Password", string.Join(", ", result.Errors
					.Select(e => e.Description)));
				return View(viewmodel);
			}
		} catch (InvalidCredentialException e) {
			ModelState.AddModelError("OldPassword", e.Message);
			return View(viewmodel);
		}
		return RedirectToAction(nameof(Settings));
	}

	/// <summary>
	/// handling the forgot password button and sends the recovery emails
	///
	///
	/// </summary>
	public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewmodel) {
		if (!ModelState.IsValid) {
			return View(nameof(Index));
		}
		var user = await _userManager.FindByEmailAsync(viewmodel.Email);
		if (user == null) {
			ViewBag.FeedbackPasswordReset = "die Email wurde gesendet!";
			// acting like email was sent for security reasons
			//
			// otherwise you could bruteforce usernames
			return View(nameof(Index));
		}
		await _rep.SendResetPasswordToken(user);
		ViewBag.FeedbackPasswordReset = "die Email wurde gesendet!";
		return View(nameof(Index));
	}
}
