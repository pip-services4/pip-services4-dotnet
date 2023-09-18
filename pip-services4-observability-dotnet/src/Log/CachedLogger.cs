using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using System;
using System.Collections.Generic;

namespace PipServices4.Observability.Log
{
	/// <summary>
	/// Abstract logger that caches captured log messages in memory and periodically dumps them.
	/// Child classes implement saving cached messages to their specified destinations.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - level:             maximum log level to capture
	/// - source:            source(context) name
	/// 
	/// options:
	/// - interval:        interval in milliseconds to save log messages(default: 10 seconds)
	/// - max_cache_size:  maximum number of messages stored in this cache (default: 100)        
	/// 
	/// ### References ###
	/// 
	/// - *:context-info:*:*:1.0     (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_info_1_1_context_info.html">ContextInfo</a> to detect the context id and specify counters source
	/// </summary>
	/// See <see cref="ILogger"/>, <see cref="Logger"/>, <see cref="LogMessage"/>
	public abstract class CachedLogger : Logger, IReconfigurable
	{
		protected List<LogMessage> _cache = new List<LogMessage>();
		protected bool _updated = false;
		protected long _lastDumpTime = DateTime.UtcNow.Ticks;
		protected int _maxCacheSize = 100;
		protected int _interval = 10000;
		protected object _lock = new object();

		/// <summary>
		/// Writes a log message to the logger destination.
		/// </summary>
		/// <param name="level">a log level.</param>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="error">an error object associated with this message.</param>
		/// <param name="message">a human-readable message to log.</param>
		protected override void Write(LogLevel level, string correlationId, Exception error, string message)
		{
			ErrorDescription errorDescription = error != null ? ErrorDescriptionFactory.Create(error, correlationId) : null;
			LogMessage logMessage = new LogMessage()
			{
				Time = DateTime.UtcNow,
				Level = LogLevelConverter.ToString(level),
				Source = _source,
				Error = errorDescription,
				Message = message,
				CorrelationId = correlationId
			};

			lock (_lock)
			{
				_cache.Add(logMessage);
			}

			Update();
		}

		/// <summary>
		/// Saves log messages from the cache.
		/// </summary>
		/// <param name="messages">a list with log messages</param>
		protected abstract void Save(List<LogMessage> messages);

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters to be set.</param>
		public override void Configure(ConfigParams config)
		{
			base.Configure(config);

			_interval = config.GetAsIntegerWithDefault("options.interval", _interval);
			_maxCacheSize = config.GetAsIntegerWithDefault("options.max_cache_size", _maxCacheSize);
		}

		/// <summary>
		/// Clears (removes) all cached log messages.
		/// </summary>
		public void Clear()
		{
			lock (_lock)
			{
				_cache = new List<LogMessage>();
			}
			_updated = false;
		}

		/// <summary>
		/// Dumps (writes) the currently cached log messages.
		/// </summary>
		/// See <see cref="Write(LogLevel, string, Exception, string)"/>
		public void Dump()
		{
			if (_updated)
			{
				if (!_updated) return;

				List<LogMessage> messages;

				lock (_lock)
				{
					messages = _cache;
					_cache = new List<LogMessage>();
				}

				try
				{
					Save(messages);
				}
				catch (Exception ex)
				{
					lock (_lock)
					{
						// Put failed messages back to cache
						messages.AddRange(_cache);
						_cache = messages;

						// Truncate cache to max size
						while (_cache.Count > _maxCacheSize)
							_cache.RemoveAt(0);
					}

					throw ex;
				}
				finally
				{
					_updated = false;
					_lastDumpTime = DateTime.UtcNow.Ticks;
				}
			}
		}

		/// <summary>
		/// Makes message cache as updated and dumps it when timeout expires.
		/// </summary>
		/// See <see cref="Dump"/>
		protected void Update()
		{
			_updated = true;

			var now = DateTime.UtcNow.Ticks;

			if (now > _lastDumpTime + TimeSpan.FromMilliseconds(_interval).Ticks)
			{
				try
				{
					Dump();
				}
				catch (Exception)
				{
					// Todo: decide what to do
				}
			}
		}
	}
}
