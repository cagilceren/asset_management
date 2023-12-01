using Microsoft.AspNetCore.Mvc;

namespace NACTAM.Controllers;

/// <summary>
/// Controller for error pages
///
/// author: Niklas Thuer, Tuan Bui
/// </summary>
public class ErrorController : Controller {
	/// <summary>
	/// handles error codes
	/// </summary>
	/// <param name="statusCode">Error code</param>
	[Route("/Error/{statusCode}")]
	public IActionResult HttpStatusCodeHandler(int statusCode) {
		if (statusCode == 403)
			return View("NoPermission");
		if (statusCode == 400)
			return RedirectToAction("Login", "Home");
		return View(statusCode.ToString());
	}
}
