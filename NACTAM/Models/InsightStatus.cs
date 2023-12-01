namespace NACTAM.Models {
	/// <summary>
	/// shows the current insight status between a private person and tax advisor
	///
	/// author: Tuan Bui, Niklas Thuer
	/// </summary>
	/// <remarks>
	/// The status types need to be sorted in ascending access order
	/// </remarks>
	public enum InsightStatus {
		/// <summary>
		/// User is assigned to the tax advisor, no insights possible so far
		/// </summary>
		Assigned = 0,
		/// <summary>
		/// Tax advisor requested simple insight, but not accepted so far
		/// </summary>
		SimpleUnaccepted = 1,
		/// <summary>
		/// simple insights possible so far
		/// </summary>
		Simple = 2,
		/// <summary>
		/// Tax advisor requested extended insight, but not accepted so far
		/// </summary>
		ExtendedUnaccepted = 3,
		/// <summary>
		/// all insights possible
		/// </summary>
		Extended = 4
	}
}
