using System.ComponentModel.DataAnnotations;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Viewmodel for updating user passwords
	/// author: Tuan Bui
	/// </summary>
	public class PasswordResetViewModel {
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

		/// <summary>
		/// Recognized token for auth to reset the password
		/// </summary>
		public string Token { get; set; }

		/// <summary>
		/// username of the user, whose password is supposed to be reset
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// empty constructor needed
		/// </summary>
		public PasswordResetViewModel() { }

		/// <summary>
		/// constructor for inserting the token
		/// </summary>
		public PasswordResetViewModel(string token, string userName) {
			Token = token;
			UserName = userName;
		}
	}
}
