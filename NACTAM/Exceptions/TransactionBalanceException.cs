namespace NACTAM.Exceptions;

/// <summary>
/// This exception is thrown when a operation would lead to a negative balance and therefore is not allowed because internal errors would occur
/// </summary>
public class TransactionBalanceException : Exception {
	/// <inheritdoc />
	public TransactionBalanceException(string message) : base(message) {
	}

	/// <inheritdoc />
	public TransactionBalanceException(string message, Exception inner) : base(message, inner) {
	}

	/// <inheritdoc />
	public TransactionBalanceException() : base() {
	}
}