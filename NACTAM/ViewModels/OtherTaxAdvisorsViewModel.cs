using System.Collections;

using NACTAM.Models;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Viewmodel for partial View otherTaxAdvisorView
	///
	/// author: Thuer Niklas
	/// </summary>
	public class OtherTaxAdvisorsViewModel {

		/// <summary>
		/// all assigned advisor of user except this advisor
		/// </summary>
		public IEnumerable<TaxAdvisor> AssignedAdvisors;
		/// <summary>
		/// all unassigned advisor of user except this advisor
		/// </summary>
		public IEnumerable<TaxAdvisor> UnassignedAdvisors;

		/// <summary>
		/// username of selcted user
		/// </summary>
		public string SelectedUser;

		public OtherTaxAdvisorsViewModel(IEnumerable<TaxAdvisor> assignedAdvisors, IEnumerable<TaxAdvisor> unassignedAdvisors, string userName) {
			this.AssignedAdvisors = assignedAdvisors;
			this.UnassignedAdvisors = unassignedAdvisors;
			SelectedUser = userName;
		}

		/// <summary>
		/// check if any advisor is assigned to selcted user
		/// </summary>
		public bool NoAssignedAdvisors() {
			return !AssignedAdvisors.Any();
		}
	}
}
