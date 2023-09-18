using System;

namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Dummy implementation of performance counters that doesn't do anything.
	/// 
	/// It can be used in testing or in situations when counters is required but shall be disabled.
	/// </summary>
	/// See <see cref="ICounters"/>
	public sealed class NullCounters : ICounters
	{
		/// <summary>
		/// Begins measurement of execution time interval. It returns Timing object which
		/// has to be called at <see cref="CounterTiming.EndTiming"/> to end the measurement and
		/// update the counter.
		/// </summary>
		/// <param name="name">a counter name of Interval type.</param>
		/// <returns>a Timing callback object to end timing.</returns>
		public CounterTiming BeginTiming(string name)
		{
			return new CounterTiming();
		}

		/// <summary>
		/// Calculates min/average/max statistics based on the current and previous values.
		/// </summary>
		/// <param name="name">a counter name of Statistics type</param>
		/// <param name="value">a value to update statistics</param>
		public void Stats(string name, float value)
		{
		}

		/// <summary>
		/// Records the last calculated measurement value.
		/// Usually this method is used by metrics calculated externally.
		/// </summary>
		/// <param name="name">a counter name of Last type.</param>
		/// <param name="value">a last value to record.</param>
		public void Last(string name, float value)
		{
		}

		/// <summary>
		/// Records the current time as a timestamp.
		/// </summary>
		/// <param name="name">a counter name of Timestamp type.</param>
		public void TimestampNow(string name)
		{
		}

		/// <summary>
		/// Records the given timestamp.
		/// </summary>
		/// <param name="name">a counter name of Timestamp type.</param>
		/// <param name="value">a timestamp to record.</param>
		public void Timestamp(string name, DateTime value)
		{
		}

		/// <summary>
		/// Increments counter by 1.
		/// </summary>
		/// <param name="name">a counter name of Increment type.</param>
		public void IncrementOne(string name)
		{
		}

		/// <summary>
		/// Increments counter by given value.
		/// </summary>
		/// <param name="name">a counter name of Increment type.</param>
		/// <param name="value">a value to add to the counter.</param>
		public void Increment(string name, int value)
		{
		}
	}
}
