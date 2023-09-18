using PipServices4.Commons.Convert;
using System;
using System.Text;

namespace PipServices4.Observability.Log
{
	/// <summary>
	/// Logger that writes log messages to console.
	/// 
	/// Errors are written to standard err stream
	/// and all other messages to standard out stream.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - level:             maximum log level to capture
	/// - source:            source(context) name
	/// 
	/// ### References ###
	/// 
	/// - *:context-info:*:*:1.0     (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_info_1_1_context_info.html">ContextInfo</a> to detect the context id and specify counters source
	/// </summary>
	/// <example>
	/// <code>
	/// var logger = new ConsoleLogger();
	/// logger.SetLevel(LogLevel.debug);
	/// 
	/// logger.Error("123", ex, "Error occured: %s", ex.message);
	/// logger.Debug("123", "Everything is OK.");
	/// </code>
	/// </example>
	public class ConsoleLogger : Logger
	{
		/// <summary>
		/// Writes a log message to the logger destination(s).
		/// </summary>
		/// <param name="level">a log level.</param>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		protected override void Write(LogLevel level, string correlationId, Exception error, string message)
		{
			if (Level < level) return;

			var build = new StringBuilder();
			build.Append('[');
			build.Append(correlationId != null ? correlationId : "---");
			build.Append(':');
			build.Append(level.ToString());
			build.Append(':');
			build.Append(StringConverter.ToString(DateTime.UtcNow));
			build.Append("] ");

			build.Append(message);

			if (error != null)
			{
				if (message.Length == 0)
					build.Append("Error: ");
				else
					build.Append(": ");

				build.Append(ComposeError(error));
			}

			var output = build.ToString();

			if (level == LogLevel.Fatal || level == LogLevel.Error || level == LogLevel.Warn)
				Console.Error.WriteLine(output);
			else
				Console.Out.WriteLine(output);
		}
	}
}