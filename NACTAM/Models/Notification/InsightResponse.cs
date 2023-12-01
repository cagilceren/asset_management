namespace NACTAM.Models {
	/// <summary>
	/// Notification that gets sent for either accepting or denying an insight
	///
	/// Usernames get escaped, so XSS attacks are unlikely
	/// by this function (vuln may still be possible)
	///
	/// author: Tuan Bui
	/// </summary>

	public class InsightResponse : INotification {
		/// <inheritdoc/>
		public int Id { get; set; }

		/// <inheritdoc/>
		public NotificationStatus IsRead { get; set; } = NotificationStatus.Unread;
		/// <inheritdoc/>
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public bool IsExtended { get; set; } = false;
		/// <inheritdoc/>
		public bool IsAccepted { get; set; } = false;

		/// <summary>
		/// assigned private person, navigation property
		/// </summary>
		public PrivatePerson PrivatePerson { get; set; } = null!;
		/// <summary>
		/// assigned private person id
		/// </summary>
		public string PrivatePersonId { get; set; }

		/// <inheritdoc/>
		public User Recipient { get; set; } = null!;
		/// <inheritdoc/>
		public string RecipientId { get; set; }

		/// <inheritdoc/>
		public string ToDisplayText() {
			string attr = IsExtended ? "erweiterten" : "einfachen";
			string accepted = IsAccepted ? "" : "nicht";
			return $@"<a id=""notificationid-{Id}-InsightResponse"" class=""notificationtype-{IsRead} notificaton notification dropdown-item d-flex align-items-center"" href=""/Advisor/MyUsers""><div class=""mr-3"">
						<div class=""icon-circle bg-primary"">
							<img src=""/Home/ProfilePicture?UserName={PrivatePerson.UserName}"" class=""notification-profile-pic""></img>
						</div>
					</div>
					<div>
						<div class=""small text-gray-500"">{CreatedAt.ToString()} {((INotification)this).GetBadge()}</div>
						<span {((INotification)this).IsReadText()}>
							<b>{System.Security.SecurityElement.Escape(PrivatePerson.FirstName)} {System.Security.SecurityElement.Escape(PrivatePerson.LastName)}</b> hat die {attr} Einsicht {accepted} angenommen.						</span>
					</div>
				</a>";
		}
	}
}
