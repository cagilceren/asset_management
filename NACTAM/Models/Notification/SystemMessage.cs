namespace NACTAM.Models {
	/// <summary>
	/// Notification that gets sent from the system
	///
	/// Content DOESN'T GET ESCAPED, Text needs to be sanitized before the input
	///
	/// author: Tuan Bui
	/// </summary>
	public class SystemMessage : INotification {
		/// <inheritdoc/>
		public int Id { get; set; }

		public string Text { get; set; }

		/// <inheritdoc/>
		public User Recipient { get; set; } = null!;
		/// <inheritdoc/>
		public string RecipientId { get; set; }

		/// <inheritdoc/>
		public NotificationStatus IsRead { get; set; } = NotificationStatus.Unread;
		/// <inheritdoc/>
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		/// <inheritdoc/>
		public string ToDisplayText() {
			return $@"<a id=""notificationid-{Id}-SystemMessage"" class=""notificationtype-{IsRead} notification dropdown-item d-flex align-items-center"" href=""/NotificationList""><div class=""mr-3"">
						<div class=""icon-circle bg-primary"">
							<i class=""fas fa-wrench text-white""></i>
						</div>
					</div>
					<div>
						<div class=""small text-gray-500"">{CreatedAt.ToString()}{((INotification)this).GetBadge()}</div>
						<span {((INotification)this).IsReadText()}>
							{Text}
						</span>
					</div>
				</a>";
		}
	}
}
