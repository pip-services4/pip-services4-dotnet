using System;

namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Interface for performance counters that measure execution metrics.
	/// 
	/// The performance counters measure how code is performing: 
	/// how fast or slow, how many transactions performed, how many objects
	/// are stored, what was the latest transaction time and so on.
	/// 
	/// They are critical to monitor and improve performance, scalability
	/// and reliability of code in production.
	/// </summary>
	public interface ICounters
	{
		/// <summary>
		/// Begins measurement of execution time interval. It returns Timing object which
		/// has to be called at <see cref="CounterTiming.EndTiming"/> to end the measurement and
		/// update the counter.
		/// </summary>
		/// <param name="name">a counter name of Interval type.</param>
		/// <returns>a Timing callback object to end timing.</returns>
		CounterTiming BeginTiming(string name);

		/// <summary>
		/// Calculates min/average/max statistics based on the current and previous values.
		/// </summary>
		/// <param name="name">a counter name of Statistics type</param>
		/// <param name="value">a value to update statistics</param>
		void Stats(string name, float value);

		/// <summary>
		/// Records the last calculated measurement value.
		/// Usually this method is used by metrics calculated externally.
		/// </summary>
		/// <param name="name">a counter name of Last type.</param>
		/// <param name="value">a last value to record.</param>
		void Last(string name, float value);

		/// <summary>
		/// Records the current time as a timestamp.
		/// </summary>
		/// <param name="name">a counter name of Timestamp type.</param>
		void TimestampNow(string name);

		/// <summary>
		/// Records the given timestamp.
		/// </summary>
		/// <param name="name">a counter name of Timestamp type.</param>
		/// <param name="value">a timestamp to record.</param>
		void Timestamp(string name, DateTime value);

		/// <summary>
		/// Increments counter by 1.
		/// </summary>
		/// <param name="name">a counter name of Increment type.</param>
		void IncrementOne(string name);

		/// <summary>
		/// Increments counter by given value.
		/// </summary>
		/// <param name="name">a counter name of Increment type.</param>
		/// <param name="value">a value to add to the counter.</param>
		void Increment(string name, int value);
	}
}
