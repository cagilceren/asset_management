using System.ComponentModel.DataAnnotations;

namespace NACTAM.Models {

	/// <summary>
	/// class for modeling the relationship between user and tax advisor
	///
	/// author: Tuan Bui
	/// </summary>
	public class InsightAllowance {
		/// <summary>
		/// primary key in database
		/// </summary>
		public int Id { get; set; }


		/// <summary>
		/// Id of the assigned private person
		/// </summary>
		public string UserId { get; set; }
		/// <summary>
		/// assigned private person
		/// </summary>
		public PrivatePerson User { get; set; } = null!; // navigation property

		/// <summary>
		/// Id of the assigned advisor
		/// </summary>
		public string AdvisorId { get; set; }
		/// <summary>
		/// assigned advisor
		/// </summary>
		public TaxAdvisor Advisor { get; set; } = null!; // navigation property

		/// <summary>
		/// status of the assignment
		/// </summary>
		[Required]
		public InsightStatus Status { get; set; } = InsightStatus.Assigned;

	}

}
