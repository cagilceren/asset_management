
using System.Collections;

namespace NACTAM.Models.Repositories {

	/// <summary>
	/// helper interface for everything TaxAdvisor related
	///author: Niklas Thuer, Tuan Bui
	/// </summary>
	public interface ITaxAdvisorRepository {

		/// <summary>
		/// assigns user to taxadvisor
		/// </summary>
		/// <param name="myData">TaxAdvisor</param>
		/// <param name="userName">userName of user</param>
		public Task AssignUser(TaxAdvisor myData, string userName);
		/// <summary>
		/// get all users not assigned to taxadvisor
		/// </summary>
		/// <param name="myData">TaxAdvisor</param>
		public IEnumerable<PrivatePerson> GetAllCustomers(TaxAdvisor myData);
		/// <summary>
		/// get all users assigned to taxadvisor
		/// </summary>
		/// <param name="myData">TaxAdvisor</param>
		public IEnumerable<PrivatePerson> GetMyCustomers(TaxAdvisor myData);
		/// <summary>
		/// unassigns user from taxadvisor
		/// </summary>
		/// <param name="myData">TaxAdvisor</param>
		/// <param name="userName">userName of user</param>
		public Task RevokeUser(TaxAdvisor myData, string userName);
		/// <summary>
		/// change allowance status of insight allowance between user and taxadvisor
		/// </summary>
		/// <param name="myData">TaxAdvisor</param>
		/// <param name="userName">userName of user</param>
		/// <param name="status">new insight status</param>
		public Task ChangeAllowanceStatus(TaxAdvisor myData, String userName, InsightStatus status);
		/// <summary>
		/// check if insight allowance between user and advisor is extended
		/// </summary>
		/// <param name="myData">TaxAdvisor</param>
		/// <param name="userId">id of user</param>
		public bool CheckExtendedInsightStatus(TaxAdvisor myData, string userId);
		/// <summary>
		/// check if username is in database
		/// </summary>
		/// <param name="userName">userName of user</param>
		public Task<bool> CheckUserName(string userName);
		/// <summary>
		/// check if insight allowance between user and taxadvisor has specific status
		/// </summary>
		/// <param name="advisorUserName">username of advisor</param>
		/// <param name="userName">userName of user</param>
		/// <param name="shouldStatus">status which insight allowance should have</param>
		public Task<bool> CheckInsightStatus(string advisorUserName, string userName, InsightStatus shouldStatus);
		/// <summary>
		/// check if user is assigned to taxadvisor
		/// </summary>
		/// <param name="advisorUserName">username of TaxAdvisor</param>
		/// <param name="userName">userName of user</param>
		public Task<bool> CheckIsAssigned(string advisorUserName, string userName);
		/// <summary>
		/// check if user is not assigned to taxadvisor
		/// </summary>
		/// <param name="advisorUserName">username of TaxAdvisor</param>
		/// <param name="userName">userName of user</param>
		public Task<bool> CheckIsUnassigned(string advisorUserName, string userName);
		/// <summary>
		/// check if insight allowance between user and taxadvisor has the status or
		/// one with more access
		/// </summary>
		/// <param name="advisorUserName">username of advisor</param>
		/// <param name="userName">userName of user</param>
		/// <param name="shouldStatus">status which insight allowance should have</param>
		public Task<bool> CheckImplyStatus(string advisorUserName, string userName, InsightStatus shouldStatus);
	}
}
