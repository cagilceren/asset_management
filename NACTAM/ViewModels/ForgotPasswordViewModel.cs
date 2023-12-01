using System.ComponentModel.DataAnnotations;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Viewmodel for resetting passwords
	///
	/// author: Tuan Bui
	/// </summary>
	public class ForgotPasswordViewModel {
		/// <summary>
		/// Email address to send the reset link to
		/// </summary>
		[EmailAddress]
		public string Email { get; set; } = "";
	}
}
