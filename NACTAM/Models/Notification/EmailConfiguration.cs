namespace NACTAM.Models {
	/// <summary>
	/// helper class for reading the configuration for
	/// Email specific attributes
	///
	/// Usage: <c>builder.Configuration.GetSection("EmailConfiguration")?.Get&lt;EmailConfiguration&gt;() ?? throw new InvalidOperationException("Missing Email configuration")</c>
	///
	/// author: Tuan Bui
	/// </summary>
	public class EmailConfiguration {
		/// <summary>
		/// Email address from which the emails are sent from
		/// </summary>
		public string From { get; set; } = null!; // gets set by JSON reader
		/// <summary>
		/// SMTP Email Server, which hosts the email address
		/// </summary>
		public string SmtpServer { get; set; } = null!; // gets set by JSON reader
		/// <summary>
		/// Port number for the SMTP Server, SMTP: 25 (insecure!), SMTPS: 587 or 465
		/// </summary>
		public int Port { get; set; }
		/// <summary>
		/// username of the corresponding email address,
		/// is sometimes set to be the same as the email address
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// Password for the email address: You might want to use
		/// App passwords to avoid issues with authentication
		/// </summary>
		public string Password { get; set; }
	}
}
