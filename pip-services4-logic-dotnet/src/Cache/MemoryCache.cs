using PipServices4.Components.Config;
using PipServices4.Components.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Logic.Cache
{
	/// <summary>
	/// Cache that stores values in the process memory.
	/// 
	/// Remember: This implementation is not suitable for synchronization of distributed processes.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// options:
	/// - timeout:               default caching timeout in milliseconds (default: 1 minute)
	/// - max_size:              maximum number of values stored in this cache (default: 1000)   
	/// </summary>
	/// <example>
	/// <code>
	/// var cache = new MemoryCache();
	/// ...
	/// cache.StoreAsync("123", "key1", "ABC", 0);
	/// ...
	/// </code>
	/// </example>
	/// See <see cref="ICache"/>
	public class MemoryCache : AbstractCache
	{
		private readonly long DefaultTimeout = 60000;
		private const long DefaultMaxSize = 1000;

		private readonly Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();
		private readonly object _lock = new object();

		/// <summary>
		/// Creates instance of local in-memory cache component
		/// </summary>
		public MemoryCache()
			: this(null)
		{
		}

		/// <summary>
		/// Creates instance of local in-memory cache component
		/// </summary>
		/// <param name="config">configuration parameters</param>
		public MemoryCache(ConfigParams config)
		{
			Timeout = DefaultTimeout;
			MaxSize = DefaultMaxSize;

			if (config != null)
			{
				Configure(config);
			}
		}

		public long MaxSize { get; set; }

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters.</param>
		public override void Configure(ConfigParams config)
		{
			Timeout = config.GetAsLongWithDefault("timeout", Timeout);
			MaxSize = config.GetAsLongWithDefault("max_size", MaxSize);
		}

		/// <summary>
		/// Clears component state.
		/// </summary>
		private void Cleanup()
		{
			CacheEntry oldest = null;
			var keysToRemove = new List<string>();

			lock (_lock)
			{
				foreach (var entry in _cache)
				{
					if (entry.Value.IsExpired())
					{
						keysToRemove.Add(entry.Key);
					}
					if (oldest == null || oldest.Expiration > entry.Value.Expiration)
					{
						oldest = entry.Value;
					}
				}

				foreach (var key in keysToRemove)
				{
					_cache.Remove(key);
				}

				if (_cache.Count > MaxSize && oldest != null)
				{
					_cache.Remove(oldest.Key);
				}
			}
		}

		/// <summary>
		/// Retrieves cached value from the cache using its key. If value is missing in
		/// the cache or expired it returns null.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		/// <returns>a cached value or null if value wasn't found or timeout expired.</returns>
		public async override Task<T> RetrieveAsync<T>(IContext context, string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			await Task.Delay(0);

			lock (_lock)
			{
				CacheEntry entry;
				if (_cache.TryGetValue(key, out entry))
				{
					if (entry.IsExpired())
					{
						_cache.Remove(key);
						return default(T);
					}

					return (T)entry.Value;
				}

				return default(T);
			}
		}

		/// <summary>
		/// Stores value in the cache with expiration time.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		/// <param name="value">a value to store.</param>
		/// <param name="timeout">expiration timeout in milliseconds.</param>
		/// <returns>a cached value stored in the cache.</returns>
		public async override Task<T> StoreAsync<T>(IContext context, string key, T value, long timeout)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			lock (_lock)
			{
				CacheEntry entry;
				_cache.TryGetValue(key, out entry);
				timeout = timeout > 0 ? timeout : Timeout;

				//if (value == default(T))
				//{
				//    if (entry != null)
				//        _cache.Remove(key);
				//    return default(T);
				//}

				if (entry != null)
					entry.SetValue(value, timeout);
				else
					_cache[key] = new CacheEntry(key, value, timeout);

				// cleanup
				if (MaxSize > 0 && _cache.Count > MaxSize)
					Cleanup();
			}

			return await Task.FromResult(value);
		}

		/// <summary>
		/// Removes a value from the cache by its key.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		public async override Task RemoveAsync(IContext context, string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			lock (_lock)
			{
				_cache.Remove(key);
			}

			await Task.Delay(0);
		}

		/// <summary>
		/// Clears component state.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		public async Task ClearAsync(IContext context)
		{
			lock (_lock)
			{
				_cache.Clear();
			}

			await Task.Delay(0);
		}
	}
}