using PipServices4.Commons.Data;
using PipServices4.Components.Config;
using System.Collections.Generic;

namespace PipServices4.Config.Connect
{
	/// <summary>
	/// Contains connection parameters to connect to external services.
	/// They are used together with credential parameters, but usually stored
	/// separately from more protected sensitive values.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - discovery_key: key to retrieve parameters from discovery service 
	/// - protocol:      connection protocol like http, https, tcp, udp
	/// - host:          host name or IP address 
	/// - port:          port number
	/// - uri:           resource URI or connection string with all parameters in it
	/// 
	/// In addition to standard parameters ConnectionParams may contain any number of custom parameters
	/// </summary>
	/// <example>
	/// <code>
	/// Example ConnectionParams object usage:
	/// 
	/// var connection = ConnectionParams.FromTuples(
	///     "protocol", "http",
	///     "host", "10.1.1.100",
	///     "port", "8080",
	///     "cluster", "mycluster"
	/// );
	/// 
	/// var host = connection.Host;                             // Result: "10.1.1.100"
	/// var port = connection.Port;                             // Result: 8080
	/// var cluster = connection.GetAsNullableString("cluster");     // Result: "mycluster" 
	/// </code>
	/// </example>
	/// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_config_1_1_config_params.html">ConfigParams</a>, 
	/// <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_auth_1_1_credential_params.html">CredentialParams</a>, 
	/// <see cref="ConnectionResolver"/>, 
	/// <see cref="IDiscovery"/>
	public class ConnectionParams : ConfigParams
    {
        /// <summary>
        /// Creates a new connection parameters and fills it with values.
        /// </summary>
        public ConnectionParams() { }

        /// <summary>
        /// Creates a new connection parameters and fills it with values.
        /// </summary>
        /// <param name="map">(optional) an object to be converted into key-value pairs to initialize this connection.</param>
        public ConnectionParams(IDictionary<string, string> map)
            : base(map)
        { }

        /// <summary>
        /// Checks if these connection parameters shall be retrieved from
        /// DiscoveryService.The connection parameters are redirected to
        /// DiscoveryService when discovery_key parameter is set.
        /// </summary>
        public bool UseDiscovery
        {
            get { return ContainsKey("discovery_key"); }
        }

        /// <summary>
        /// Gets or sets the key to retrieve this connection from DiscoveryService. 
        /// </summary>
        public string DiscoveryKey
        {
            get { return GetAsNullableString("discovery_key"); }
            set { this["discovery_key"] = value; }
        }

        /// <summary>
        /// Gets or sets the connection protocol
        /// </summary>
        public string Protocol
        {
            get { return GetAsNullableString("protocol") ?? "http"; }
            set { this["protocol"] = value; }
        }

        /// <summary>
        /// Gets the connection protocol
        /// </summary>
        /// <returns>the connection protocol or the default value if it's not set.</returns>
        public string GetProtocol()
        {
            return GetAsString("protocol");
        }

        /// <summary>
        /// Gets the connection protocol with default value.
        /// </summary>
        /// <param name="defaultValue">the default protocol</param>
        /// <returns>the connection protocol or the default value if it's not set.</returns>
        public string GetProtocolWithDefault(string defaultValue)
        {
            return GetAsStringWithDefault("protocol", defaultValue);
        }

        /// <summary>
        /// Gets or sets the service host name or IP address.
        /// </summary>
        public string Host
        {
            get
            {
                var host = GetAsNullableString("host");
                host = host ?? GetAsNullableString("ip");
                return string.IsNullOrWhiteSpace(host) ? "localhost" : host;
            }
            set
            {
                this["host"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the service port number
        /// </summary>
        public int Port
        {
            get { return GetAsIntegerWithDefault("port", 8080); }
            set { SetAsObject("port", value); }
        }

        /// <summary>
        /// Gets the port number.
        /// </summary>
        /// <returns>the port number.</returns>
        public int GetPort()
        {
            return GetAsInteger("port");
        }

        /// <summary>
        /// Gets the port number with default value.
        /// </summary>
        /// <param name="defaultPort">a default port number.</param>
        /// <returns>the port number.</returns>
        public int GetPortWithDefault(int defaultPort)
        {
            return GetAsIntegerWithDefault("port", defaultPort);
        }

        /// <summary>
        /// Gets the resource URI or connection string. Usually it includes all connection parameters in it.
        /// </summary>
        public string Uri
        {
            get { return GetAsNullableString("uri"); }
            set { SetAsObject("uri", value); }
        }

        /// <summary>
        /// Creates a new ConnectionParams object filled with key-value pairs serialized as a string.
        /// </summary>
        /// <param name="line">a string with serialized key-value pairs as
        /// "key1=value1;key2=value2;..." Example:
        /// "Key1=123;Key2=ABC;Key3=2016-09-16T00:00:00.00Z"</param>
        /// <returns>a new ConnectionParams object.</returns>
        public new static ConnectionParams FromString(string line)
        {
            var map = StringValueMap.FromString(line);
            return new ConnectionParams(map);
        }

        /// <summary>
        /// Creates a new ConnectionParams object filled with provided key-value pairs called tuples.
        /// Tuples parameters contain a sequence of key1, value1, key2, value2, ... pairs.
        /// </summary>
        /// <param name="tuples">the tuples to fill a new ConnectionParams object.</param>
        /// <returns>a new ConnectionParams object.</returns>
        public static new ConnectionParams FromTuples(params object[] tuples)
        {
            var map = StringValueMap.FromTuplesArray(tuples);
            return new ConnectionParams(map);
        }

        /// <summary>
        /// Retrieves all ConnectionParams from configuration parameters from
        /// "connections" section.If "connection" section is present instead, than it
        /// returns a list with only one ConnectionParams.
        /// </summary>
        /// <param name="config">a configuration parameters to retrieve connections</param>
        /// <param name="configAsDefault">boolean parameter for default configuration. If "true"
        /// the default value will be added to the result.</param>
        /// <returns>a list of retrieved ConnectionParams</returns>
        public static List<ConnectionParams> ManyFromConfig(ConfigParams config, bool configAsDefault = true)
        {
            var result = new List<ConnectionParams>();

            // Try to get multiple connections first
            var connections = config.GetSection("connections");

            if (connections.Count > 0)
            {
                var connectionSections = connections.GetSectionNames();

                foreach (var section in connectionSections)
                {
                    var connection = connections.GetSection(section);
                    result.Add(new ConnectionParams(connection));
                }
            }
            // Then try to get a single connection
            else
            {
                var connection = config.GetSection("connection");
                if (connection.Count > 0)
                    result.Add(new ConnectionParams(connection));
                // Apply default if possible
                else if (configAsDefault)
                    result.Add(new ConnectionParams(config));
            }

            return result;
        }

        /// <summary>
        /// Retrieves a single ConnectionParams from configuration parameters from
        /// "connection" section.If "connections" section is present instead, then is
        /// returns only the first connection element.
        /// </summary>
        /// <param name="config">ConnectionParams, containing a section named "connection(s)".</param>
        /// <param name="configAsDefault">boolean parameter for default configuration. If "true"
        /// the default value will be added to the result.</param>
        /// <returns>the generated ConnectionParams object.</returns>
        public static ConnectionParams FromConfig(ConfigParams config, bool configAsDefault = true)
        {
            var connections = ManyFromConfig(config, configAsDefault);
            return connections.Count > 0 ? connections[0] : null;
        }
    }
}