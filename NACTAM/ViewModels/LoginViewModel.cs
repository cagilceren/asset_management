using System.ComponentModel.DataAnnotations;

namespace NACTAM.ViewModels {
	/// <summary>
	/// Viewmodel for login
	///
	/// author: Tuan Bui
	/// </summary>
	public class LoginViewModel {
		/// <summary>
		/// Username input, case insensitve
		/// </summary>
		[MaxLength(50)]
		public string UserName { get; set; } = "";

		/// <summary>
		/// Password input
		/// </summary>
		[MaxLength(128)]
		[DataType(DataType.Password)]
		public string Password { get; set; } = "";

		/// <summary>
		/// Will you remember me?
		/// </summary>
		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; } = false;
	}
}
