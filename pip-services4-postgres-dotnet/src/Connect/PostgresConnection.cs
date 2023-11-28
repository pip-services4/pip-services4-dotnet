using Npgsql;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Observability.Log;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PipServices4.Postgres.Connect
{
    /// <summary>
    /// PostgreSQL connection using plain driver.
    /// 
    /// By defining a connection and sharing it through multiple persistence components
    /// you can reduce number of used database connections.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// connection(s):
    /// - discovery_key:             (optional) a key to retrieve the connection from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
    /// - host:                      host name or IP address
    /// - port:                      port number (default: 27017)
    /// - uri:                       resource URI or connection string with all parameters in it
    /// 
    /// credential(s):
    /// - store_key:                 (optional) a key to retrieve the credentials from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_auth_1_1_i_credential_store.html">ICredentialStore</a>
    /// - username:                  (optional) user name
    /// - password:                  (optional) user password
    /// 
    /// options:
    /// - max_pool_size:             (optional) maximum connection pool size (default: 2)
    /// - keep_alive:                (optional) enable connection keep alive (default: true)
    /// - connect_timeout:           (optional) connection timeout in milliseconds (default: 5 sec)
    /// - auto_reconnect:            (optional) enable auto reconnection (default: true)
    /// - max_page_size:             (optional) maximum page size (default: 100)
    /// - debug:                     (optional) enable debug output (default: false).
    /// 
    /// ### References ###
    /// 
    /// - *:logger:*:*:1.0           (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// - *:discovery:*:*:1.0        (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services
    /// - *:credential-store:*:*:1.0 (optional) Credential stores to resolve credentials
    /// </summary>
    public class PostgresConnection : IReferenceable, IReconfigurable, IOpenable
    {
        private ConfigParams _defaultConfig = ConfigParams.FromTuples(
            "options.connect_timeout", 0,
            "options.idle_timeout", 10000,
            "options.max_pool_size", 3
        );

        protected string _connectionString;

        /// <summary>
        /// The connection resolver.
        /// </summary>
        protected PostgresConnectionResolver _connectionResolver = new PostgresConnectionResolver();

        /// <summary>
        /// The configuration options.
        /// </summary>
        protected ConfigParams _options = new ConfigParams();

        /// <summary>
        /// The database name.
        /// </summary>
        protected string _databaseName;

        /// <summary>
        /// The logger.
        /// </summary>
        protected CompositeLogger _logger = new CompositeLogger();

        /// <summary>
        /// Creates a new instance of the connection component.
        /// </summary>
        public PostgresConnection()
        { }

        /// <summary>
        /// Gets PostgreSQL connection object.
        /// </summary>
        /// <returns>The PostgreSQL connection object.</returns>
        public async Task<NpgsqlConnection> GetConnection(IContext context = null)
        {
            return await CreateAndOpenConnectionAsync(context);
        }

        /// <summary>
        /// Gets the name of the connected database.
        /// </summary>
        /// <returns>The name of the connected database.</returns>
        public string GetDatabaseName()
        {
            return _databaseName;
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public void SetReferences(IReferences references)
        {
            _logger.SetReferences(references);
            _connectionResolver.SetReferences(references);
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public virtual void Configure(ConfigParams config)
        {
            config = config.SetDefaults(_defaultConfig);

            _connectionResolver.Configure(config);

            _options = _options.Override(config.GetSection("options"));
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public virtual bool IsOpen()
        {
            return _databaseName != null;
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async virtual Task OpenAsync(IContext context)
        {
            var connectionString = await _connectionResolver.ResolveAsync(context);

            var settings = ComposeSettings();
            _connectionString = connectionString.TrimEnd(';') + ";" + JoinParams(settings);

            using (var connection = await CreateAndOpenConnectionAsync(context))
            {
                _databaseName = connection.Database;
            }
        }

        protected async virtual Task<NpgsqlConnection> CreateAndOpenConnectionAsync(IContext context)
        {
            _logger.Trace(context, "Connecting to postgres...");

            try
            {
                var connection = new NpgsqlConnection(_connectionString);

                // Try to connect
                await connection.OpenAsync();

                _logger.Debug(context, "Connected to postgres database {0}", connection.Database);

                return connection;
            }
            catch (Exception ex)
            {
                throw new ConnectionException(context != null ? ContextResolver.GetTraceId(context) : null, 
                    "CONNECT_FAILED", "Connection to postgres failed", ex);
            }
        }

        private ConfigParams ComposeSettings()
        {
            var maxPoolSize = _options.GetAsNullableInteger("max_pool_size");
            var connectTimeout = _options.GetAsNullableInteger("connect_timeout");
            var idleTimeout = _options.GetAsNullableInteger("idle_timeout");

            ConfigParams settings = new ConfigParams();

            if (maxPoolSize.HasValue) settings["Maximum Pool Size"] = maxPoolSize.Value.ToString();
            if (connectTimeout.HasValue) settings["Timeout"] = connectTimeout.Value.ToString();
            if (idleTimeout.HasValue) settings["Keepalive"] = idleTimeout.Value.ToString();

            return settings;
        }

        private static string JoinParams(ConfigParams config)
        { 
            return string.Join(";", config.Select(x => string.Format("{0}={1}", x.Key, x.Value))); 
        }
        
        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async virtual Task CloseAsync(IContext context)
        {
            _databaseName = null;
            await Task.Delay(0);
        }
    }
}
