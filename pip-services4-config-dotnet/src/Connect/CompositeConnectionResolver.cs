using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Config.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Config.Connect
{
	/// <summary>
	/// Helper class that resolves connection and credential parameters,
	/// validates them and generates connection options.
	/// ### Configuration parameters ###
	/// - connection(s):
	/// - discovery_key:                (optional) a key to retrieve the connection from <see cref="IDiscovery"/>
	///   - protocol:                    communication protocol
	///   - host:                        host name or IP address
	///   - port:                        port number
	///   - uri:                         resource URI or connection string with all parameters in it
	/// - credential(s):
	///   - store_key:                   (optional) a key to retrieve the credentials from <see cref="ICredentialStore"/>
	///   - username:                    user name
	///   - password:                    user password 
	///   
	/// ### References ###
	/// 
	/// - *:discovery:*:*:1.0          (optional) <see cref="IDiscovery"/> services to resolve connections
	/// - *:credential-store:*:*:1.0   (optional) Credential stores to resolve credentials
	/// </summary>
	class CompositeConnectionResolver
    {
        /// <summary>
        /// The connection options
        /// </summary>
        protected ConfigParams _options;

        /// <summary>
        /// The connections resolver.
        /// </summary>
        protected ConnectionResolver _connectionResolver = new ConnectionResolver();

        /// <summary>
        /// The credentials resolver.
        /// </summary>
        protected CredentialResolver _credentialResolver = new CredentialResolver();

        /// <summary>
        /// The cluster support (multiple connections)
        /// </summary>
        protected bool _clusterSupported = true;

        /// <summary>
        /// The default protocol
        /// </summary>
        protected string _defaultProtocol = null;

        /// <summary>
        /// The default port number
        /// </summary>
        protected int _defaultPort = 0;

        /// <summary>
        /// The list of supported protocols
        /// </summary>
        protected IList<string> _supportedProtocols = null;

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public void Configure(ConfigParams config)
        {
            _connectionResolver.Configure(config);
            _credentialResolver.Configure(config);
            _options = config.GetSection("options");
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies. </param>
        public void SetReferences(IReferences references)
        {
            _connectionResolver.SetReferences(references);
            _credentialResolver.SetReferences(references);
        }

        /// <summary>
        /// Resolves connection options from connection and credential parameters.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <returns>resolved options or throw error</returns>
        public async Task<ConfigParams> ResolveAsync(string correlationId)
        {
            List<ConnectionParams> connections = await _connectionResolver.ResolveAllAsync(correlationId);

            // Validate if cluster (multiple connections) is supported
            if (connections.Count > 0 && !_clusterSupported)
            {
                throw new ConfigException(
                            correlationId,
                            "MULTIPLE_CONNECTIONS_NOT_SUPPORTED",
                            "Multiple (cluster) connections are not supported"
                        );
            }

            // Validate connections
            foreach (var connection in connections)
                ValidateConnection(correlationId, connection);

            CredentialParams credential = await _credentialResolver.LookupAsync(correlationId);
            credential = credential != null ? credential : new CredentialParams();

            // Validate credential
            ValidateCredential(correlationId, credential);

            var options = ComposeOptions(connections, credential, _options);

            return options;
        }

        /// <summary>
        /// Composes Composite connection options from connection and credential parameters.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="connections">connection parameters</param>
        /// <param name="credential">credential parameters</param>
        /// <param name="parameters">optional parameters</param>
        /// <returns>resolved options or throw error</returns>
        public ConfigParams Compose(string correlationId, IList<ConnectionParams> connections, CredentialParams credential, ConfigParams parameters)
        {
            // Validate connection parameters
            foreach (var connection in connections)
                ValidateConnection(correlationId, connection);

            // Validate credential parameters
            ValidateCredential(correlationId, credential);

            var options = ComposeOptions(connections, credential, parameters);

            return options;
        }

        /// <summary>
        /// Validates connection parameters.
        /// This method can be overriden in child classes.
        /// Throw error if validation failed.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="connection">connection parameters to be validated</param>
        protected void ValidateConnection(string correlationId, ConnectionParams connection)
        {
            if (connection == null)
                throw new ConfigException(correlationId, "NO_CONNECTION", "Connection parameters are not set is not set");

            // URI usually contains all information
            var uri = connection.Uri;
            if (uri != null) return;

            var protocol = connection.GetProtocolWithDefault(_defaultProtocol);
            if (protocol == null)
                throw new ConfigException(correlationId, "NO_PROTOCOL", "Connection protocol is not set");

            if (_supportedProtocols != null && _supportedProtocols.IndexOf(protocol) < 0)
                throw new ConfigException(correlationId, "UNSUPPORTED_PROTOCOL", "The protocol " + protocol + " is not supported");

            var host = connection.Host;
            if (host == null)
                throw new ConfigException(correlationId, "NO_HOST", "Connection host is not set");

            var port = connection.GetPortWithDefault(_defaultPort);
            if (port == 0)
                throw new ConfigException(correlationId, "NO_PORT", "Connection port is not set");
        }

        /// <summary>
        /// Validates credential parameters.
        /// This method can be overriden in child classes.
        /// Throw error if validation failed.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="credential">credential parameters to be validated</param>
        protected void ValidateCredential(string correlationId, CredentialParams credential)
        {
            return;
        }

        /// <summary>
        /// Composes connection and credential parameters into connection options.
        /// This method can be overriden in child classes.
        /// </summary>
        /// <param name="connections">a list of connection parameters</param>
        /// <param name="credential">credential parameters</param>
        /// <param name="parameters">optional parameters</param>
        /// <returns>a composed connection options.</returns>
        protected ConfigParams ComposeOptions(IList<ConnectionParams> connections, CredentialParams credential, ConfigParams parameters)
        {
            // Connection options
            var options = new ConfigParams();

            // Merge connection parameters
            foreach (var connection in connections)
                options = MergeConnection(options, connection);

            // Merge credential parameters
            options = MergeCredential(options, credential);

            // Merge optional parameters
            options = MergeOptional(options, parameters);

            // Perform final processing
            options = FinalizeOptions(options);

            return options;
        }

        /// <summary>
        /// Merges connection options with connection parameters
        /// This method can be overriden in child classes.
        /// </summary>
        /// <param name="options">connection options</param>
        /// <param name="connection">connection parameters to be merged</param>
        /// <returns>merged connection options.</returns>
        protected ConfigParams MergeConnection(ConfigParams options, ConnectionParams connection)
        {
            var mergedOptions = options.SetDefaults(connection);
            return mergedOptions;
        }

        /// <summary>
        /// Merges connection options with credential parameters
        /// This method can be overriden in child classes.
        /// </summary>
        /// <param name="options">connection options</param>
        /// <param name="credential">credential parameters to be merged</param>
        /// <returns>merged connection options.</returns>
        protected ConfigParams MergeCredential(ConfigParams options, CredentialParams credential)
        {
            var mergedOptions = options.Override(credential);
            return mergedOptions;
        }

        /// <summary>
        /// Merges connection options with optional parameters
        /// This method can be overriden in child classes.
        /// </summary>
        /// <param name="options">connection options</param>
        /// <param name="parameters">optional parameters to be merged</param>
        /// <returns>merged connection options.</returns>
        protected ConfigParams MergeOptional(ConfigParams options, ConfigParams parameters)
        {
            var mergedOptions = options.SetDefaults(parameters);
            return mergedOptions;
        }

        /// <summary>
        /// Finalize merged options
        /// This method can be overriden in child classes.
        /// </summary>
        /// <param name="options">connection options</param>
        /// <returns>finalized connection options</returns>
        protected ConfigParams FinalizeOptions(ConfigParams options)
        {
            return options;
        }
    }
}
