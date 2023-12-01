using NACTAM.Models;
using NACTAM.ViewModels.TaxRecommendation;

namespace NACTAM.ViewModels {

	/// <summary>
	/// Viewmodel for simple insight partial view
	///
	/// author: Thuer Niklas
	/// </summary>
	public class SimpleInsightViewModel {

		/// <summary>
		/// Index list over number of years
		/// </summary>
		public List<int> Index = new List<int>();
		/// <summary>
		/// All years in a list
		/// </summary>
		public List<int> Years;
		/// <summary>
		/// All profits in a list
		/// </summary>
		public List<decimal> ProfitsPerYear;
		/// <summary>
		/// All mining and staking profits in a list
		/// </summary>
		public List<decimal> MiningAndStakingPerYear;

		public SimpleInsightViewModel(List<int> years, List<decimal> profitsPerYear, List<decimal> miningAndStakingPerYear) {
			this.Years = years;
			this.ProfitsPerYear = profitsPerYear;
			this.MiningAndStakingPerYear = miningAndStakingPerYear;
			for (int i = 0; i < years.Count(); i++) {
				Index.Add(i);
			}
		}

		/// <summary>
		/// Check if there is a year to show
		/// </summary>
		public bool NoYearsToShow() {
			return !Years.Any();
		}
	}
}