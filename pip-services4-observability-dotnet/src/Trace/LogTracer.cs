using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Observability.Log;
using System;
using System.Text;

namespace PipServices4.Observability.Trace
{
	/// <summary>
	/// Tracer that dumps recorded traces to logger.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - options:
	///     - log_level:         log level to record traces(default: debug)
	///     
	/// ### References ###
	/// 
	/// - *:logger:*:*:1.0          <see cref="ILogger"/> components to dump the captured counters
	/// - *:context-info:*:*:1.0     (optional) <see cref="ContextInfo"/> to detect the context id and specify counters source
	/// 
	/// See <see cref="Tracer"/>, <see cref="CachedCounters"/>, <see cref="CompositeLogger"/>
	/// 
	/// <example>
	/// <code>
	/// var tracer = new LogTracer();
	/// tracer.SetReferences(References.FromTuples(
	///     new Descriptor("pip-services", "logger", "console", "default", "1.0"), new ConsoleLogger()
	/// ));
	/// 
	/// var timing = trcer.BeginTrace("123", "mycomponent", "mymethod");
	/// try {
	///     ...
	///     timing.EndTrace();
	/// } catch
	/// {
	///     timing.EndFailure(err);
	/// }
	/// </code>
	/// </example>
	/// </summary>
	public class LogTracer : IConfigurable, IReferenceable, ITracer
	{
		private readonly CompositeLogger _logger = new CompositeLogger();
		private LogLevel _logLevel = LogLevel.Debug;

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters to be set.</param>
		public void Configure(ConfigParams config)
		{
			_logLevel = LogLevelConverter.ToLogLevel(
				config.GetAsObject("options.log_level"),
				_logLevel
			);
		}

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies. </param>
		public void SetReferences(IReferences references)
		{
			_logger.SetReferences(references);
		}

		private void LogTrace(string correlationId, string component, string operation, Exception error, long duration)
		{
			StringBuilder builder = new StringBuilder();

			if (error != null)
				builder.Append("Failed to execute ");
			else
				builder.Append("Executed ");

			builder.Append(component);
			builder.Append('.');
			builder.Append(operation);

			if (duration > 0)
				builder.Append(" in " + duration + " msec");

			if (error != null)
				_logger.Error(correlationId, error, builder.ToString());
			else
				_logger.Log(_logLevel, correlationId, null, builder.ToString());
		}

		/// <summary>
		/// Records an operation trace with its name and duration
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="component">a name of called component</param>
		/// <param name="operation">a name of the executed operation. </param>
		/// <param name="duration">execution duration in milliseconds. </param>
		public void Trace(string correlationId, string component, string operation, long duration)
		{
			LogTrace(correlationId, component, operation, null, duration);
		}

		/// <summary>
		/// Records an operation failure with its name, duration and error
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="component">a name of called component</param>
		/// <param name="operation">a name of the executed operation. </param>
		/// <param name="error">an error object associated with this trace.</param>
		/// <param name="duration">execution duration in milliseconds. </param>
		public void Failure(string correlationId, string component, string operation, Exception error, long duration)
		{
			LogTrace(correlationId, component, operation, error, duration);
		}

		/// <summary>
		/// Begings recording an operation trace
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="component">a name of called component</param>
		/// <param name="operation">a name of the executed operation. </param>
		/// <returns>a trace timing object.</returns>
		public TraceTiming BeginTrace(string correlationId, string component, string operation)
		{
			return new TraceTiming(correlationId, component, operation, this);
		}
	}
}
