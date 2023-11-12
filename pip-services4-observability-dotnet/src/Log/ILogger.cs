using PipServices4.Components.Context;
using System;

namespace PipServices4.Observability.Log
{
	/// <summary>
	/// Interface for logger components that capture execution log messages.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Gets or sets the maximum log level. Messages with higher log level are filtered out.
		/// </summary>
		LogLevel Level { get; set; }

		/// <summary>
		/// Logs a message at specified log level.
		/// </summary>
		/// <param name="level">a log level.</param>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Log(LogLevel level, IContext context, Exception error, string message, params object[] args);

		/// <summary>
		/// Logs fatal (unrecoverable) message that caused the process to crash.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Fatal(IContext context, string message, params object[] args);
		/// <summary>
		/// Logs fatal (unrecoverable) message that caused the process to crash.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Fatal(IContext context, Exception error, string message = null, params object[] args);

		/// <summary>
		/// Logs recoverable application error.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Error(IContext context, string message, params object[] args);
		/// <summary>
		/// Logs recoverable application error.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Error(IContext context, Exception error, string message = null, params object[] args);

		/// <summary>
		/// Logs a warning that may or may not have a negative impact.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Warn(IContext context, string message, params object[] args);
		/// <summary>
		/// Logs a warning that may or may not have a negative impact.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Warn(IContext context, Exception error, string message = null, params object[] args);

		/// <summary>
		/// Logs an important information message
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Info(IContext context, string message, params object[] args);
		/// <summary>
		/// Logs an important information message
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Info(IContext context, Exception error, string message = null, params object[] args);

		/// <summary>
		/// Logs a high-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Debug(IContext context, string message, params object[] args);
		/// <summary>
		/// Logs a high-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Debug(IContext context, Exception error, string message = null, params object[] args);

		/// <summary>
		/// Logs a low-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Trace(IContext context, string message, params object[] args);
		/// <summary>
		/// Logs a low-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		void Trace(IContext context, Exception error, string message = null, params object[] args);
	}
}