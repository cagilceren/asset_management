using NACTAM.Models;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Viewmodel for All users
	///
	/// author: Thuer Niklas
	/// </summary>
	public class AllUsersViewModel {

		/// <summary>
		/// Users assigned to me
		/// </summary>
		public IEnumerable<PrivatePerson> MyUsers;
		/// <summary>
		/// Users not assigned to me
		/// </summary>
		public IEnumerable<PrivatePerson> AllOtherUsers;
		/// <summary>
		/// my TaxAdvisor Model
		/// </summary>
		private TaxAdvisor _myData;

		/// <summary>
		/// constructor for AllUsersViewModel
		/// </summary>
		public AllUsersViewModel(IEnumerable<PrivatePerson> MyUsers, IEnumerable<PrivatePerson> AllOtherUsers, TaxAdvisor myData) {
			this.MyUsers = MyUsers;
			this.AllOtherUsers = AllOtherUsers;
			this._myData = myData;
		}

		/// <summary>
		/// returns the number of Advisors assigned to given user
		/// </summary>
		public int GetNumberOfAdvisors(PrivatePerson user) {
			if (user.Advisors == null)
				return 0;

			if (user.Advisors.Contains(_myData)) {
				return user.Advisors.Count() - 1;
			} else {
				return user.Advisors.Count();
			}

		}
	}
}
