using Newtonsoft.Json;
using System;

namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Data object to store measurement for a performance counter.
	/// This object is used by CachedCounters to store counters.
	/// </summary>
	public sealed class Counter
	{
		/// <summary>
		/// Creates a instance of the data obejct
		/// </summary>
		public Counter() { }

		/// <summary>
		/// Creates a instance of the data obejct
		/// </summary>
		/// <param name="name">a counter name.</param>
		/// <param name="type">a counter type.</param>
		public Counter(string name, CounterType type)
		{
			Name = name;
			Type = type;
		}

		/** The counter unique name */
		[JsonProperty("name")]
		public string Name { get; set; }

		/** The counter type that defines measurement algorithm */
		[JsonProperty("type")]
		public CounterType? Type { get; set; }

		/** The last recorded value */
		[JsonProperty("last")]
		public double? Last { get; set; }

		/** The total count */
		[JsonProperty("count")]
		public int? Count { get; set; }

		/** The minimum value */
		[JsonProperty("min")]
		public double? Min { get; set; }

		/** The maximum value */
		[JsonProperty("max")]
		public double? Max { get; set; }

		/** The average value */
		[JsonProperty("average")]
		public double? Average { get; set; }

		/** The recorded timestamp */
		[JsonProperty("time")]
		public DateTime? Time { get; set; }
	}
}
