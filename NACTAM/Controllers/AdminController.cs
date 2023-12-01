using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NACTAM.Models;
using NACTAM.Models.Repositories;
using NACTAM.ViewModels;

using static Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal.RegisterModel;

namespace NACTAM.Controllers;

/// <summary>
/// Controller for everything Admin related
///
/// author: Tuan Bui
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller {
	private readonly ILogger<AdminController> _logger;

	private readonly UserManager<User> _userManager;
	private readonly IUserRepository _userRepository;

	/// <summary>
	/// Constructor with services, that are being dependency injected
	/// <paramref name="logger">a logger</paramref>
	/// <paramref name="userManager">the user manager</paramref>
	/// <paramref name="userRepository">the user container as repository</paramref>
	/// </summary>
	public AdminController(UserManager<User> userManager, IUserRepository userRepository, ILogger<AdminController> logger) {
		_logger = logger;
		_userRepository = userRepository;
		_userManager = userManager;
	}


	/// <summary>
	/// Shows an overview of all users for the admin
	/// </summary>
	public async Task<IActionResult> AllUsersAdmin() {
		return View(new AllUsersAdminViewModel((await _userManager.GetUsersInRoleAsync("TaxAdvisor")).Concat(await _userManager.GetUsersInRoleAsync("PrivatePerson"))));
	}

	/// <summary>
	/// Shows the view for creating a new user
	/// </summary>
	[HttpGet]
	public IActionResult CreateUser() {
		return View(new CreateUserViewModel(new InputModel { }));
	}

	/// <summary>
	/// Creates a new user from the <c>CreateUserViewModel</c>.
	///
	/// <paramref name="viewmodel">the viewmodel to be processed</paramref>
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> CreateUser(CreateUserViewModel viewmodel) {
		var userFromName = await _userManager.FindByNameAsync(viewmodel.UserName);
		if (userFromName != null) {
			ModelState.AddModelError("UserName", "UserName already exist.");
			return View(viewmodel);
		}

		var allErrors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));

		if (ModelState.IsValid) {
			var result = await _userRepository.AddFromViewModel(viewmodel);
			if (!result.Succeeded) {
				ModelState.AddModelError("Input.Password", string.Join(", ", result.Errors
					.Select(e => e.Description)));
				return View(viewmodel);
			}
			return RedirectToAction(nameof(AllUsersAdmin));
		}
		return View(viewmodel);
	}

	/// <summary>
	/// Shows the page for editing users
	/// </summary>
	[HttpGet]
	public async Task<IActionResult> EditUser(string id) {
		var user = await _userManager.FindByIdAsync(id);
		Console.WriteLine(user);
		Console.WriteLine(id);
		Console.WriteLine("DEBUG DEBUG DEBUG");
		return View("EditUserAdmin", new UserSettingsViewModel { CurrentUser = user });
	}

	/// <summary>
	/// Handles editing user data as an admin
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> EditUser(UserSettingsViewModel result) {
		var user = await _userManager.GetUserAsync(User);
		var allErrors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));
		var userFromName = await _userManager.FindByNameAsync(result.CurrentUser.UserName);
		if (userFromName != null && userFromName.Id != user.Id) {
			ModelState.AddModelError("User.UserName", "UserName already exist.");
			return View(result);
		}
		if (ModelState.IsValid) {
			if (userFromName != null)
				await _userRepository.UpdateUser(userFromName, result.CurrentUser, result.IconBytes);
			else
				await _userRepository.UpdateUser(await _userManager.FindByIdAsync(user.Id), result.CurrentUser, result.IconBytes);
		}
		return View(result);
	}

	/// <summary>
	/// handles the post request for deleting users
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> DeleteUser(string id) {
		var user = await _userManager.FindByIdAsync(id);
		await _userRepository.DeleteUser(user);
		return RedirectToAction(nameof(AllUsersAdmin));
	}
}
