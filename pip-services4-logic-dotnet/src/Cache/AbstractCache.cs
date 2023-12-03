using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using System.Threading.Tasks;

namespace PipServices4.Logic.Cache
{
	/// <summary>
	/// Abstract cache class to provide common cache functionality
	/// </summary>
	/// See <see cref="ICache" />, 
	/// <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_config_1_1_i_configurable.html">IConfigurable</a>,
	/// <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_refer_1_1_i_referenceable.html">IReferenceable</a>,
	/// <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_run_1_1_i_openable.html">IOpenable</a>
	public abstract class AbstractCache : ICache, IConfigurable, IReferenceable, IOpenable
	{
		private readonly long DefaultTimeout = 60000; // 1 min

		/// <summary>
		/// Gets or sets the timeout.
		/// </summary>
		public long Timeout { get; set; }

		/// <summary>
		/// Sets the components configuration.
		/// </summary>
		/// <param name="config">Configuration parameters.</param>
		public virtual void Configure(ConfigParams config)
		{
			Timeout = config.GetAsLongWithDefault("timeout", DefaultTimeout);
		}

		/// <summary>
		/// Sets the references.
		/// </summary>
		/// <param name="references">The references.</param>
		public virtual void SetReferences(IReferences references)
		{
		}

		/// <summary>
		/// Closes component, disconnects it from services, disposes resources
		/// </summary>
		/// <param name="context">a unique transaction id to trace calls across components</param>
		public virtual Task CloseAsync(IContext context)
		{
			return Task.Delay(0);
		}

		/// <summary>
		/// Checks if component is opened
		/// </summary>
		public virtual bool IsOpen()
		{
			return true;
		}

		/// <summary>
		/// Opens component, establishes connections to services
		/// </summary>
		/// <param name="context">a unique transaction id to trace calls across components</param>
		public virtual Task OpenAsync(IContext context)
		{
			return Task.Delay(0);
		}

		/// <summary>
		/// Removes an object from cache.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="key">Unique key identifying the object.</param>
		public abstract Task RemoveAsync(IContext context, string key);

		/// <summary>
		/// Retrieves a value from cache by unique key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
		/// <param name="key">Unique key identifying a data object.</param>
		/// <returns>
		/// Cached value or null if the value is not found.
		/// </returns>
		public abstract Task<T> RetrieveAsync<T>(IContext context, string key);

		/// <summary>
		/// Stores an object identified by a unique key in cache.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
		/// <param name="key">Unique key identifying a data object.</param>
		/// <param name="value">The data object to store.</param>
		/// <param name="timeout">Time to live for the object in milliseconds.</param>
		/// <returns>
		/// Cached value or null if the value is not stored.
		/// </returns>
		public abstract Task<T> StoreAsync<T>(IContext context, string key, T value, long timeout);
	}
}