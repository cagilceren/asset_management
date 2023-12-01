namespace NACTAM.Models {

	/// <summary>
	/// class to model tax advisors
	/// </summary>
	public class TaxAdvisor : User {
		/// <summary>
		/// List of assigned customers
		/// </summary>
		public List<PrivatePerson> Customers { get; set; } = null!; // navigation property
		/// <summary>
		/// List of assigned allowances
		/// </summary>
		public List<InsightAllowance> Allowances { get; set; } = null!; // navigation property
	}

}
