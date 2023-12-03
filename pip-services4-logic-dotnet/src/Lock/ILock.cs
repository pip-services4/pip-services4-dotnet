using PipServices4.Components.Context;

namespace PipServices4.Logic.Lock
{
	/// <summary>
	/// Interface for locks to synchronize work or parallel processes and to prevent collisions.
	/// 
	/// The lock allows to manage multiple locks identified by unique keys.
	/// </summary>
	public interface ILock
	{
		/// <summary>
		/// Makes a single attempt to acquire a lock by its key.
		/// It returns immediately a positive or negative result.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		/// <param name="ttl">a lock timeout (time to live) in milliseconds.</param>
		/// <returns>a lock result</returns>
		bool TryAcquireLock(IContext context, string key, long ttl);

		/// <summary>
		/// Makes multiple attempts to acquire a lock by its key within give time interval.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		/// <param name="ttl">a lock timeout (time to live) in milliseconds.</param>
		/// <param name="timeout">a lock acquisition timeout.</param>
		void AcquireLock(IContext context, string key, long ttl, long timeout);

		/// <summary>
		/// Releases prevously acquired lock by its key.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		void ReleaseLock(IContext context, string key);
	}
}
