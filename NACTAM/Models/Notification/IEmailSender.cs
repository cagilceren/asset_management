namespace NACTAM.Models {
	/// <summary>
	/// Service for sending emails,
	/// cautious: Mail doesn't sanitize HTML inputs for more flexibility!
	///
	/// author: Tuan Bui
	/// </summary>
	public interface IEmailSender {
		/// <summary>
		/// sends Email
		/// </summary>
		Task SendEmailAsync(Mail message);
		/// <summary>
		/// gets the URL that is currently connected
		/// </summary>
		public (string?, string?) GetURL();
	}
}
