using Microsoft.EntityFrameworkCore;

using NACTAM.Identity.Data;
using NACTAM.Models.Repositories;

namespace NACTAM.Models {
	/// <summary>
	/// helper class for managing notifications per database and email
	///
	/// author: Tuan Bui
	/// </summary>
	public class NotificationContainer : INotificationRepository {
		private readonly NACTAMContext _db;
		private readonly IEmailSender _sender;
		public NotificationContainer(NACTAMContext db, IEmailSender sender) {
			_db = db;
			_sender = sender;
		}

		public async Task AddAssignedAdvisor(TaxAdvisor taxAdvisor, PrivatePerson user) {
			var notification = new AssignedAdvisor { Recipient = user, TaxAdvisor = taxAdvisor };
			await _db.AssignedAdvisor.AddAsync(notification);
			await _db.SaveChangesAsync();

			if (user.EmailNotification)
				await _sender.SendEmailAsync(new Mail(new List<User> { user }, "Assigned a new advisor", notification.ToDisplayText()));
		}

		public async Task AddSystemMessage(User user, string text) {
			var notification = new SystemMessage { Text = System.Security.SecurityElement.Escape(text) };
			await _db.SystemMessage.AddAsync(notification);
			await _db.SaveChangesAsync();

			if (user.EmailNotification)
				await _sender.SendEmailAsync(new Mail(new List<User> { user }, "System Message", notification.ToDisplayText()));
		}

		public async Task AddInsightRequest(TaxAdvisor taxAdvisor, PrivatePerson user, bool isExtended) {
			var notification = new InsightRequest { Recipient = user, TaxAdvisor = taxAdvisor, IsExtended = isExtended, RecipientId = user.Id, TaxAdvisorId = taxAdvisor.Id };
			await _db.InsightRequest.AddAsync(notification);
			await _db.SaveChangesAsync();

			if (user.EmailNotification)
				await _sender.SendEmailAsync(new Mail(new List<User> { user }, "New insight request", notification.ToDisplayText()));

		}

		public async Task AddInsightResponse(PrivatePerson customer, TaxAdvisor user, bool isExtended, bool isAccepted) {
			var notification = new InsightResponse { Recipient = user, PrivatePerson = customer, IsExtended = isExtended, IsAccepted = isAccepted };
			await _db.InsightResponse.AddAsync(notification);
			await _db.SaveChangesAsync();

			if (user.EmailNotification)
				await _sender.SendEmailAsync(new Mail(new List<User> { user }, "New insight response", notification.ToDisplayText()));
		}

		public async Task RemoveAssignedAdvisor(TaxAdvisor taxAdvisor, PrivatePerson user) {
			var notification = new RevokedAdvisor { Recipient = user, TaxAdvisor = taxAdvisor };
			await _db.RevokedAdvisor.AddAsync(notification);
			await _db.SaveChangesAsync();

			if (user.EmailNotification)
				await _sender.SendEmailAsync(new Mail(new List<User> { user }, "Removed advisor", notification.ToDisplayText()));
		}

		public IEnumerable<INotification> NotificationsFor(User user) {
			return ((IEnumerable<INotification>)_db.InsightRequest.Where(x => x.Recipient == user))
				.Concat(_db.InsightResponse.Where(x => x.Recipient == user))
				.Concat(_db.RevokedAdvisor.Where(x => x.Recipient == user))
				.Concat(_db.AssignedAdvisor.Where(x => x.Recipient == user))
				.Concat(_db.SystemMessage.Where(x => x.Recipient == user))
				.OrderByDescending(x => x.CreatedAt);
		}

		/// <summary>
		/// helper function for marking everything as read
		/// </summary>
		public async Task MarkAll(User user) {
			await _db.InsightRequest
				.Where(x => x.IsRead == NotificationStatus.Unread && x.Recipient == user)
				.ExecuteUpdateAsync(s => s.SetProperty(e => e.IsRead, e => NotificationStatus.Read));
			await _db.InsightResponse
				.Where(x => x.IsRead == NotificationStatus.Unread && x.Recipient == user)
				.ExecuteUpdateAsync(s => s.SetProperty(e => e.IsRead, e => NotificationStatus.Read));
			await _db.RevokedAdvisor
				.Where(x => x.IsRead == NotificationStatus.Unread && x.Recipient == user)
				.ExecuteUpdateAsync(s => s.SetProperty(e => e.IsRead, e => NotificationStatus.Read));
			await _db.AssignedAdvisor
				.Where(x => x.IsRead == NotificationStatus.Unread && x.Recipient == user)
				.ExecuteUpdateAsync(s => s.SetProperty(e => e.IsRead, e => NotificationStatus.Read));
			await _db.SaveChangesAsync();
		}

		/// <summary>
		/// Changes all unhandeled Insightrequests to undone
		///
		/// author: Tuan Bui
		/// </summary>
		public async Task UndoInsightRequest(TaxAdvisor taxAdvisor, User user, bool isExtended) {
			foreach (var insightRequest in _db.InsightRequest
				.Where(x => x.TaxAdvisor == taxAdvisor && x.Recipient == user && x.IsExtended == isExtended && (x.IsRead == NotificationStatus.Unread || x.IsRead == NotificationStatus.Read)))
				insightRequest.IsRead = NotificationStatus.Undone;
			await _db.SaveChangesAsync();
		}

		/// <summary>
		/// Changes all unhandeled Insightrequests to handeled
		///
		/// author: Tuan Bui
		/// </summary>
		public async Task HandleInsightRequest(TaxAdvisor taxAdvisor, User user, bool isExtended) {
			foreach (var insightRequest in _db.InsightRequest
				.Where(x => x.TaxAdvisor == taxAdvisor && x.Recipient == user && x.IsExtended == isExtended && (x.IsRead == NotificationStatus.Unread || x.IsRead == NotificationStatus.Read)))
				insightRequest.IsRead = NotificationStatus.Handeled;
			await _db.SaveChangesAsync();
		}

		/// <summary>
		/// Marks an unread notification as read
		///
		/// author: Tuan Bui
		/// </summary>
		public async Task ReadNotification(int notificationId, User user, string notType) {
			INotification? notification = null;
			if (notType == "AssignedAdvisor")
				notification = _db.AssignedAdvisor.FirstOrDefault(x => x.Id == notificationId);
			else if (notType == "InsightRequest")
				notification = _db.InsightRequest.FirstOrDefault(x => x.Id == notificationId);
			else if (notType == "InsightResponse")
				notification = _db.InsightResponse.FirstOrDefault(x => x.Id == notificationId);
			else if (notType == "RevokedAdvisor")
				notification = _db.RevokedAdvisor.FirstOrDefault(x => x.Id == notificationId);
			else if (notType == "SystemMessage")
				notification = _db.SystemMessage.FirstOrDefault(x => x.Id == notificationId);


			if (notification != null && notification.Recipient == user && notification.IsRead == NotificationStatus.Unread) {
				notification.IsRead = NotificationStatus.Read;
				await _db.SaveChangesAsync();
			}
		}
	}
}
