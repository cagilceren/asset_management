using Microsoft.AspNetCore.Identity;

using NACTAM.ViewModels;

namespace NACTAM.Models.Repositories {


	/// <summary>
	/// helper interface for everything Account and User related, like the
	/// connections between <c>PrivatePerson</c> and <c>TaxAdvisor</c>
	///
	/// </summary>
	public interface IUserRepository {
		/// <summary>
		/// creates a new user from the viewmodel
		/// author: Tuan Bui
		/// </summary>
		public Task<IdentityResult> AddFromViewModel(CreateUserViewModel viewmodel);
		/// <summary>
		/// deletes user
		/// author: Tuan Bui
		/// </summary>
		public Task DeleteUser(User user);

		/// <summary>
		/// creates a new private person
		///
		/// author: Tuan Bui
		/// </summary>
		/// <param name="user">Data of a user</param>
		/// <param name="password">Password to be hashed</param>
		public Task<IdentityResult> AddPrivatePerson(PrivatePerson user, string password);

		/// <summary>
		/// creates a new taxadvisor
		///
		/// author: Tuan Bui
		/// </summary>
		/// <param name="user">Data of a user</param>
		/// <param name="password">Password to be hashed</param>
		public Task<IdentityResult> AddTaxAdvisor(TaxAdvisor user, string password);

		/// <summary>
		/// creates a new administrator
		///
		/// author: Tuan Bui
		/// </summary>
		/// <param name="user">Data of a user</param>
		/// <param name="password">Password to be hashed</param>
		public Task<IdentityResult> AddAdmin(Admin user, string password);

		/// <summary>
		/// Applies <c>values</c> values to the user
		/// author: Tuan Bui
		/// </summary>
		public Task UpdateUser(User user, User values, byte[]? profile);
		/// <summary>
		/// loads the advisor navigation attribute for a private person
		/// author: Tuan Bui
		/// </summary>
		public Task LoadAdvisors(PrivatePerson user);
		/// <summary>
		/// gets all of the insight allowances of a user
		/// author: Niklas Thuer
		/// </summary>
		public IEnumerable<InsightAllowance> GetAllowances(PrivatePerson user);
		/// <remarks>
		/// doesn't handle the case, if the request was revoked in the meantime
		/// author: Niklas Thuer
		/// </remarks>
		public Task AcceptRequest(PrivatePerson user, TaxAdvisor advisor, bool isExtended);
		/// <remarks>
		/// doesn't handle the case, if the request was revoked in the meantime
		/// author: Niklas Thuer
		/// </remarks>
		public Task RemoveInsight(PrivatePerson user, TaxAdvisor advisor);
		/// <remarks>
		/// doesn't handle the case, if the request was revoked in the meantime
		/// author: Niklas Thuer
		/// </remarks>
		public Task DenyRequest(PrivatePerson user, TaxAdvisor advisor, bool isExtended);

		/// <summary>
		/// sets darkmode to a certain value, but in database only
		/// author: Tuan Bui
		/// </summary>
		public Task SetDarkmode(User user, bool isDark);
		/// <summary>
		/// gets all Taxadvisors
		/// author: Niklas Thuer
		/// </summary>
		public IEnumerable<TaxAdvisor> GetAllTaxAdvisors();

		/// <summary>
		/// update the password
		/// </summary>
		public Task<IdentityResult> UpdatePassword(User user, string oldPassword, string newPassword);

		/// <summary>
		/// update send reset Email
		/// </summary>
		public Task<string> SendResetPasswordToken(User user);

		/// <summary>
		/// update send reset Email
		/// </summary>
		public Task<IdentityResult> ResetPassword(User user, string newPassword, string token);
	}
}
