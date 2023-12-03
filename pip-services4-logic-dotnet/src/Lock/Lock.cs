using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using System;
using System.Threading;

namespace PipServices4.Logic.Lock
{
	/// <summary>
	/// Abstract lock that implements default lock acquisition routine.
	/// 
	/// options:
	/// - retry_timeout:   timeout in milliseconds to retry lock acquisition. (Default: 100)
	/// </summary>
	/// See <see cref="ILock"/>
	public abstract class Lock : ILock, IReconfigurable
	{
		private int _retryTimeout = 100;

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters to be set.</param>
		public virtual void Configure(ConfigParams config)
		{
			_retryTimeout = config.GetAsIntegerWithDefault("options.retry_timeout", _retryTimeout);
		}

		/// <summary>
		/// Makes a single attempt to acquire a lock by its key.
		/// It returns immediately a positive or negative result.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		/// <param name="ttl">a lock timeout (time to live) in milliseconds.</param>
		/// <returns>a lock result</returns>
		public abstract bool TryAcquireLock(IContext context, string key, long ttl);

		/// <summary>
		/// Releases prevously acquired lock by its key.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		public abstract void ReleaseLock(IContext context, string key);

		/// <summary>
		/// Makes multiple attempts to acquire a lock by its key within give time interval.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="key">a unique lock key to acquire.</param>
		/// <param name="ttl">a lock timeout (time to live) in milliseconds.</param>
		/// <param name="timeout">a lock acquisition timeout.</param>
		public void AcquireLock(IContext context, string key, long ttl, long timeout)
		{
			var expireTime = DateTime.UtcNow.Ticks + TimeSpan.FromMilliseconds(timeout).Ticks;

			// Repeat until time expires
			do
			{
				// Try to get lock first
				if (TryAcquireLock(context, key, ttl))
					return;

				// Sleep 
				Thread.Sleep(_retryTimeout);

			} while (DateTime.UtcNow.Ticks < expireTime);

			// Throw exception
			throw new ConflictException(
                context != null ? ContextResolver.GetTraceId(context) : null,
				"LOCK_TIMEOUT",
				"Acquiring lock " + key + " failed on timeout"
			).WithDetails("key", key);
		}
	}
}
