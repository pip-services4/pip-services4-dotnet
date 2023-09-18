using PipServices4.Components.Refer;
using System;
using System.Collections.Generic;

namespace PipServices4.Observability.Log
{
	/// <summary>
	/// Aggregates all loggers from component references under a single component.
	/// 
	/// It allows to log messages and conveniently send them to multiple destinations.
	/// 
	/// ### References ###
	/// 
	/// - *:logger:*:*:1.0 	(optional) ILogger components to pass log messages
	/// </summary>
	/// <example>
	/// <code>
	/// class MyComponent: IConfigurable, IReferenceable 
	/// {
	///     CompositeLogger _logger = new CompositeLogger();
	///     public void Configure(ConfigParams config)
	///     {
	///         this._logger.Configure(config);
	///         ...
	///     }
	///     public void SetReferences(IReferences references)
	///     {
	///         this._logger.SetReferences(references);
	///         ...
	///     }
	///     public void MyMethod(String correlationId)
	///     {
	///         this._logger.Debug(correlationId, "Called method mycomponent.mymethod");
	///         ...
	///     }
	/// }
	/// </code>
	/// </example>
	/// See <see cref="ILogger"/>
	public class CompositeLogger : Logger
	{
		protected readonly List<ILogger> _loggers = new List<ILogger>();

		/// <summary>
		/// Creates a new instance of the logger.
		/// </summary>
		/// <param name="references">references to locate the component dependencies.</param>
		public CompositeLogger(IReferences references = null)
		{
			Level = LogLevel.Trace;

			if (references != null)
				SetReferences(references);
		}

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies.</param>
		public override void SetReferences(IReferences references)
		{
			base.SetReferences(references);

			var loggers = references.GetOptional<ILogger>(new Descriptor(null, "logger", null, null, null));
			foreach (var logger in loggers)
			{
				if (logger != this)
					_loggers.Add(logger);
			}
		}

		/// <summary>
		/// Writes a log message to the logger destination(s).
		/// </summary>
		/// <param name="level">a log level.</param>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		protected override void Write(LogLevel level, string correlationId, Exception error, string message)
		{
			foreach (var logger in _loggers)
				logger.Log(level, correlationId, error, message);
		}
	}
}