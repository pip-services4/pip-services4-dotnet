using Enyim.Caching;
using Enyim.Caching.Configuration;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Config.Auth;
using PipServices4.Config.Connect;
using System;
using System.Threading.Tasks;

namespace PipServices4.Memcached.Lock
{
    /// <summary>
    /// Distributed lock that implemented based on Memcaches caching service.
    /// 
    /// The current implementation does not support authentication.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// connection(s):
    /// - discovery_key:         (optional) a key to retrieve the connection from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
    /// - host:                  host name or IP address
    /// - port:                  port number
    /// - uri:                   resource URI or connection string with all parameters in it
    /// 
    /// options:
    /// - retry_timeout:         timeout in milliseconds to retry lock acquisition. (Default: 100)
    /// - max_size:              maximum number of values stored in this cache (default: 1000)        
    /// - max_key_size:          maximum key length (default: 250)
    /// - max_expiration:        maximum expiration duration in milliseconds (default: 2592000)
    /// - max_value:             maximum value length (default: 1048576)
    /// - pool_size:             pool size (default: 5)
    /// - reconnect:             reconnection timeout in milliseconds (default: 10 sec)
    /// - retries:               number of retries (default: 3)
    /// - timeout:               default caching timeout in milliseconds (default: 1 minute)
    /// - failures:              number of failures before stop retrying (default: 5)
    /// - retry:                 retry timeout in milliseconds (default: 30 sec)
    /// - idle:                  idle timeout before disconnect in milliseconds (default: 5 sec)
    /// 
    /// ### References ###
    /// 
    /// - *:discovery:*:*:1.0        (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    /// </summary>
    /// <example>
    /// <code>
    /// var lock = new MemcachedLock();
    /// lock.Configure(ConfigParams.FromTuples(
    /// "host", "localhost",
    /// "port", 11211));
    /// lock.Open("123");
    /// lock.TryAcquireLock("123", "key1", 0);
    /// lock.ReleaseLock("123", "key1");
    /// </code>
    /// </example>
    public class MemcachedLock : PipServices4.Logic.Lock.Lock,
        IConfigurable, IReferenceable, IOpenable
    {
        private ConnectionResolver _connectionResolver = new ConnectionResolver();
        private CredentialResolver _credentialResolver = new CredentialResolver();
        private MemcachedClient _client = null;

        public MemcachedLock()
        {
        }

        // Todo: Make the method virtual
        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public new void Configure(ConfigParams config)
        {
            base.Configure(config);
            _connectionResolver.Configure(config);
            _credentialResolver.Configure(config);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public void SetReferences(IReferences references)
        {
            _connectionResolver.SetReferences(references);
            _credentialResolver.SetReferences(references);
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public bool IsOpen()
        {
            return _client != null;
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async Task OpenAsync(IContext context)
        {
            var connections = await _connectionResolver.ResolveAllAsync(context);
            if (connections.Count == 0)
                throw new ConfigException(context != null ? ContextResolver.GetTraceId(context) : null, "NO_CONNECTION", "Connection is not configured");

            var options = new MemcachedClientConfiguration();

            foreach (var connection in connections)
            {
                var uri = connection.Uri;

                if (!string.IsNullOrEmpty(uri))
                {
                    options.AddServer(uri, 11211);
                }
                else
                {
                    var host = connection.Host ?? "localhost";
                    var port = connection.Port != 0 ? connection.Port : 11211;

                    options.AddServer(host, port);
                }
            }

            _client = new MemcachedClient(null, options);
        }

        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async Task CloseAsync(IContext context)
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }

            await Task.Delay(0);
        }

        private void CheckOpened(IContext context)
        {
            if (!IsOpen())
                throw new InvalidStateException(context != null ? ContextResolver.GetTraceId(context) : null, "NOT_OPENED", "Connection is not opened");
        }

        /// <summary>
        /// Makes a single attempt to acquire a lock by its key.
        /// It returns immediately a positive or negative result.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="key">a unique lock key to acquire.</param>
        /// <param name="ttl">a lock timeout (time to live) in milliseconds.</param>
        /// <returns>a lock result</returns>
        public override bool TryAcquireLock(IContext context, string key, long ttl)
        {
            CheckOpened(context);

            return _client.StoreAsync(Enyim.Caching.Memcached.StoreMode.Add, key, "lock", TimeSpan.FromMilliseconds(ttl)).Result;
        }

        /// <summary>
        /// Releases prevously acquired lock by its key.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="key">a unique lock key to release.</param>
        public override void ReleaseLock(IContext context, string key)
        {
            CheckOpened(context);

            _client.RemoveAsync(key).Wait();
        }
    }
}
