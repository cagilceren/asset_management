using System.ComponentModel.DataAnnotations;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Viewmodel for updating user passwords
	///
	/// author: Tuan Bui
	/// </summary>
	public class PasswordChangeViewModel {
		/// <summary>
		/// the old/current password
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		public string OldPassword { get; set; } = "";
		/// <summary>
		/// the new password
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = "";
		/// <summary>
		/// repeating the new password
		/// </summary>
		[Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
		public string PasswordConfirmation { get; set; } = "";
	}
}
