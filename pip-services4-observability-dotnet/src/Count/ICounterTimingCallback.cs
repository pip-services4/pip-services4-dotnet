namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Interface for a callback to end measurement of execution elapsed time.
	/// </summary>
	/// See <see cref="CounterTiming"/>
	public interface ICounterTimingCallback
	{
		/// <summary>
		/// Ends measurement of execution elapsed time and updates specified counter. 
		/// </summary>
		/// <param name="name">a counter name</param>
		/// <param name="elapsed">execution elapsed time in milliseconds to update the counter.</param>
		void EndTiming(string name, double elapsed);
	}
}
