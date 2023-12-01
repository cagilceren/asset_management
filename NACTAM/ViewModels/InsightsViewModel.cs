using System.Collections;

using NACTAM.Models;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Viewmodel for Insights Page
	///
	/// author: Thuer Niklas
	/// </summary>
	public class InsightsViewModel {

		/// <summary>
		/// all insights of user
		/// </summary>
		public IEnumerable Insights;
		/// <summary>
		/// Number of unhandled requests
		/// </summary>
		public int RequestNumber;

		public InsightsViewModel(IEnumerable<InsightAllowance> allowances) {
			Insights = allowances;
			RequestNumber = allowances.Where(x => x.Status == InsightStatus.SimpleUnaccepted || x.Status == InsightStatus.ExtendedUnaccepted).Count();
		}
	}
}