using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using System;
using System.Collections.Generic;

namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Abstract implementation of performance counters that measures and stores counters in memory.
	/// Child classes implement saving of the counters into various destinations.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// options:
	/// - interval:        interval in milliseconds to save current counters measurements(default: 5 mins)
	/// - reset_timeout:   timeout in milliseconds to reset the counters. 0 disables the reset(default: 0)
	/// </summary>
	public abstract class CachedCounters : ICounters, IReconfigurable, ICounterTimingCallback
	{
		protected readonly IDictionary<string, Counter> _cache = new Dictionary<string, Counter>();
		protected bool _updated;
		protected long _lastDumpTime = DateTime.UtcNow.Ticks;
		protected long _lastResetTime = DateTime.UtcNow.Ticks;
		protected readonly object _lock = new object();
		protected long _interval = 300000;
		protected long _resetTimeout = 0;

		/// <summary>
		/// Saves the current counters measurements.
		/// </summary>
		/// <param name="counters">current counters measurements to be saves.</param>
		protected abstract void Save(IEnumerable<Counter> counters);

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters to be set.</param>
		public virtual void Configure(ConfigParams config)
		{
			_interval = config.GetAsLongWithDefault("interval", _interval);
			_resetTimeout = config.GetAsLongWithDefault("reset_timeout", _resetTimeout);
		}

		/// <summary>
		/// Clears (resets) a counter specified by its name.
		/// </summary>
		/// <param name="name">a counter name to clear.</param>
		public void Clear(string name)
		{
			lock (_lock)
			{
				_cache.Remove(name);
			}
		}

		/// <summary>
		/// Clears (resets) all counters.
		/// </summary>
		public void ClearAll()
		{
			lock (_lock)
			{
				_cache.Clear();
				_updated = false;
			}
		}

		/// <summary>
		/// Dumps (saves) the current values of counters.
		/// </summary>
		/// See <see cref="Save(IEnumerable{Counter})"/>
		public void Dump()
		{
			if (!_updated) return;

			var counters = GetAll();

			Save(counters);

			lock (_lock)
			{
				_updated = false;
				_lastDumpTime = DateTime.UtcNow.Ticks;
			}
		}

		/// <summary>
		/// Makes counter measurements as updated and dumps them when timeout expires.
		/// </summary>
		/// See <see cref="Dump"/>
		protected void Update()
		{
			_updated = true;
			if (DateTime.UtcNow.Ticks > _lastDumpTime + TimeSpan.FromMilliseconds(_interval).Ticks)
			{
				try
				{
					Dump();
				}
				catch (InvocationException)
				{
					// Todo: decide what to do
				}
			}
		}

		private void ResetIfNeeded()
		{
			if (_resetTimeout == 0) return;

			var now = DateTime.UtcNow.Ticks;
			if (now - _lastResetTime > TimeSpan.FromMilliseconds(_resetTimeout).Ticks)
			{
				_cache.Clear();
				_updated = false;
				_lastResetTime = now;
			}
		}

		/// <summary>
		/// Gets all captured counters.
		/// </summary>
		/// <returns>a list with counters.</returns>
		public IEnumerable<Counter> GetAll()
		{
			lock (_lock)
			{
				ResetIfNeeded();
				return _cache.Values;
			}
		}

		/// <summary>
		/// Gets a counter specified by its name. It counter does not exist or its type
		/// doesn't match the specified type it creates a new one.
		/// </summary>
		/// <param name="name">a counter name to retrieve.</param>
		/// <param name="type">a counter type.</param>
		/// <returns>an existing or newly created counter of the specified type.</returns>
		public Counter Get(string name, CounterType type)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			lock (_lock)
			{
				Counter counter;

				ResetIfNeeded();
				_cache.TryGetValue(name, out counter);

				if (counter == null || counter.Type != type)
				{
					counter = new Counter(name, type);
					_cache[name] = counter;
				}

				return counter;
			}
		}

		private void CalculateStats(Counter counter, double value)
		{
			if (counter == null)
				throw new ArgumentNullException(nameof(counter));

			counter.Last = value;
			counter.Count = counter.Count + 1 ?? 1;
			counter.Max = counter.Max.HasValue ? Math.Max(counter.Max.Value, value) : value;
			counter.Min = counter.Min.HasValue ? Math.Min(counter.Min.Value, value) : value;
			counter.Average = (counter.Average.HasValue && counter.Count > 1
				? (counter.Average * (counter.Count - 1) + value) / counter.Count : value);
		}

		/// <summary>
		/// Begins measurement of execution time interval. It returns Timing object which
		/// has to be called at <see cref="CounterTiming.EndTiming"/> to end the measurement and
		/// update the counter.
		/// </summary>
		/// <param name="name">a counter name of Interval type.</param>
		/// <returns>a Timing callback object to end timing.</returns>
		public CounterTiming BeginTiming(string name)
		{
			return new CounterTiming(name, this);
		}

		/// <summary>
		/// Ends measurement of execution elapsed time and updates specified counter.
		/// </summary>
		/// <param name="name">a counter name</param>
		/// <param name="elapsed">execution elapsed time in milliseconds to update the counter.</param>
		public void EndTiming(string name, double elapsed)
		{
			var counter = Get(name, CounterType.Interval);
			CalculateStats(counter, elapsed);
			Update();
		}

		/// <summary>
		/// Calculates min/average/max statistics based on the current and previous values.
		/// </summary>
		/// <param name="name">a counter name of Statistics type</param>
		/// <param name="value">a value to update statistics</param>
		public void Stats(string name, float value)
		{
			var counter = Get(name, CounterType.Statistics);
			CalculateStats(counter, value);
			Update();
		}

		/// <summary>
		/// Records the last calculated measurement value.
		/// Usually this method is used by metrics calculated externally.
		/// </summary>
		/// <param name="name">a counter name of Last type.</param>
		/// <param name="value">a last value to record.</param>
		public void Last(string name, float value)
		{
			var counter = Get(name, CounterType.LastValue);
			counter.Last = value;
			Update();
		}

		/// <summary>
		/// Records the current time as a timestamp.
		/// </summary>
		/// <param name="name">a counter name of Timestamp type.</param>
		public void TimestampNow(string name)
		{
			Timestamp(name, DateTime.UtcNow);
		}

		/// <summary>
		/// Records the given timestamp.
		/// </summary>
		/// <param name="name">a counter name of Timestamp type.</param>
		/// <param name="value">a timestamp to record.</param>
		public void Timestamp(string name, DateTime value)
		{
			var counter = Get(name, CounterType.Timestamp);
			counter.Time = value;
			Update();
		}

		/// <summary>
		/// Increments counter by 1.
		/// </summary>
		/// <param name="name">a counter name of Increment type.</param>
		public void IncrementOne(string name)
		{
			Increment(name, 1);
		}

		/// <summary>
		/// Increments counter by given value.
		/// </summary>
		/// <param name="name">a counter name of Increment type.</param>
		/// <param name="value">a value to add to the counter.</param>
		public void Increment(string name, int value)
		{
			var counter = Get(name, CounterType.Increment);
			counter.Count = counter.Count.HasValue ? counter.Count + value : value;
			Update();
		}
	}
}
