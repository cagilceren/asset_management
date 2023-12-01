namespace NACTAM.Models.Repositories {

	/// <summary>
	/// Repository class as an interface for <c>NotificationContainer</c>
	/// for dependency injection
	///
	/// author: Tuan Bui
	/// </summary>
	public interface INotificationRepository {
		/// <summary>
		/// adds another insight request between an advisor and a private person
		///
		/// <param name="taxAdvisor">requester of the message</param>
		/// <param name="user">recipient of the message</param>
		/// <param name="isExtended">determines, whether an extended  or simple insight is being requested</param>
		///
		/// </summary>
		public Task AddInsightRequest(TaxAdvisor taxAdvisor, PrivatePerson user, bool isExtended);

		public Task UndoInsightRequest(TaxAdvisor taxAdvisor, User user, bool isExtended);

		public Task HandleInsightRequest(TaxAdvisor taxAdvisor, User user, bool isExtended);

		public Task AddInsightResponse(PrivatePerson customer, TaxAdvisor user, bool isExtended, bool isAccepted);

		public Task AddAssignedAdvisor(TaxAdvisor taxAdvisor, PrivatePerson user);

		public Task RemoveAssignedAdvisor(TaxAdvisor taxAdvisor, PrivatePerson user);

		public IEnumerable<INotification> NotificationsFor(User user);

		public IEnumerable<INotification> NotificationsUnread(User user)
			=> NotificationsFor(user).Where(x => x.IsRead == NotificationStatus.Unread);

		public IEnumerable<INotification> NotificationsHeader(User user)
			=> NotificationsFor(user).Where(x => x.IsRead == NotificationStatus.Unread || x.IsRead == NotificationStatus.Read);

		public Task MarkAll(User user);

		public string NotificationsToHTML(IEnumerable<INotification> notifications)
			=> string.Join("\n", notifications.Select(x => x.ToDisplayText()));

		public int CountUnread(User user) => NotificationsUnread(user).Count();

		public Task ReadNotification(int notificationId, User user, string notType);
	}
}
