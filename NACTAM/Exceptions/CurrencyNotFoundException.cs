namespace NACTAM.Exceptions;
using System;


/// <summary>
/// This exception is thrown when a currency is not found
/// </summary>
public class CurrencyNotFoundException : Exception {
	/// <inheritdoc />
	public CurrencyNotFoundException() {
	}

	/// <inheritdoc />
	public CurrencyNotFoundException(string message)
		: base(message) {
	}

	/// <inheritdoc />
	public CurrencyNotFoundException(string message, Exception inner)
		: base(message, inner) {
	}


}