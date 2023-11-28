using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Config.Auth;
using PipServices4.Config.Connect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlData = MySql.Data;

namespace PipServices4.Mysql.Connect
{
    /// <summary>
    /// Helper class that resolves MySql connection and credential parameters,
    /// validates them and generates a connection URI.
    /// 
    /// It is able to process multiple connections to MySql cluster nodes.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// connection(s):
    /// - discovery_key:               (optional) a key to retrieve the connection from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
    /// - host:                        host name or IP address
    /// - port:                        port number (default: 27017)
    /// - database:                    database name
    /// - uri:                         resource URI or connection string with all parameters in it 
    /// 
    /// credential(s):
    /// - store_key:                   (optional) a key to retrieve the credentials from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_auth_1_1_i_credential_store.html">ICredentialStore</a>
    /// - username:                    user name
    /// - password:                    user password
    /// 
    /// ### References ###
    /// 
    /// - *:discovery:*:*:1.0          (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services
    /// - *:credential-store:*:*:1.0   (optional) Credential stores to resolve credentials
    /// </summary>
    public class MySqlConnectionResolver: IReferenceable, IConfigurable
    {
        /// <summary>
        /// The connections resolver.
        /// </summary>
        protected ConnectionResolver _connectionResolver = new ConnectionResolver();
        /// <summary>
        /// The credentials resolver.
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
            _connectionResolver.Configure(config, false);
            _credentialResolver.Configure(config, false);
        }

        private static void ValidateConnection(IContext context, ConnectionParams connection)
        {
            var uri = connection.Uri;
            if (uri != null) return;

            var traceId = context != null ? ContextResolver.GetTraceId(context) : null;
            var host = connection.Host;
            if (host == null)
                throw new ConfigException(traceId, "NO_HOST", "Connection host is not set");

            var port = connection.Port;
            if (port == 0)
                throw new ConfigException(traceId, "NO_PORT", "Connection port is not set");

            var database = connection.GetAsNullableString("database");
            if (database == null)
                throw new ConfigException(traceId, "NO_DATABASE", "Connection database is not set");
        }

        private static void ValidateConnections(IContext context, List<ConnectionParams> connections)
        {
            var traceId = context != null ? ContextResolver.GetTraceId(context) : null;

            if (connections == null || connections.Count == 0)
                throw new ConfigException(traceId, "NO_CONNECTION", "Database connection is not set");

            foreach (var connection in connections)
                ValidateConnection(context, connection);
        }

        private static string ComposeUri1(List<ConnectionParams> connections, CredentialParams credential)
        {
            // If there is a uri then return it immediately
            string uri;
            foreach (var connection in connections)
            {
                uri = connection.Uri;
                if (!string.IsNullOrWhiteSpace(uri))
                    return uri;
            }

            var builder = new MySqlData.MySqlClient.MySqlConnectionStringBuilder();

            // Define hosts
            foreach (var connection in connections)
            {
				var host = connection.Host;
				var port = connection.Port;

				builder.Server = host;
                if (port > 0) builder.Port = Convert.ToUInt32(port);
            }

            // Define database
            foreach (var connection in connections)
            {
                var database = connection.GetAsNullableString("database");
                if (!string.IsNullOrWhiteSpace(database))
                {
                    builder.Database = database;
                    break;
                }
            }

            // Define authentication part
            if (credential != null)
            {
                var username = credential.Username;
                if (!string.IsNullOrWhiteSpace(username))
                {
                    builder.UserID = username;

                    var password = credential.Password;
                    if (!string.IsNullOrWhiteSpace(password))
                        builder.Password = password;
                }
            }

            // Define additional parameters parameters

            var options = ConfigParams.MergeConfigs(connections.Select(c => c.ToDictionary(p => p.Key, p => p.Value)).ToArray()).Override(credential);
            options.Remove("uri");
            options.Remove("host");
            options.Remove("port");
            options.Remove("database");
            options.Remove("username");
            options.Remove("password");

			foreach (var key in options.Keys)
			{
                builder[key] = options[key];
			}

            // Compose uri
            uri = builder.ToString();
            
            return uri;
        }

        /// <summary>
        /// Resolves MySql connection string from connection and credential parameters.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns>resolved connection string.</returns>
        public async Task<string> ResolveAsync(IContext context)
        {
            var connections = await _connectionResolver.ResolveAllAsync(context);
            var credential = await _credentialResolver.LookupAsync(context);

            ValidateConnections(context, connections);

            return ComposeUri1(connections, credential);
        }
    }
}