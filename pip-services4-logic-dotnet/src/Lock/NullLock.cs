using PipServices4.Components.Context;

namespace PipServices4.Logic.Lock
{
	/// <summary>
	/// Dummy lock implementation that doesn't do anything.
	/// 
	/// It can be used in testing or in situations when lock is required
	/// but shall be disabled.
	/// </summary>
	/// See <see cref="ILock"/>
	public class NullLock : ILock
	{
		/// <summary>
		/// Makes multiple attempts to acquire a lock by its key within give time interval.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		/// <param name="ttl">a lock timeout (time to live) in milliseconds.</param>
		/// <param name="timeout">a lock acquisition timeout.</param>
		public void AcquireLock(IContext context, string key, long ttl, long timeout)
		{ }

		/// <summary>
		/// Releases prevously acquired lock by its key.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		public void ReleaseLock(IContext context, string key)
		{ }

		/// <summary>
		/// Makes a single attempt to acquire a lock by its key.
		/// It returns immediately a positive or negative result.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		/// <param name="ttl">a lock timeout (time to live) in milliseconds.</param>
		/// <returns>a lock result</returns>
		public bool TryAcquireLock(IContext context, string key, long ttl)
		{
			return true;
		}
	}
}
