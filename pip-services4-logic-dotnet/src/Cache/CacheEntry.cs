using System;

namespace PipServices4.Logic.Cache
{
	/// <summary>
	/// Data object to store cached values with their keys used by MemoryCache
	/// </summary>
	public class CacheEntry
	{
		/// <summary>
		/// Creates a new instance of the cache entry and assigns its values.
		/// </summary>
		/// <param name="key">a unique key to locate the value.</param>
		/// <param name="value">a value to be stored.</param>
		/// <param name="timeout">expiration timeout in milliseconds.</param>
		public CacheEntry(string key, object value, long timeout)
		{
			Key = key;
			Value = value;
			Expiration = DateTime.UtcNow.Ticks + TimeSpan.FromMilliseconds(timeout).Ticks;
		}

		/// <summary>
		/// Gets the key to locate the cached value.
		/// </summary>
		public string Key { get; }

		/// <summary>
		/// Gets the cached value.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		/// Gets the expiration timeout.
		/// </summary>
		public long Expiration { get; private set; }

		/// <summary>
		/// Sets a new value and extends its expiration.
		/// </summary>
		/// <param name="value">a new cached value.</param>
		/// <param name="timeout">a expiration timeout in milliseconds.</param>
		public void SetValue(object value, long timeout)
		{
			Value = value;
			Expiration = DateTime.UtcNow.Ticks + TimeSpan.FromMilliseconds(timeout).Ticks;
		}

		/// <summary>
		/// Checks if this value already expired.
		/// </summary>
		/// <returns>true if the value already expires and false otherwise.</returns>
		public bool IsExpired()
		{
			return Expiration < DateTime.UtcNow.Ticks;
		}
	}
}