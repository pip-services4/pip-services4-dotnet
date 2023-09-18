using System;

namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Callback object returned by <see cref="ICounters.BeginTiming(string)"/> to end timing
	/// of execution block and update the associated counter.
	/// </summary>
	/// <example>
	/// <code>
	/// var timing = counters.beginTiming("mymethod.exec_time");
	/// try {
	/// ...
	/// } finally {
	/// timing.endTiming();
	/// }
	/// </code>
	/// </example>
	public class CounterTiming : IDisposable
	{
		private readonly long _start;
		private readonly ICounterTimingCallback _callback;
		private readonly string _counter;

		/// <summary>
		/// Creates a new instance of the timing callback object.
		/// </summary>
		public CounterTiming() { }

		/// <summary>
		/// Creates a new instance of the timing callback object.
		/// </summary>
		/// <param name="counter">an associated counter name</param>
		/// <param name="callback">a callback that shall be called when endTiming is called.</param>
		public CounterTiming(string counter, ICounterTimingCallback callback)
		{
			_counter = counter;
			_callback = callback;
			_start = DateTime.UtcNow.Ticks;
		}

		/// <summary>
		/// Ends timing of an execution block, calculates elapsed time and updates the associated counter.
		/// </summary>
		public void EndTiming()
		{
			if (_callback == null)
			{
				return;
			}

			var elapsed = DateTime.UtcNow.Ticks - _start;

			_callback.EndTiming(_counter, TimeSpan.FromTicks(elapsed).TotalMilliseconds);
		}

		public void Dispose()
		{
			EndTiming();
		}
	}
}
