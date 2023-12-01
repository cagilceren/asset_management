using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace NACTAM.Models {
	/// <summary>
	/// Custom User Type based on IdentityUser (containing Email, Password, HashedPassword, UserName, Id)
	/// with custom Address, First Name, Last Name, Profile picture
	///
	/// and some custom user settings (like darkmode and email notifications)
	///
	///
	/// author: Tuan Bui
	/// </summary>
	public class User : IdentityUser {
		/// <summary>
		/// The value to check for either PrivatePerson, TaxAdvisor or Administrator
		/// </summary>
		public string DiscriminatorValue { get { return this.GetType().Name; } }

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
		/// optional profile picture
		/// </summary>
		public byte[]? Icon { get; set; } = null!;
		/// <summary>
		/// dark mode setting for user
		/// </summary>
		public bool DarkMode { get; set; } = false; // saved in db
		/// <summary>
		/// checks for email notificaton
		/// </summary>
		public bool EmailNotification { get; set; } = true;

		/// <summary>
		/// Check if all adress values of user are set
		/// </summary>
		public bool AddressSet() {
			return StreetName != null && HouseNumber != null && ZIP != null && City != null;
		}
	}
}
