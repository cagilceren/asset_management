using NACTAM.Models;


namespace NACTAM.ViewModels {


	/// <summary>
	/// Viewmodel for MyUsers View
	///
	/// author: Thuer Niklas
	/// </summary>
	public class MyUsersViewModel {
		/// <summary>
		/// All users assigned to taxadvisor
		/// </summary>
		public IEnumerable<PrivatePerson> Users;
		/// <summary>
		/// All insight allowances of taxadvisor
		/// </summary>
		public List<InsightAllowance> InsightAllowances;

		public MyUsersViewModel(TaxAdvisor myData) {
			Users = (myData.Customers ?? new List<PrivatePerson>()).OrderBy(x => x.UserName);
			InsightAllowances = myData.Allowances ?? new List<InsightAllowance>();
		}

		/// <summary>
		/// Get status of insight allowance with user with given username
		/// </summary>
		public InsightStatus? GetInsightAllowance(String userName) {
			return InsightAllowances.FirstOrDefault(x => x.User.UserName == userName)?.Status;
		}
	}
}
