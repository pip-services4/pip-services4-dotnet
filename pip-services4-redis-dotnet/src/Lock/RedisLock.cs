using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Config.Auth;
using PipServices4.Config.Connect;
using PipServices4.Data.Keys;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace PipServices4.Redis.Lock
{
    /// <summary>
    /// Distributed lock that is implemented based on Redis in-memory database.
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
    /// - retry_timeout:         timeout in milliseconds to retry lock acquisition. (Default: 100)
    /// - retries:               number of retries(default: 3)
    /// 
    /// ### References ###
    /// 
    /// - *:discovery:*:*:1.0        (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    /// - *:credential-store:*:*:1.0 (optional) Credential stores to resolve credential
    /// </summary>
    /// <example>
    /// <code>
    /// var lock = new RedisLock();
    /// lock.configure(ConfigParams.FromTuples(
    /// "host", "localhost",
    /// "port", 6379 ));
    /// 
    /// lock.Open("123");
    /// 
    /// lock.TryAcquireLock("123", "key1", 0);
    /// lock.ReleaseLock("123", "key1");
    /// </code>
    /// </example>
    public class RedisLock : Logic.Lock.Lock,
        IConfigurable, IReferenceable, IOpenable
    {
        private ConnectionResolver _connectionResolver = new ConnectionResolver();
        private CredentialResolver _credentialResolver = new CredentialResolver();

        private string _lock = IdGenerator.NextLong();
        private int _connectTimeout = 30000;
        private int _retryTimeout = 3000;
        private int _retries = 3;
        private ConnectionMultiplexer _client = null;
        private IDatabase _database = null;

        public RedisLock()
        {
        }

        // Make the method virtual
        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public new void Configure(ConfigParams config)
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
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        public async Task OpenAsync(IContext context)
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
        public async Task CloseAsync(IContext context)
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
        /// Makes a single attempt to acquire a lock by its key.
        /// It returns immediately a positive or negative result.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a unique lock key to acquire.</param>
        /// <param name="ttl">a lock timeout (time to live) in milliseconds.</param>
        /// <returns>a lock result</returns>
        public override bool TryAcquireLock(IContext context, string key, long ttl)
        {
            CheckOpened(context);

            return _database.StringSetAsync(key, _lock, TimeSpan.FromMilliseconds(ttl), When.NotExists).Result;
        }

        /// <summary>
        /// Releases prevously acquired lock by its key.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a unique lock key to release.</param>
        public override void ReleaseLock(IContext context, string key)
        {
            CheckOpened(context);

            var transaction = _database.CreateTransaction();
            transaction.AddCondition(Condition.StringEqual(key, _lock));
            transaction.KeyDeleteAsync(key);
            transaction.Execute();
        }
    }
}
