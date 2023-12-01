using Microsoft.AspNetCore.Mvc;

using NACTAM.Models;

namespace NACTAM.ViewModels;

/// <summary>
/// View model for the settings view
///
/// author: Tuan Bui
/// </summary>
/// <remarks>
/// Icon is not saved by <c>CurrentUser</c>
/// </remarks>
public class UserSettingsViewModel {
	/// <summary>
	/// data of the current user
	/// </summary>
	[BindProperty]
	public User CurrentUser { get; set; }

	/// <summary>
	/// Profile picture input
	/// </summary>
	public List<IFormFile>? Icon { get; set; }

	/// <summary>
	/// Converts profile picture to byte array
	/// </summary>
	public byte[]? IconBytes {
		get {
			if (this.Icon is null)
				return null;
			using var ms = new MemoryStream();
			this.Icon[0].CopyTo(ms);
			return ms.ToArray();
		}
	}

}
