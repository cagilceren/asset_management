using System.Collections;

using NACTAM.Models;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Viewmodel for requests view
	///
	/// author: Thuer Niklas 
	/// </summary>
	public class RequestsViewModel {

		/// <summary>
		/// All Requests of user
		/// </summary>
		public IEnumerable Requests;

		public RequestsViewModel(IEnumerable<InsightAllowance> allowances) {
			Requests = allowances.Where(x => x.Status == InsightStatus.SimpleUnaccepted || x.Status == InsightStatus.ExtendedUnaccepted);
		}
	}
}