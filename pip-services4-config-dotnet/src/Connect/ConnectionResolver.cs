using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Config.Connect
{
	/// <summary>
	/// Helper class to retrieve component connections.
	/// 
	/// If connections are configured to be retrieved from IDiscovery,
	/// it automatically locates IDiscovery in component references
	/// and retrieve connections from there using <c>discovery_key</c> parameter.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// connection:
	/// - discovery_key:               (optional) a key to retrieve the connection from IDiscovery
	/// - ...                          other connection parameters
	/// 
	/// connections:                   alternative to connection
	/// - [connection params 1]:       first connection parameters
	/// - ...
	/// - [connection params N]:       Nth connection parameters
	/// - ...
	/// 
	/// ### References ###
	/// 
	/// - *:discovery:*:*:1.0            (optional) IDiscovery services to resolve connections
	/// </summary>
	/// <example>
	/// <code>
	/// var config = ConfigParams.FromTuples(
	/// "connection.host", "10.1.1.100",
	/// "connection.port", 8080
	/// );
	/// 
	/// var connectionResolver = new ConnectionResolver();
	/// connectionResolver.Configure(config);
	/// connectionResolver.SetReferences(references);
	/// connectionResolver.ResolveAsync("123");
	/// </code>
	/// </example>
	/// See <see cref="ConnectionParams"/>, <see cref="IDiscovery"/>
	public sealed class ConnectionResolver
    {
        private readonly List<ConnectionParams> _connections = new List<ConnectionParams>();
        private IReferences _references;

        /// <summary>
        /// Creates a new instance of connection resolver.
        /// </summary>
        /// <param name="config">(optional) component configuration parameters</param>
        /// <param name="references">(optional) component references</param>
        public ConnectionResolver(ConfigParams config = null, IReferences references = null)
        {
            if (config != null) Configure(config);
            if (references != null) SetReferences(references);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public void SetReferences(IReferences references)
        {
            _references = references;
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        /// <param name="configAsDefault">boolean parameter for default configuration. If "true"
        /// the default value will be added to the result.</param>
        public void Configure(ConfigParams config, bool configAsDefault = true)
        {
            _connections.AddRange(ConnectionParams.ManyFromConfig(config, configAsDefault));
        }

        /// <summary>
        /// Gets all connections configured in component configuration.
        /// 
        /// Redirect to Discovery services is not done at this point.If you need fully
        /// fleshed connection use resolve() method instead.
        /// </summary>
        /// <returns>a list with connection parameters</returns>
        public List<ConnectionParams> GetAll()
        {
            return _connections;
        }

        /// <summary>
        /// Adds a new connection to component connections
        /// </summary>
        /// <param name="connection">new connection parameters to be added</param>
        public void Add(ConnectionParams connection)
        {
            _connections.Add(connection);
        }

        private async Task<bool> RegisterInDiscoveryAsync(IContext context, ConnectionParams connection)
        {
            if (!connection.UseDiscovery) return false;

            var key = connection.DiscoveryKey;
            if (_references == null) return false;

            var discoveries = _references.GetOptional<IDiscovery>(new Descriptor("*", "discovery", "*", "*", "*"));
            if (discoveries == null) return false;

            foreach (var discovery in discoveries)
                await discovery.RegisterAsync(context, key, connection);

            return true;
        }

        /// <summary>
        /// Registers the given connection in all referenced discovery services. This
        /// method can be used for dynamic service discovery.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="connection">a connection to register.</param>
        public async Task RegisterAsync(IContext context, ConnectionParams connection)
        {
            var result = await RegisterInDiscoveryAsync(context, connection);

            if (result)
                _connections.Add(connection);
        }

        private async Task<ConnectionParams> ResolveInDiscoveryAsync(IContext context, ConnectionParams connection)
        {
            if (connection.UseDiscovery == false)
                return null;

            var key = connection.DiscoveryKey;
            if (_references == null) return null;

            var traceId = context != null ? ContextResolver.GetTraceId(context) : null;

            var discoveries = _references.GetOptional<IDiscovery>(new Descriptor("*", "discovery", "*", "*", "*"));
            if (discoveries.Count == 0)
                throw new ConfigException(traceId, "CANNOT_RESOLVE", "Discovery wasn't found to make resolution");

            foreach (var discovery in discoveries)
            {
                var resolvedConnection = await discovery.ResolveOneAsync(context, key);
                if (resolvedConnection != null)
                    return resolvedConnection;
            }

            return null;
        }

        /// <summary>
        /// Resolves a single component connection. If connections are configured to be
        /// retrieved from Discovery service it finds a IDiscovery and resolves the connection there.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns>resolved connection parameters or null if nothing was found.</returns>
        public async Task<ConnectionParams> ResolveAsync(IContext context)
        {
            if (_connections.Count == 0) return null;

            // Return connection that doesn't require discovery
            foreach (var connection in _connections)
            {
                if (!connection.UseDiscovery)
                    return connection;
            }

            // Return connection that require discovery
            foreach (var connection in _connections)
            {
                if (connection.UseDiscovery)
                {
                    var resolvedConnection = await ResolveInDiscoveryAsync(context, connection);
                    if (resolvedConnection != null)
                    {
                        // Merge configured and new parameters
                        resolvedConnection = new ConnectionParams(ConfigParams.MergeConfigs(connection, resolvedConnection));
                        return resolvedConnection;
                    }
                }
            }

            return null;
        }

        private async Task<List<ConnectionParams>> ResolveAllInDiscoveryAsync(IContext context, ConnectionParams connection)
        {
            var result = new List<ConnectionParams>();

            if (connection.UseDiscovery == false)
                return result;

            var key = connection.DiscoveryKey;
            if (_references == null) return null;

            var discoveries = _references.GetOptional<IDiscovery>(new Descriptor("*", "discovery", "*", "*", "*"));
            if (discoveries.Count == 0)
                throw new ConfigException(
                    context != null ? ContextResolver.GetTraceId(context) : null
                    , "CANNOT_RESOLVE", "Discovery wasn't found to make resolution");

            foreach (var discovery in discoveries)
            {
                var resolvedConnections = await discovery.ResolveAllAsync(context, key);
                if (resolvedConnections != null)
                    result.AddRange(resolvedConnections);
            }

            return result;
        }

        /// <summary>
        /// Resolves all component connection. If connections are configured to be
        /// retrieved from Discovery service it finds a IDiscovery and resolves the connection there.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns>a list of resolved connections.</returns>
        public async Task<List<ConnectionParams>> ResolveAllAsync(IContext context)
        {
            var resolved = new List<ConnectionParams>();
            var toResolve = new List<ConnectionParams>();

            // Sort connections
            foreach (var connection in _connections)
            {
                if (connection.UseDiscovery)
                    toResolve.Add(connection);
                else
                    resolved.Add(connection);
            }

            // Resolve addresses that require that
            if (toResolve.Count <= 0)
                return resolved;

            foreach (var connection in toResolve)
            {
                var resolvedConnections = await ResolveAllInDiscoveryAsync(context, connection);

                foreach (var resolvedConnection in resolvedConnections)
                {
                    // Merge configured and new parameters
                    var localResolvedConnection = new ConnectionParams(ConfigParams.MergeConfigs(connection, resolvedConnection));
                    resolved.Add(localResolvedConnection);
                }
            }

            return resolved;
        }
    }
}
