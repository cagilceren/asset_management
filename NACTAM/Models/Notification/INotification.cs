namespace NACTAM.Models {
	/// <summary>
	/// general interface for combining Notifications into a list
	///
	/// author: Tuan Bui
	/// </summary>
	public interface INotification {

		/// <summary>
		/// current state of the notification
		/// </summary>
		public NotificationStatus IsRead { get; set; }

		/// <summary>
		/// id of the recipient
		/// </summary>
		public string RecipientId { get; set; }
		/// <summary>
		/// User the notification is being sent to, most likely an autoloaded
		/// navigation property
		/// </summary>
		public User Recipient { get; set; }

		/// <summary>
		/// Creation Date of the notification
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// Uniform helper function to generate HTML of a notification
		/// </summary>
		public string ToDisplayText();

		/// <summary>
		/// makes the HTML attributes for each type of message
		/// </summary>
		public string IsReadText() {
			return IsRead == NotificationStatus.Unread ? "class=\"unread-notifications\"" :
				IsRead == NotificationStatus.Undone ? "class=\"undone-notifications\"" :
				IsRead == NotificationStatus.Handeled ? "class=\"handeled-notifications\"" :
				"";
		}

		/// <summary>
		/// returns the badge displayed on top of the message in HTML
		/// </summary>
		public string GetBadge() {
			return IsRead == NotificationStatus.Unread ? "<span class=\"badge badge-info\">New</span>" :
				IsRead == NotificationStatus.Undone ? "<span class=\"badge badge-light\">Undone</span>" :
				IsRead == NotificationStatus.Handeled ? "<span class=\"badge badge-secondary\">Handeled</span>" :
				"";
		}
	}
}
