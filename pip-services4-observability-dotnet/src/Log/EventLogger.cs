using PipServices4.Components.Context;
using System;

namespace PipServices4.Observability.Log
{
	public class EventLogger : Logger
	{
		protected override void Write(LogLevel level, IContext context, Exception error, string message)
		{
			if (error != null)
			{
				if (string.IsNullOrWhiteSpace(message))
					message = "Error: " + ComposeError(error);
				else
					message = message + ": " + ComposeError(error);
			}

			switch (level)
			{
				case LogLevel.Fatal:
					LogEventSource.Current.Fatal(context, message);
					break;
				case LogLevel.Error:
					LogEventSource.Current.Error(context, message);
					break;
				case LogLevel.Warn:
					LogEventSource.Current.Warn(context, message);
					break;
				case LogLevel.Info:
					LogEventSource.Current.Info(context, message);
					break;
				case LogLevel.Debug:
					LogEventSource.Current.Debug(context, message);
					break;
				case LogLevel.Trace:
					LogEventSource.Current.Trace(context, message);
					break;
			}
		}
	}
}
