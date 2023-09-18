using System.Threading.Tasks;

namespace PipServices4.Logic.Cache
{
	/// <summary>
	/// Dummy cache implementation that doesn't do anything.
	/// 
	/// It can be used in testing or in situations when cache is required
	/// but shall be disabled.
	/// </summary>
	/// <see cref="ICache"/>
	public class NullCache : AbstractCache
	{
		/// <summary>
		/// Retrieves cached value from the cache using its key. If value is missing in
		/// the cache or expired it returns null.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		/// <returns>a cached value or null if value wasn't found or timeout expired.</returns>
		public async override Task<T> RetrieveAsync<T>(string correlationId, string key)
		{
			return await Task.FromResult(default(T));
		}

		/// <summary>
		/// Stores value in the cache with expiration time.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		/// <param name="value">a value to store.</param>
		/// <param name="timeout">expiration timeout in milliseconds.</param>
		/// <returns>a cached value stored in the cache.</returns>
		public async override Task<T> StoreAsync<T>(string correlationId, string key, T value, long timeout)
		{
			return await Task.FromResult(value);
		}

		/// <summary>
		/// Removes a value from the cache by its key.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		public async override Task RemoveAsync(string correlationId, string key)
		{
			await Task.Delay(0);
		}
	}
}