namespace NACTAM.Exceptions;
// white an exception for rate limit
using System;

/// <summary>
/// This exception is thrown when a illegal user operation is performed
/// </summary>
public class IllegalUserOperationException : Exception {
	/// <inheritdoc />
	public IllegalUserOperationException() {
	}

	/// <inheritdoc />
	public IllegalUserOperationException(string message)
		: base(message) {
	}

	/// <inheritdoc />
	public IllegalUserOperationException(string message, Exception inner)
		: base(message, inner) {
	}
}