using System;
using System.Collections.Generic;

namespace PipServices4.Logic.Lock
{
	/// <summary>
	/// Lock that is used to synchronize execution within one process using shared memory.
	/// 
	/// Remember: This implementation is not suitable for synchronization of distributed processes.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// options:
	/// - retry_timeout:   timeout in milliseconds to retry lock acquisition. (Default: 100)
	/// </summary>
	/// <example>
	/// <code>
	/// var lock = new MemoryLock();
	/// lock.TryAcquireLock("123", "key1", 0);
	/// try {
	/// // Processing...
	/// }
	/// finally  {
	/// lock.ReleaseLock("123", "key1");
	/// // Continue...
	/// });
	/// </code>
	/// </example>
	/// See <see cref="ILock"/>, <see cref="Lock"/>
	public class MemoryLock : Lock
	{
		private Dictionary<string, long> _locks = new Dictionary<string, long>();

		/// <summary>
		/// Makes a single attempt to acquire a lock by its key.
		/// It returns immediately a positive or negative result.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		/// <param name="ttl">a lock timeout (time to live) in milliseconds.</param>
		/// <returns>a lock result</returns>
		public override bool TryAcquireLock(string correlationId, string key, long ttl)
		{
			var now = DateTime.UtcNow.Ticks;

			lock (_locks)
			{
				long expireTime;

				if (_locks.TryGetValue(key, out expireTime))
				{
					if (expireTime > now) return false;
				}

				expireTime = now + TimeSpan.FromMilliseconds(ttl).Ticks;
				_locks[key] = expireTime;
			}

			return true;
		}

		/// <summary>
		/// Releases prevously acquired lock by its key.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		public override void ReleaseLock(string correlationId, string key)
		{
			lock (_locks)
			{
				_locks.Remove(key);
			}
		}
	}
}
