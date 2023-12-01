namespace NACTAM.Models {
	/// <summary>
	/// The current status of a notification
	///
	/// author: Tuan Bui
	/// </summary>
	public enum NotificationStatus {
		/// <summary>
		/// Default state of a notification
		/// </summary>
		Unread,
		/// <summary>
		/// state after a notification is hovered over
		/// is still visible in the header, but doesn't
		/// contribute to the notification count
		/// </summary>
		Read,
		/// <summary>
		/// state, when a notification is handeled, like
		/// after accepting a request
		///
		/// doesn't show up in the header, but is still
		/// visible in the notification page for journaling reasons
		/// </summary>
		Handeled,
		/// <summary>
		/// Appears, when a request gets revoked
		/// </summary>
		Undone
	}
}
