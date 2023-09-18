using PipServices4.Components.Refer;
using System;
using System.Collections.Generic;

namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Aggregates all counters from component references under a single component.
	/// 
	/// It allows to capture metrics and conveniently send them to multiple destinations.
	/// 
	/// ### References ###
	/// 
	/// - *:counters:*:*:1.0         (optional) ICounters components to pass collected measurements
	/// </summary>
	/// <example>
	/// <code>
	/// class MyComponent: IReferenceable 
	/// {
	///     CompositeCounters _counters = new CompositeCounters();
	///     public void SetReferences(IReferences references)
	///     {
	///         this._counters.SetReferences(references);
	///         ...
	///     }
	///     
	///     public void MyMethod()
	///     {
	///         this._counters.Increment("mycomponent.mymethod.calls");
	///         var timing = this._counters.BeginTiming("mycomponent.mymethod.exec_time");
	///         try
	///         {
	///             ...
	///         }
	///         finally
	///         {
	///             timing.EndTiming();
	///         }
	///     }
	/// }
	/// </code>
	/// </example>
	/// See <see cref="ICounters"/>
	public class CompositeCounters : ICounters, ICounterTimingCallback, IReferenceable
	{
		protected readonly List<ICounters> _counters = new List<ICounters>();

		/// <summary>
		/// Creates a new instance of the counters.
		/// </summary>
		/// <param name="references">references to locate the component dependencies.</param>
		public CompositeCounters(IReferences references = null)
		{
			if (references != null) SetReferences(references);
		}

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies.</param>
		public virtual void SetReferences(IReferences references)
		{
			var counters = references.GetOptional<ICounters>(new Descriptor(null, "counters", null, null, null));
			foreach (var counter in counters)
			{
				if (counter != this)
				{
					_counters.Add(counter);
				}
			}
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
			foreach (var counter in _counters)
			{
				var callback = counter as ICounterTimingCallback;
				if (callback != null)
					callback.EndTiming(name, elapsed);
			}
		}

		/// <summary>
		/// Calculates min/average/max statistics based on the current and previous values.
		/// </summary>
		/// <param name="name">a counter name of Statistics type</param>
		/// <param name="value">a value to update statistics</param>
		public void Stats(string name, float value)
		{
			foreach (var counter in _counters)
				counter.Stats(name, value);
		}

		/// <summary>
		/// Records the last calculated measurement value.
		/// Usually this method is used by metrics calculated externally.
		/// </summary>
		/// <param name="name">a counter name of Last type.</param>
		/// <param name="value">a last value to record.</param>
		public void Last(string name, float value)
		{
			foreach (var counter in _counters)
				counter.Last(name, value);
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
			foreach (var counter in _counters)
				counter.Timestamp(name, value);
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
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			foreach (var counter in _counters)
				counter.Increment(name, value);
		}
	}
}
