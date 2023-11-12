using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using System;
using System.Text;

namespace PipServices4.Observability.Log
{
	/// <summary>
	/// Abstract logger that captures and formats log messages.
	/// Child classes take the captured messages and write them to their specific destinations.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// Parameters to pass to the Configure() method for component configuration:
	/// 
	/// - level:             maximum log level to capture
	/// - source:            source (context) name
	/// 
	/// ### References ###
	/// 
	/// - *:context-info:*:*:1.0     (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_info_1_1_context_info.html">ContextInfo</a> to detect the context id and specify counters source
	/// </summary>
	/// See <see cref="ILogger"/>
	public abstract class Logger : ILogger, IReconfigurable, IReferenceable
	{
		protected LogLevel _level = LogLevel.Info;
		protected string _source = null;

		/// <summary>
		/// Creates a new instance of the logger.
		/// </summary>
		protected Logger()
		{ }

		/// <summary>
		/// Gets or sets the maximum log level.
		/// </summary>
		public LogLevel Level
		{
			get { return _level; }
			set { _level = value; }
		}

		/// <summary>
		/// Gets or sets source (context) name.
		/// </summary>
		public string Source
		{
			get { return _source; }
			set { _source = value; }
		}

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters to be set.</param>
		public virtual void Configure(ConfigParams config)
		{
			_level = LogLevelConverter.ToLogLevel(
				config.GetAsObject("level") ?? _level);
			_source = config.GetAsStringWithDefault("source", _source);
		}

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies.</param>
		public virtual void SetReferences(IReferences references)
		{
			var contextInfo = references.GetOneOptional<ContextInfo>(
				new Descriptor("pip-services4", "context-info", "*", "*", "1.0"));
			if (contextInfo != null && _source == null)
				_source = contextInfo.Name;
		}

		protected abstract void Write(LogLevel level, IContext context, Exception error, string message);

		/// <summary>
		/// Composes an human-readable error description
		/// </summary>
		/// <param name="error">an error to format.</param>
		/// <returns>a human-reable error description.</returns>
		protected string ComposeError(Exception error)
		{
			var builder = new StringBuilder();

			builder.Append(error.Message);

			if (error.InnerException != null)
			{
				builder.Append(" Cause by: ");
				builder.Append("" + error.InnerException);
			}
			if (error.StackTrace != null)
			{
				builder.Append(" Stack trace: ");
				builder.Append(error.StackTrace);
			}

			return builder.ToString();
		}

		/// <summary>
		/// Formats the log message and writes it to the logger destination.
		/// </summary>
		/// <param name="level">a log level</param>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		protected void FormatAndWrite(LogLevel level, IContext context, Exception error, string message, object[] args)
		{
			var mes = !string.IsNullOrWhiteSpace(message) ? message : string.Empty;
			if (args != null && args.Length > 0)
				mes = string.Format(mes, args);

			Write(level, context, error, mes);
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
			FormatAndWrite(level, context, error, message, args);
		}

		/// <summary>
		/// Logs fatal (unrecoverable) message that caused the process to crash.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Fatal(IContext context, string message, params object[] args)
		{
			FormatAndWrite(LogLevel.Fatal, context, null, message, args);
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
			FormatAndWrite(LogLevel.Fatal, context, error, message, args);
		}

		/// <summary>
		/// Logs recoverable application error.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Error(IContext context, string message, params object[] args)
		{
			FormatAndWrite(LogLevel.Error, context, null, message, args);
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
			FormatAndWrite(LogLevel.Error, context, error, message, args);
		}

		/// <summary>
		/// Logs a warning that may or may not have a negative impact.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Warn(IContext context, string message, params object[] args)
		{
			FormatAndWrite(LogLevel.Warn, context, null, message, args);
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
			FormatAndWrite(LogLevel.Warn, context, error, message, args);
		}

		/// <summary>
		/// Logs an important information message
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Info(IContext context, string message, params object[] args)
		{
			FormatAndWrite(LogLevel.Info, context, null, message, args);
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
			FormatAndWrite(LogLevel.Info, context, error, message, args);
		}

		/// <summary>
		/// Logs a high-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Debug(IContext context, string message, params object[] args)
		{
			FormatAndWrite(LogLevel.Debug, context, null, message, args);
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
			FormatAndWrite(LogLevel.Debug, context, error, message, args);
		}

		/// <summary>
		/// Logs a low-level debug information for troubleshooting.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="message">a human-readable message to log.</param>
		/// <param name="args">arguments to parameterize the message.</param>
		public void Trace(IContext context, string message, params object[] args)
		{
			FormatAndWrite(LogLevel.Trace, context, null, message, args);
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
			FormatAndWrite(LogLevel.Trace, context, error, message, args);
		}
	}
}