using System.Threading.Tasks;

namespace PipServices4.Logic.Cache
{
	/// <summary>
	/// Interface for caches that are used to cache values to improve performance.
	/// </summary>
	public interface ICache
	{
		/// <summary>
		/// Retrieves cached value from the cache using its key. If value is missing in 
		/// the cache or expired it returns null.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		/// <returns>a cached value or null if value wasn't found or timeout expired.</returns>
		Task<T> RetrieveAsync<T>(string correlationId, string key);

		/// <summary>
		/// Stores value in the cache with expiration time.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		/// <param name="value">a value to store.</param>
		/// <param name="timeout">expiration timeout in milliseconds.</param>
		/// <returns>a cached value stored in the cache.</returns>
		Task<T> StoreAsync<T>(string correlationId, string key, T value, long timeout);

		/// <summary>
		/// Removes a value from the cache by its key.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		Task RemoveAsync(string correlationId, string key);
	}
}
