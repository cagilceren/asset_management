namespace NACTAM.Seeding {
	/// <summary>
	/// Interface for creating a service to seed data
	///
	/// author: Tuan Bui
	/// </summary>
	public interface ISeeding {
		/// <summary>
		/// function that does the seeding
		/// </summary>
		public Task Init();
	}
}
