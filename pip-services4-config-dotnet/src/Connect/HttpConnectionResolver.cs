using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Config.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Config.Connect
{
	/// <summary>
	/// Helper class to retrieve connections for HTTP-based services abd clients.
	/// 
	/// In addition to regular functions of ConnectionResolver is able to parse http:// URIs
	/// and validate connection parameters before returning them.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// connection:
	/// - discovery_key:               (optional) a key to retrieve the connection from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
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
	/// - *:discovery:*:*:1.0            (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services
	/// </summary>
	/// <example>
	/// <code>
	/// var config = ConfigParams.fromTuples(
	/// "connection.host", "10.1.1.100",
	/// "connection.port", 8080 );
	/// 
	/// var connectionResolver = new HttpConnectionResolver();
	/// connectionResolver.Configure(config);
	/// connectionResolver.SetReferences(references);
	/// 
	/// var params = connectionResolver.ResolveAsync("123");
	/// </code>
	/// </example>
	public class HttpConnectionResolver : IReferenceable, IConfigurable
    {
        /// <summary>
        /// The base connection resolver.
        /// </summary>
        protected ConnectionResolver _connectionResolver = new ConnectionResolver();

        /// <summary>
        /// The base credential resolver.
        /// </summary>
        protected CredentialResolver _credentialResolver = new CredentialResolver();

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
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public void Configure(ConfigParams config)
        {
            _connectionResolver.Configure(config);
            _credentialResolver.Configure(config);
        }

        private void ValidateConnection(IContext context, ConnectionParams connection, CredentialParams credential)
        {
            var traceId = context != null ? ContextResolver.GetTraceId(context) : null;

            if (connection == null)
                throw new ConfigException(traceId, "NO_CONNECTION", "HTTP connection is not set");

            var uri = connection.Uri;
            if (!string.IsNullOrEmpty(uri))
                return;

            var protocol = connection.GetProtocolWithDefault("http");
            if ("http" != protocol && "https" != protocol)
            {
                throw new ConfigException(
                        traceId, "WRONG_PROTOCOL", "Protocol is not supported by REST connection")
                    .WithDetails("protocol", protocol);
            }

            var host = connection.Host;
            if (host == null)
                throw new ConfigException(traceId, "NO_HOST", "Connection host is not set");

            var port = connection.Port;
            if (port == 0)
                throw new ConfigException(traceId, "NO_PORT", "Connection port is not set");

            // Check HTTPS credentials
            if (protocol == "https")
            {
                // Check for credential
                if (credential == null)
                {
                    throw new ConfigException(
                        traceId, "NO_CREDENTIAL", "SSL certificates are not configured for HTTPS protocol");
                }

                // Sometimes when we use https we are on an internal network and do not want to have to deal with security.
                // When we need a https connection and we don't want to pass credentials, set the value 'no_credentials_needed' in the config yml credentials section
                if (credential.GetAsNullableString("internal_network") == null)
                {
                    if (credential.GetAsNullableString("ssl_password") == null)
                    {
                        throw new ConfigException(
                            traceId, "NO_SSL_PASSWORD", "SSL password is not configured in credentials");
                    }

                    if (credential.GetAsNullableString("ssl_pfx_file") == null)
                    {
                        throw new ConfigException(
                            traceId, "NO_SSL_PFX_FILE", "SSL pfx file is not configured in credentials");
                    }
                }
            }
        }

        private void UpdateConnection(ConnectionParams connection, CredentialParams credential)
        {
            if (string.IsNullOrEmpty(connection.Uri))
            {
                var uri = connection.Protocol + "://" + connection.Host;
                if (connection.Port != 0)
                    uri += ":" + connection.Port;
                connection.Uri = uri;
            }
            else
            {
                var uri = new Uri(connection.Uri);
                connection.Protocol = uri.Scheme;
                connection.Host = uri.Host;
                connection.Port = uri.Port;
            }

            if (connection.Protocol == "https")
            {
                connection.AddSection("credential",
                    credential.GetAsNullableString("internal_network") == null ? credential : new CredentialParams());
            }
            else
            {
                connection.AddSection("credential", new CredentialParams());
            }
        }

        /// <summary>
        /// Resolves a single component connection. If connections are configured to be
        /// retrieved from Discovery service it finds a IDiscovery and resolves the connection there.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns>resolved connection.</returns>
        public async Task<ConnectionParams> ResolveAsync(IContext context)
        {
            var connection = await _connectionResolver.ResolveAsync(context);
            var credential = await _credentialResolver.LookupAsync(context);
            ValidateConnection(context, connection, credential);
            UpdateConnection(connection, credential);
            return connection;
        }

        /// <summary>
        /// Resolves all component connection. If connections are configured to be
        /// retrieved from Discovery service it finds a IDiscovery and resolves the connection there.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns>resolved connections.</returns>
        public async Task<List<ConnectionParams>> ResolveAllAsync(IContext context)
        {
            var connections = await _connectionResolver.ResolveAllAsync(context);
            var credential = await _credentialResolver.LookupAsync(context);
            foreach (var connection in connections)
            {
                ValidateConnection(context, connection, credential);
                UpdateConnection(connection, credential);
            }

            return connections;
        }

        /// <summary>
        /// Registers the given connection in all referenced discovery services. This
        /// method can be used for dynamic service discovery.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async Task RegisterAsync(IContext context)
        {
            var connection = await _connectionResolver.ResolveAsync(context);
            var credential = await _credentialResolver.LookupAsync(context);
            ValidateConnection(context, connection, credential);

            await _connectionResolver.RegisterAsync(context, connection);
        }
    }
}