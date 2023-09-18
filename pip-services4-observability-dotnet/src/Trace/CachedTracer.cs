using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using System;
using System.Collections.Generic;

namespace PipServices4.Observability.Trace
{
	/// <summary>
	/// Abstract tracer that caches recorded traces in memory and periodically dumps them.
	/// Child classes implement saving cached traces to their specified destinations.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - source:            source (context) name
	/// - options:
	///     - interval:        interval in milliseconds to save log messages(default: 10 seconds)
	///     - max_cache_size:  maximum number of messages stored in this cache (default: 100)    
	///     
	/// ### References ###
	/// 
	/// - *:context-info:*:*:1.0     (optional) <see cref="ContextInfo"/> to detect the context id and specify counters source
	/// 
	/// See <see cref="ITracer"/>, <see cref="OperationTrace"/>
	/// </summary>
	public abstract class CachedTracer : ITracer, IReconfigurable, IReferenceable
	{
		protected string _source = null;
		protected IList<OperationTrace> _cache = new List<OperationTrace>();
		protected bool _updated = false;
		protected long _lastDumpTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		protected int _maxCacheSize = 100;
		protected long _interval = 10000;

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters to be set.</param>
		public void Configure(ConfigParams config)
		{
			_interval = config.GetAsLongWithDefault("options.interval", _interval);
			_maxCacheSize = config.GetAsIntegerWithDefault("options.max_cache_size", _maxCacheSize);
			_source = config.GetAsStringWithDefault("source", _source);
		}

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies. </param>
		public void SetReferences(IReferences references)
		{
			var contextInfo = references.GetOneOptional<ContextInfo>(
				new Descriptor("pip-services", "context-info", "*", "*", "1.0"));

			if (contextInfo != null && _source == null)
				_source = contextInfo.Name;
		}

		/// <summary>
		/// Writes a log message to the logger destination.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="component">a name of called component</param>
		/// <param name="operation">a name of the executed operation. </param>
		/// <param name="error">an error object associated with this trace.</param>
		/// <param name="duration">execution duration in milliseconds. </param>
		protected void Write(string correlationId, string component, string operation, Exception error, long duration)
		{
			ErrorDescription errorDesc = error != null ? ErrorDescriptionFactory.Create(error) : null;
			OperationTrace trace = new OperationTrace
			{
				Time = DateTime.UtcNow,
				Source = _source,
				Component = component,
				Operation = operation,
				CorrelationId = correlationId,
				Duration = duration,
				Error = errorDesc
			};

			_cache.Add(trace);
			Update();
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
			Write(correlationId, component, operation, null, duration);
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
			Write(correlationId, component, operation, error, duration);
		}

		/// <summary>
		/// Begings recording an operation trace
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="component">a name of called component</param>
		/// <param name="operation">a name of the executed operation. </param>
		/// <returns></returns>
		public TraceTiming BeginTrace(string correlationId, string component, string operation)
		{
			return new TraceTiming(correlationId, component, operation, this);
		}

		/// <summary>
		/// Saves log messages from the cache.
		/// </summary>
		/// <param name="messages">a list with log messages</param>
		protected abstract void Save(IList<OperationTrace> messages);

		/// <summary>
		/// Clears (removes) all cached log messages.
		/// </summary>
		public void Clear()
		{
			_cache = new List<OperationTrace>();
			_updated = false;
		}

		/// <summary>
		/// Dumps (writes) the currently cached log messages.
		/// 
		/// See <see cref="Write(string, string, string, Exception, long)"/>
		/// </summary>
		public void Dump()
		{
			if (_updated)
			{
				if (!_updated) return;

				var traces = _cache;
				_cache = new List<OperationTrace>();

				try
				{
					Save(traces);
				}
				catch (Exception err)
				{
					// Adds traces back to the cache
					foreach (var item in _cache)
						traces.Add(item);
					_cache = traces;

					// Truncate cache
					var deleteCount = _cache.Count - _maxCacheSize;
					if (deleteCount > 0)
						((List<OperationTrace>)_cache).RemoveRange(0, deleteCount);
				}

				_updated = false;
				_lastDumpTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

			}
		}

		/// <summary>
		/// Makes trace cache as updated
		/// and dumps it when timeout expires.
		/// 
		/// See <see cref="Dump"/>
		/// </summary>
		protected void Update()
		{
			_updated = true;
			var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			if (now > this._lastDumpTime + this._interval)
			{
				try
				{
					Dump();
				}
				catch
				{
					// Todo: decide what to do
				}
			}
		}
	}
}
