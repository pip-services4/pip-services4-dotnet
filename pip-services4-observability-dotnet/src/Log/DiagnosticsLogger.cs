using PipServices4.Commons.Convert;
using System;
using System.Text;

namespace PipServices4.Observability.Log
{
	/// <summary>
	/// Diagnostic logger.
	/// </summary>
	public class DiagnosticsLogger : Logger
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

			switch (level)
			{
				case LogLevel.Fatal:
				case LogLevel.Error:
					System.Diagnostics.Trace.TraceError(output);
					break;
				case LogLevel.Warn:
					System.Diagnostics.Trace.TraceWarning(output);
					break;
				case LogLevel.Info:
					System.Diagnostics.Trace.TraceInformation(output);
					break;
				case LogLevel.Debug:
				case LogLevel.Trace:
					System.Diagnostics.Debug.WriteLine(output);
					break;
			}
		}
	}
}
