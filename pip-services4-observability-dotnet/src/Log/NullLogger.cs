using PipServices4.Components.Context;
using System;

namespace PipServices4.Observability.Log
{
	/// <summary>
	/// Dummy implementation of logger that doesn't do anything.
	/// 
	/// It can be used in testing or in situations when logger is required but shall be disabled.
	/// </summary>
	/// See <see cref="ILogger"/>
	public sealed class NullLogger : ILogger
	{
		/// <summary>
		/// Gets the maximum log level.
		/// </summary>
		public LogLevel Level
		{
			get { return LogLevel.None; }
			set { }
		}

		/// <summary>
		/// Logs a high-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Debug(IContext context, string message, params object[] args)
		{
		}

		/// <summary>
		/// Logs a high-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Debug(IContext context, Exception error, string message = null, params object[] args)
		{
		}

		/// <summary>
		/// Logs recoverable application error.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Error(IContext context, string message, params object[] args)
		{
		}

		/// <summary>
		/// Logs recoverable application error.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Error(IContext context, Exception error, string message = null, params object[] args)
		{
		}

		/// <summary>
		/// Logs fatal (unrecoverable) message that caused the process to crash.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Fatal(IContext context, string message, params object[] args)
		{
		}

		/// <summary>
		/// Logs fatal (unrecoverable) message that caused the process to crash.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Fatal(IContext context, Exception error, string message = null, params object[] args)
		{
		}

		/// <summary>
		/// Logs an important information message
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Info(IContext context, string message, params object[] args)
		{
		}

		/// <summary>
		/// Logs an important information message
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Info(IContext context, Exception error, string message = null, params object[] args)
		{
		}

		/// <summary>
		/// Logs a message at specified log level.
		/// </summary>
		/// <param name="level">a log level.</param>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Log(LogLevel level, IContext context, Exception error, string message, params object[] args)
		{
		}

		/// <summary>
		/// Logs a low-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Trace(IContext context, string message, params object[] args)
		{
		}

		/// <summary>
		/// Logs a low-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Trace(IContext context, Exception error, string message = null, params object[] args)
		{
		}

		/// <summary>
		/// Logs a warning that may or may not have a negative impact.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Warn(IContext context, string message, params object[] args)
		{
		}

		/// <summary>
		/// Logs a warning that may or may not have a negative impact.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Warn(IContext context, Exception error, string message = null, params object[] args)
		{
		}
	}
}