using MimeKit;

namespace NACTAM.Models {
	/// <summary>
	/// Email with multiple recipients
	///
	/// Content is displayed as an HTML String and is therefore NOT ESCAPED
	///
	/// author: Tuan Bui
	/// </summary>

	public class Mail {
		public List<MailboxAddress> To { get; set; }
		/// <summary>
		/// subject of the email
		/// </summary>
		public string Subject { get; set; }
		/// <summary>
		/// content of the email
		/// </summary>
		public string Content { get; set; }

		/// <remark>
		/// content is HTML and not escaped by default!!!
		/// </remark>
		/// <summary>
		/// generates a new Mail
		/// </summary>
		/// <param name="to">List of users and user display names to send to</param>
		/// <param name="subject">Subject of email</param>
		/// <param name="content">Actual message as HTML string</param>
		public Mail(IEnumerable<(string, string)> to, string subject, string content) {
			To = new List<MailboxAddress>();
			To.AddRange(to.Select(x => new MailboxAddress(x.Item1, x.Item2)));
			Subject = subject;
			Content = content;
		}

		/// <summary>
		/// shortcut constructor for using a list of users
		/// </summary>
		/// <remark>
		/// content is HTML and not escaped by default!!!
		/// </remark>
		/// <param name="users">List of users</param>
		/// <param name="subject">Subject of email</param>
		/// <param name="content">Actual message as HTML string</param>
		public Mail(IEnumerable<User> users, string subject, string content) :
			this(users.Select(x => (x.FirstName + " " + x.LastName, x.Email)), subject, content) { }

		/// <summary>
		/// converts itself into a sendable datatype <c>MimeMessage</c>
		/// </summary>
		/// <param name="from">Username and Email of the sender</param>
		public MimeMessage ToMimeMessage((string, string) from) {
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress(from.Item1, from.Item2));
			emailMessage.To.AddRange(To);
			emailMessage.Subject = Subject;
			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = Content };
			return emailMessage;
		}
	}
}
