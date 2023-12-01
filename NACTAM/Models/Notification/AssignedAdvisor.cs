namespace NACTAM.Models {
	/// <summary>
	/// Notification that gets sent for assigning a tax advisor to a user
	///
	/// Usernames get escaped, so XSS attacks are unlikely
	/// by this function (vuln may still be possible)
	///
	/// author: Tuan Bui
	/// </summary>
	public class AssignedAdvisor : INotification {
		/// <inheritdoc/>
		public int Id { get; set; }

		/// <inheritdoc/>
		public NotificationStatus IsRead { get; set; } = NotificationStatus.Unread;
		/// <inheritdoc/>
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		/// <inheritdoc/>
		public User Recipient { get; set; } = null!; // navigation propeties
		/// <inheritdoc/>
		public string RecipientId { get; set; } = null!;

		/// <summary>
		/// assigned tax advisor, navigation property
		/// </summary>
		public TaxAdvisor TaxAdvisor { get; set; } = null!; // navigation properties
		/// <summary>
		/// assigned tax advisor id
		/// </summary>
		public string TaxAdvisorId { get; set; } = null!;

		/// <inheritdoc/>
		public string ToDisplayText() {
			string isReadText = IsRead == NotificationStatus.Unread ? "" : "class=\"font-weight-bold\"";
			return $@"<a id=""notificationid-{Id}-AssignedAdvisor"" class=""notificationtype-{IsRead} dropdown-item d-flex align-items-center notification"" href=""/Settings/Insights""><div class=""mr-3"">
						<div class=""icon-circle bg-primary"">
							<img src=""/Home/ProfilePicture?UserName={TaxAdvisor.UserName}"" class=""notification-profile-pic""></img>
						</div>
					</div>
					<div>
						<div class=""small text-gray-500"">{CreatedAt.ToString()} {((INotification)this).GetBadge()}</div>
						<span {((INotification)this).IsReadText()}>
							<b>{System.Security.SecurityElement.Escape(TaxAdvisor.FirstName)} {System.Security.SecurityElement.Escape(TaxAdvisor.LastName)}</b>
							wurde als ihr Steuerberater zugeteilt.
						</span>
					</div>
				</a>";
		}
	}
}
