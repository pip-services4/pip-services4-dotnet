using PipServices4.Commons.Convert;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Config.Auth;
using PipServices4.Config.Connect;
using PipServices4.Logic.Cache;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace PipServices4.Redis.Cache
{
    /// <summary>
    /// Distributed cache that stores values in Redis in-memory database.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// connection(s):
    /// - discovery_key:         (optional) a key to retrieve the connection from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
    /// - host:                  host name or IP address
    /// - port:                  port number
    /// - uri:                   resource URI or connection string with all parameters in it
    ///
    /// credential(s):
    /// - store_key:             key to retrieve parameters from credential store
    /// - username:              user name(currently is not used)
    /// - password:              user password
    /// 
    /// options:
    /// - retries:               number of retries(default: 3)
    /// - timeout:               default caching timeout in milliseconds(default: 1 minute)
    /// - max_size:              maximum number of values stored in this cache (default: 1000)        
    /// 
    /// ### References ###
    /// 
    /// - *:discovery:*:*:1.0        (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    /// - *:credential-store:*:*:1.0 (optional) Credential stores to resolve credential
    /// </summary>
    /// <example>
    /// <code>
    /// var cache = new RedisCache();
    /// cache.configure(ConfigParams.FromTuples(
    /// "host", "localhost",
    /// "port", 6379));
    /// cache.open("123");
    /// 
    /// cache.StoreAsync("123", "key1", "ABC");
    /// </code>
    /// </example>
    public class RedisCache: AbstractCache
    {
        private ConnectionResolver _connectionResolver = new ConnectionResolver();
        private CredentialResolver _credentialResolver = new CredentialResolver();

        private int _connectTimeout = 30000;
        private int _retryTimeout = 3000;
        private int _retries = 3;
        private ConnectionMultiplexer _client = null;
        private IDatabase _database = null;

        /// <summary>
        /// Creates a new instance of this cache.
        /// </summary>
        public RedisCache()
        {
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public override void Configure(ConfigParams config)
        {
            base.Configure(config);

            _connectionResolver.Configure(config);
            _credentialResolver.Configure(config);

            _connectTimeout = config.GetAsIntegerWithDefault("options.connect_timeout", _connectTimeout);
            _retryTimeout = config.GetAsIntegerWithDefault("options.timeout", _retryTimeout);
            _retries = config.GetAsIntegerWithDefault("options.retries", _retries);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public override void SetReferences(IReferences references)
        {
            _connectionResolver.SetReferences(references);
            _credentialResolver.SetReferences(references);
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public override bool IsOpen()
        {
            return _client != null;
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        public override async Task OpenAsync(IContext context)
        {
            var connection = await _connectionResolver.ResolveAsync(context);
            if (connection == null)
                throw new ConfigException(context != null ? ContextResolver.GetTraceId(context) : null, "NO_CONNECTION", "Connection is not configured");

            ConfigurationOptions options;
            var uri = connection.Uri;
            if (!string.IsNullOrEmpty(uri))
            {
                options = ConfigurationOptions.Parse(uri);
            }
            else
            {                
                var host = connection.Host ?? "localhost";
                var port = connection.Port != 0 ? connection.Port : 6379;
                options = new ConfigurationOptions();
                options.EndPoints.Add(host, port);
            }

            var credential = await _credentialResolver.LookupAsync(context);
            if (credential != null && !string.IsNullOrEmpty(credential.Password))
            {
                options.Password = credential.Password;
            }

            options.ConnectTimeout = _connectTimeout;
            options.ResponseTimeout = _retryTimeout;
            options.ConnectRetry = _retries;

            _client = await ConnectionMultiplexer.ConnectAsync(options);
            _database = _client.GetDatabase();
        }

        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        public override async Task CloseAsync(IContext context)
        {
            if (_client != null)
            {
                await _client.CloseAsync();
                _client = null;
                _database = null;
            }
        }

        private void CheckOpened(IContext context)
        {
            if (!IsOpen())
                throw new InvalidStateException(context != null ? ContextResolver.GetTraceId(context) : null, "NOT_OPENED", "Connection is not opened");
        }

        /// <summary>
        /// Retrieves cached value from the cache using its key.
        /// If value is missing in the cache or expired it returns null.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a unique value key.</param>
        /// <returns>a cached value.</returns>
        public override async Task<T> RetrieveAsync<T>(IContext context, string key)
        {
            CheckOpened(context);

            var json = await _database.StringGetAsync(key);
            var value = JsonConverter.FromJson<T>(json);

            return value;
        }

        /// <summary>
        /// Stores value in the cache with expiration time.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a unique value key.</param>
        /// <param name="value">a value to store.</param>
        /// <param name="timeout">expiration timeout in milliseconds.</param>
        /// <returns>a stored value.</returns>
        public override async Task<T> StoreAsync<T>(IContext context, string key, T value, long timeout)
        {
            CheckOpened(context);

            timeout = timeout > 0 ? timeout : Timeout;

            var json = JsonConverter.ToJson(value);
            var result = await _database.StringSetAsync(key, json, TimeSpan.FromMilliseconds(timeout));

            return result ? value : default(T);
        }

        /// <summary>
        /// Removes a value from the cache by its key.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a unique value key.</param>
        public override async Task RemoveAsync(IContext context, string key)
        {
            CheckOpened(context);

            await _database.KeyDeleteAsync(key);
        }
    }
}
