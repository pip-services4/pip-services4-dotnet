using System;

namespace PipServices4.Logic.State
{
	/// <summary>
	/// Data object to store state values with their keys used by MemoryStateEntry
	/// </summary>
	public class StateEntry
	{
		private readonly string _key;
		private object _value;
		private long _lastUpdateTime;

		/// <summary>
		/// Gets the key to locate the state value.
		/// </summary>
		public string Key { get { return _key; } }

		/// <summary>
		/// Gets the last update time.
		/// </summary>
		public long LastUpdateTime { get { return _lastUpdateTime; } }

		/// <summary>
		/// Get or set state value.
		/// </summary>
		public object Value
		{
			get { return _value; }
			set { _value = value; _lastUpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(); }
		}

		/// <summary>
		/// Creates a new instance of the state entry and assigns its values.
		/// </summary>
		/// <param name="key">A unique key to locate the value.</param>
		/// <param name="value">A value to be stored.</param>
		public StateEntry(string key, object value)
		{
			_key = key;
			_value = value;
			_lastUpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		}
	}
}
