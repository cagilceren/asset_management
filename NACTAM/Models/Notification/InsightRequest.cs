namespace NACTAM.Models {
	/// <summary>
	/// Notification that gets sent for requesting an insight
	///
	/// Usernames get escaped, so XSS attacks are unlikely
	/// by this function (vuln may still be possible)
	///
	/// author: Tuan Bui
	/// </summary>

	public class InsightRequest : INotification {
		/// <remarks>
		/// appending the id to the frontend could impose a
		/// small security risk, because an attacker can now
		/// estimate the number of notifications being sent
		/// </remarks>
		public int Id { get; set; }

		/// <inheritdoc/>
		public NotificationStatus IsRead { get; set; } = NotificationStatus.Unread;
		/// <inheritdoc/>
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		/// <summary>
		/// bool for extendeded Insight request
		/// </summary>
		public bool IsExtended { get; set; } = false;

		/// <inheritdoc/>
		public User Recipient { get; set; } = null!;
		/// <inheritdoc/>
		public string RecipientId { get; set; } = null!;

		/// <summary>
		/// assigned tax advisor
		/// </summary>
		public TaxAdvisor TaxAdvisor { get; set; } = null!; // navigation properties
		/// <summary>
		/// assigned tax advisor id
		/// </summary>
		public string TaxAdvisorId { get; set; } = null!;

		/// <inheritdoc/>
		public string ToDisplayText() {
			string attr = IsExtended ? "erweiterten" : "einfachen";
			return $@"<a id=""notificationid-{Id}-InsightRequest"" class=""notificationtype-{IsRead} notification dropdown-item d-flex align-items-center"" href=""/Settings/Insights/Requests""><div class=""mr-3"">
						<div class=""icon-circle bg-primary"">
							<i class=""fas fa-file-alt text-white""></i>
						</div>
					</div>
					<div>
						<div class=""small text-gray-500"">{CreatedAt.ToString()} {((INotification)this).GetBadge()}</div>
						<span {((INotification)this).IsReadText()}>
							<b>{System.Security.SecurityElement.Escape(TaxAdvisor.FirstName)} {System.Security.SecurityElement.Escape(TaxAdvisor.LastName)}</b> fragt nach einer {attr} Einsicht.
						</span>
					</div>
				</a>";
		}
	}
}
