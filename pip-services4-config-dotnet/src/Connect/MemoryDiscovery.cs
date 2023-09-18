using PipServices4.Components.Config;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Config.Connect
{
	/// <summary>
	/// Discovery service that keeps connections in memory.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - [connection key 1]:
	/// - ...                          connection parameters for key 1
	/// - [connection key 2]:
	/// - ...                          connection parameters for key N
	/// </summary>
	/// <example>
	/// <code>
	/// ConfigParams config = ConfigParams.FromTuples(
	///     "connections.key1.host", "10.1.1.100",
	///     "connections.key1.port", "8080",
	///     "connections.key2.host", "10.1.1.101",
	///     "connections.key2.port", "8082"
	/// );
	/// 
	/// MemoryDiscovery discovery = new MemoryDiscovery();
	/// discovery.Configure(config);
	/// await discovery.resolveOne("123", "key1");
	/// </code>
	/// </example>
	/// See <see cref="IDiscovery"/>, <see cref="ConnectionParams"/>
	public class MemoryDiscovery : IDiscovery, IReconfigurable
    {
        private List<DiscoveryItem> _items = new List<DiscoveryItem>();
        private object _lock = new object();

        /// <summary>
        /// Creates a new instance of discovery service.
        /// </summary>
        public MemoryDiscovery() { }

        /// <summary>
        /// Creates a new instance of discovery service.
        /// </summary>
        /// <param name="config">(optional) configuration with connection parameters.</param>
        public MemoryDiscovery(ConfigParams config = null)
        {
            if (config != null) Configure(config);
        }

        private class DiscoveryItem
        {
            public string Key;
            public ConnectionParams Connection;
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public virtual void Configure(ConfigParams config)
        {
            ReadConnections(config);
        }

        /// <summary>
        /// Reads connections from configuration parameters.
        /// Each section represents an individual Connectionparams
        /// </summary>
        /// <param name="connections">configuration parameters to be read</param>
        private void ReadConnections(ConfigParams connections)
        {
            lock (_lock)
            {
                _items.Clear();
                var connects = connections.GetSection("connections");

                if (connections.Count > 0)
                {
                    var connectionSections = connects.GetSectionNames();

                    foreach (var key in connectionSections)
                    {
                        var config = connects.GetSection(key);

                        var item = new DiscoveryItem()
                        {
                            Key = key,
                            Connection = new ConnectionParams(config)
                        };
                        _items.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Registers connection parameters into the discovery service.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the connection parameters.</param>
        /// <param name="connection">a connection to be registered.</param>
        public async Task RegisterAsync(string correlationId, string key, ConnectionParams connection)
        {
            lock (_lock)
            {
                var item = new DiscoveryItem()
                {
                    Key = key,
                    Connection = connection
                };
                _items.Add(item);
            }

            await Task.Delay(0);
        }

        /// <summary>
        /// Resolves a single connection parameters by its key.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the connection.</param>
        /// <returns>a resolved connection.</returns>
        public async Task<ConnectionParams> ResolveOneAsync(string correlationId, string key)
        {
            ConnectionParams connection = null;

            lock (_lock)
            {
                foreach (var item in _items)
                {
                    if (item.Key == key && item.Connection != null)
                    {
                        connection = item.Connection;
                        break;
                    }
                }
            }

            return await Task.FromResult(connection);
        }

        /// <summary>
        /// Resolves all connection parameters by their key.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the connection.</param>
        /// <returns>a list with resolved connections.</returns>
        public async Task<List<ConnectionParams>> ResolveAllAsync(string correlationId, string key)
        {
            var connections = new List<ConnectionParams>();

            lock (_lock)
            {
                foreach (var item in _items)
                {
                    if (item.Key == key && item.Connection != null)
                        connections.Add(item.Connection);
                }
            }

            return await Task.FromResult(connections);
        }
    }
}
