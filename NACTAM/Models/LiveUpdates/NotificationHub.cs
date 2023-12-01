using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

using NACTAM.Models;
using NACTAM.Models.Repositories;

namespace NACTAM.Hubs {
	/// <summary>
	/// the SignalR hub for notifications and other live sent logic
	///
	/// author: Tuan Bui
	/// </summary>
	[Authorize(Roles = "PrivatePerson,TaxAdvisor")]
	public class NotificationHub : Hub {

		private readonly UserManager<User> _userManager;
		private readonly IUserRepository _userRep;
		private readonly INotificationRepository _notRep;

		/// <summary>
		/// Constructor, where everything is inserted by dependency injection
		///
		/// <paramref name="userManager">user manager for getting user information</paramref>
		/// <paramref name="notRep">repository interface for <c>NotificationContainer</c></paramref>
		/// <paramref name="userRep">repository interface for <c>UserContainer</c>, in order to set darkmode</paramref>
		/// </summary>
		public NotificationHub(INotificationRepository notRep, UserManager<User> userManager, IUserRepository userRep) : base() {
			_userManager = userManager;
			_notRep = notRep;
			_userRep = userRep;
		}

		/// <summary>
		/// function to be called to broadcast message
		///
		/// <paramref name="user">user by whom this is sent</paramref>
		/// <paramref name="message">message to be sent</paramref>
		/// </summary>
		public async Task SendMessage(string user, string message) {
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

		/// <summary>
		/// changes the darkmode live
		///
		/// <paramref name="isDark">the state to be changed to</paramref>
		/// </summary>
		public async Task SetDarkMode(bool isDark) {
			var user = await _userManager.GetUserAsync(Context.User);
			await _userRep.SetDarkmode(user, isDark);
			await Clients
				.User(user.Id)
				.SendAsync("UpdateDarkMode", isDark);
		}

		/// <summary>
		/// marks a notification as read
		///
		/// <paramref name="notificationId">notification id</paramref>
		/// <paramref name="notType">notification type in order to get the right table</paramref>
		/// </summary>
		public async Task Read(int notificationId, string notType) {
			var user = await _userManager.GetUserAsync(Context.User);
			await _notRep.ReadNotification(notificationId, user, notType);
			await Clients
				.User(user.Id)
				.SendAsync("ReceiveNotify", _notRep.NotificationsToHTML(_notRep.NotificationsHeader(user)), _notRep.CountUnread(user));
			await Clients
				.User(user.Id)
				.SendAsync("ReceiveNotifyCenter", _notRep.NotificationsToHTML(_notRep.NotificationsFor(user)), _notRep.CountUnread(user));
		}
	}
}
