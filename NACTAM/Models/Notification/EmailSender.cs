
using MailKit.Net.Smtp;

using MimeKit;

namespace NACTAM.Models {
	/// <summary>
	/// Service that sends emails, gets injected by IEmailSender
	///
	/// author: Tuan bui
	/// </summary>
	public class EmailSender : IEmailSender {
		private readonly EmailConfiguration _emailConfig;
		private readonly IHttpContextAccessor _httpContextAccessor;

		/// <summary>
		/// Constructor where parameters are dependencies injected
		/// </summary>
		/// <param name="emailConfig">email config from appsettings</param>
		/// <param name="httpContextAccessor">for getting the remota address</param>
		public EmailSender(EmailConfiguration emailConfig, IHttpContextAccessor httpContextAccessor) {
			_emailConfig = emailConfig;
			_httpContextAccessor = httpContextAccessor;
		}

		/// <inheritdoc/>
		public (string?, string?) GetURL()
			=> (_httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(), _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress + ":" + _httpContextAccessor.HttpContext?.Connection?.RemotePort);


		/// <inheritdoc/>
		public async Task SendEmailAsync(Mail message) {
			message.Content += $@"<a href=""{_httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress}/DisableEmails"" href=""{_httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress}:{_httpContextAccessor.HttpContext?.Connection?.RemotePort}/DisableEmails"" style=""display: block;margin-top: 80%;color: #ccc;"">I don't want any emails anymore</a>";
			MimeMessage mailMessage = message.ToMimeMessage((_emailConfig.From, _emailConfig.From));
			using (var client = new SmtpClient()) {
				try {
					await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
					client.AuthenticationMechanisms.Remove("XOAUTH2");
					await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
					await client.SendAsync(mailMessage);
				} catch {
					//log an error message or throw an exception or both.
					throw;
				} finally {
					await client.DisconnectAsync(true);
					client.Dispose();
				}
			}
		}
	}
}
