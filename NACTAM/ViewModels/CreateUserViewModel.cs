using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using static Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal.RegisterModel;

namespace NACTAM.ViewModels;

/// <summary>
/// View model for creating users
///
/// author: Tuan Bui
/// </summary>
public class CreateUserViewModel {
	/// <summary>
	/// Input attribute made out of <c>Email</c>, <c>Password</c> and <c>ConfirmPassword</c>
	/// reusing the constraints made by AspNetCore
	/// </summary>
	[BindProperty]
	public InputModel Input { get; set; }

	/// <summary>
	/// unique <c>UserName</c>
	/// </summary>
	[Required]
	[MaxLength(50)]
	public string UserName { get; set; } = null!; // waiting for C#11 to be in usage

	/// <summary>
	/// <c>PhoneNumber</c> of the user
	/// </summary>
	[Required]
	[MaxLength(50)]
	[Phone]
	public string PhoneNumber { get; set; } = null!; // waiting for C#11 to be in usage

	/// <summary>
	/// First name of a user
	/// </summary>
	[Required]
	[MaxLength(50)]
	public string FirstName { get; set; } = null!; // waiting for C#11 to be in usage
	/// <summary>
	/// Last name of a user
	/// </summary>
	[Required]
	[MaxLength(50)]
	public string LastName { get; set; } = null!;

	/// <summary>
	/// optional Streetname
	/// </summary>
	[MaxLength(100, ErrorMessage = "Street name \"{0}\" is too long, {1} characters allowed")]
	public string? StreetName { get; set; }

	/// <summary>
	/// optional House number
	/// </summary>
	[MaxLength(10, ErrorMessage = "House name \"{0}\" is too long, {1} characters allowed")]
	[RegularExpression(@"\d+\s*\w*(\s*[\/-]\s*\d+\s*\w*)?", ErrorMessage = "doesn't match the house number format")]
	public string? HouseNumber { get; set; }

	/// <summary>
	/// optional city name
	/// </summary>
	[MaxLength(100, ErrorMessage = "City name \"{0}\" is too long, {1} characters allowed")]
	public string? City { get; set; }
	/// <summary>
	/// optional zip code
	/// </summary>
	[MaxLength(100, ErrorMessage = "Zip name \"{0}\" is too long, {1} characters allowed")]
	[DataType(DataType.PostalCode)]
	public string? ZIP { get; set; }

	/// <summary>
	/// checks for email notificaton
	/// </summary>
	public bool EmailNotification { get; set; } = true;
	/// <summary>
	/// optional profile picture as <c>IFormFile</c>
	///
	/// Needs to be converted first. In order to do that, use
	/// IconBytes
	/// </summary>
	public List<IFormFile>? Icon { get; set; }

	/// <summary>
	/// Constructor for a new viewmodel
	/// </summary>
	public CreateUserViewModel(InputModel input) {
		Input = input;
	}

	/// <summary>
	/// Type of user to be created
	/// </summary>
	[Required]
	[RegularExpression("Tax Advisor|Private Person")]
	public string Type { get; set; } = "Private Person";

	/// <summary>
	/// Converts <c>Icon</c> to a bytearray to store it
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
