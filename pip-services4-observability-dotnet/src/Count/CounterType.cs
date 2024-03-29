namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Types of counters that measure different types of metrics
	/// </summary>
	public enum CounterType : int
	{
		/** Counters that measure execution time intervals */
		Interval = 0,
		/** Counters that keeps the latest measured value */
		LastValue = 1,
		/** Counters that measure min/average/max statistics */
		Statistics = 2,
		/** Counter that record timestamps */
		Timestamp = 3,
		/** Counter that increment counters */
		Increment = 4
	}
}
