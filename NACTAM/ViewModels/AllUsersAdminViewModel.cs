using NACTAM.Models;

namespace NACTAM.ViewModels {
	/// <summary>
	/// Viewmodel containing a list of users to manage
	///
	/// author: Tuan Bui
	/// </summary>
	public class AllUsersAdminViewModel {
		/// <summary>
		/// Overview of users
		/// </summary>
		public IEnumerable<User> Users { get; set; }

		/// <summary>
		/// standart summary
		/// </summary>
		public AllUsersAdminViewModel(IEnumerable<User> users) {
			Users = users;
		}
	}
}
