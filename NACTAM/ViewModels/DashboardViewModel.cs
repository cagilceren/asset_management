namespace NACTAM.ViewModels {
	/// <summary>
	/// A view model for Dashboard.
	/// </summary>
	/// <author> Cagil Ceren Aslan </author>
	public class DashboardViewModel {

		/// <summary>
		/// Constructor for Dashboard View Model
		/// </summary>
		public DashboardViewModel() {
			ExpensesData = new();
			IncomeData = new();
			NetEarningsData = new();
		}

		/// <summary>
		/// Gives number of transaction
		/// </summary>
		/// <value></value>
		public decimal NumberOfTransactions { get; set; }

		/// <summary>
		/// Gives a number of total expenses
		/// </summary>
		/// <value></value>
		public decimal TotalExpenses { get; set; }

		/// <summary>
		/// Gives a number of total income
		/// </summary>
		/// <value></value>
		public decimal TotalIncome { get; set; }

		/// <summary>
		/// Gives a number of total profit
		/// </summary>
		/// <value></value>
		public decimal TotalProfit { get; set; }

		/// <summary>
		/// Gives a list of expenses
		/// </summary>
		/// <value></value>
		public List<decimal> ExpensesData { get; set; }

		/// <summary>
		/// Gives a list of income
		/// </summary>
		/// <value></value>
		public List<decimal> IncomeData { get; set; }

		/// <summary>
		/// Gives a list of profit
		/// </summary>
		/// <value></value>
		public List<decimal> NetEarningsData { get; set; }
	}
}