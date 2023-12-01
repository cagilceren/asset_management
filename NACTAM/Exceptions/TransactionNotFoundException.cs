namespace NACTAM.Exceptions;
// white an exception for rate limit
using System;

/// <summary>
///  This exception is thrown when a transaction is not found
/// </summary>
public class TransactionNotFoundException : Exception {
	/// <inheritdoc />
	public TransactionNotFoundException() {
	}

	/// <inheritdoc />
	public TransactionNotFoundException(string message)
		: base(message) {
	}

	/// <inheritdoc />
	public TransactionNotFoundException(string message, Exception inner)
		: base(message, inner) {
	}
}