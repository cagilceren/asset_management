using System.ComponentModel.DataAnnotations;

namespace NACTAM.Models {
	public class TaxAllowance {
		public int Id { get; set; }

		[Required]
		public int Year { get; set; }

		[Required]
		public decimal Amount { get; set; }
	}
}
