namespace NACTAM.Models {
	/// <summary>
	/// Private person representation
	///
	/// inherits from user
	/// </summary>
	public class PrivatePerson : User {
		/// <summary>
		/// navigation property, is set by entity framework core
		///
		/// needs to be included
		/// </summary>
		public List<TaxAdvisor> Advisors { get; set; } = null!; // navigation property
		/// <summary>
		/// list of allowances
		/// </summary>
		public List<InsightAllowance> Allowances { get; set; } = null!; // navigation property

	}
}
