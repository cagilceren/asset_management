namespace NACTAM.Exceptions;

/// <summary>
/// This exception is thrown when a illegal date is used and lead to internal errors
/// </summary>
public class IllegalDateException : Exception {
	/// <inheritdoc />
	public IllegalDateException(string message) : base(message) {
	}

	/// <inheritdoc />
	public IllegalDateException(string message, Exception inner) : base(message, inner) {
	}

	/// <inheritdoc />
	public IllegalDateException() : base() {
	}
}