using PipServices4.Commons.Convert;
using PipServices4.Components.Refer;
using PipServices4.Observability.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Performance counters that periodically dumps counters measurements to logger.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// options:
	/// - interval:        interval in milliseconds to save current counters measurements(default: 5 mins)
	/// - reset_timeout:   timeout in milliseconds to reset the counters. 0 disables the reset(default: 0)
	/// 
	/// ### References ###
	/// 
	/// - *:logger:*:*:1.0           <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to dump the captured counters
	/// - *:context-info:*:*:1.0     (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_info_1_1_context_info.html">ContextInfo</a> to detect the context id and specify counters source
	/// </summary>
	/// <example>
	/// <code>
	/// var counters = new LogCounters();
	/// counters.SetReferences(References.fromTuples(
	///     new Descriptor("pip-services4", "logger", "console", "default", "1.0"), new ConsoleLogger()
	/// ));
	/// 
	/// counters.Increment("mycomponent.mymethod.calls");
	/// var timing = counters.BeginTiming("mycomponent.mymethod.exec_time");
	/// try {
	///     ...
	///     }
	/// finally {
	/// timing.EndTiming();
	/// }
	/// 
	/// counters.dump();
	/// </code>
	/// </example>
	public class LogCounters : CachedCounters, IReferenceable
	{
		private readonly CompositeLogger _logger = new CompositeLogger();

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies.</param>
		public virtual void SetReferences(IReferences references)
		{
			_logger.SetReferences(references);
		}

		private string CounterToString(Counter counter)
		{
			var result = "Counter " + counter.Name + " { ";
			result += "\"type\": " + counter.Type;
			if (counter.Last != null)
				result += ", \"last\": " + StringConverter.ToString(counter.Last);
			if (counter.Count != null)
				result += ", \"count\": " + StringConverter.ToString(counter.Count);
			if (counter.Min != null)
				result += ", \"min\": " + StringConverter.ToString(counter.Min);
			if (counter.Max != null)
				result += ", \"max\": " + StringConverter.ToString(counter.Max);
			if (counter.Average != null)
				result += ", \"avg\": " + StringConverter.ToString(counter.Average);
			if (counter.Time != null)
				result += ", \"time\": " + StringConverter.ToString(counter.Time);
			result += " }";
			return result;
		}

		/// <summary>
		/// Saves the current counters measurements.
		/// </summary>
		/// <param name="counters">current counters measurements to be saves.</param>
		protected override void Save(IEnumerable<Counter> counters)
		{
			if (_logger == null || counters == null)
				return;

			var countersArray = counters as Counter[] ?? counters.ToArray();

			if (!countersArray.Any()) return;

			new List<Counter>(countersArray)
				.Sort((c1, c2) => string.Compare(c1.Name, c2.Name, StringComparison.Ordinal));

			foreach (var counter in countersArray)
			{
				_logger.Info(null, CounterToString(counter));
			}
		}
	}
}
