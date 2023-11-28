using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Messaging.Connect;
using PipServices4.Observability.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Mqtt.Connect
{
    /// <summary>
    ///  MQTT connection using plain driver.
    ///  By defining a connection and sharing it through multiple message queues
    ///  you can reduce number of used connections.

    ///  ### Configuration parameters ###
    ///    - client_id:               (optional) name of the client id
    ///    - connection(s):
    ///      - discovery_key:               (optional) a key to retrieve the connection from [[https://pip-services4-node.github.io/pip-services4-components-node/interfaces/connect.idiscovery.html IDiscovery]]
    ///      - host:                        host name or IP address
    ///      - port:                        port number
    ///      - uri:                         resource URI or connection string with all parameters in it
    ///    - credential(s):
    ///      - store_key:                   (optional) a key to retrieve the credentials from[[https://pip-services4-node.github.io/pip-services4-components-node/interfaces/auth.icredentialstore.html ICredentialStore]]
    ///      -username:                    user name
    ///      - password:                    user password
    ///    - options:
    ///      -retry_connect:        (optional)turns on / off automated reconnect when connection is log(default: true)
    ///      - connect_timeout:      (optional)number of milliseconds to wait for connection(default: 30000)
    ///      - reconnect_timeout:    (optional)number of milliseconds to wait on each reconnection attempt(default: 1000)
    ///      - keepalive_timeout:    (optional)number of milliseconds to ping broker while inactive(default: 3000)
    ///  
    ///  ### References ###
    ///   - \*:logger:\*:\*:1.0(optional) ILogger components to pass log messages
    ///   - \*:discovery:\*:\*:1.0(optional) IDiscovery services
    ///   - \*:credential - store:\*:\*:1.0(optional) Credential stores to resolve credentials
    /// </summary>
    public class MqttConnection: IMessageQueueConnection, IConfigurable, IReferenceable, IOpenable
    {
        private static ConfigParams _defaultConfig = ConfigParams.FromTuples(
            "options.retry_connect", true,
            "options.connect_timeout", 30000,
            "options.reconnect_timeout", 1000,
            "options.keepalive_timeout", 60000
        );
        protected CompositeLogger _logger = new CompositeLogger();
        protected MqttConnectionResolver _connectionResolver = new MqttConnectionResolver();
        protected ConfigParams _options = new ConfigParams();

        // MQTT connection object
        private IMqttClientOptions _clientOptions;
        protected IMqttClient _connection;

        // Topic subscriptions
        protected List<MqttSubscription> _subscriptions = new List<MqttSubscription>();
        protected object _lock = new object();

        // Connection options
        private string _clientId;
        private bool _retryConnect = true;
        private int _connectTimeout = 30000;
        private int _reconnectTimeout = 60000;
        private int _keepAliveTimeout = 1000;

        public MqttConnection()
        {
        }

        /// <summary>
        /// Configure are configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">Configuration parameters to be set</param>
        public void Configure(ConfigParams config)
        {
            config = config.SetDefaults(_defaultConfig);
            _connectionResolver.Configure(config);
            _logger.Configure(config);

            _options = _options.Override(config.GetSection("options"));

            _clientId = config.GetAsStringWithDefault("client_id", _clientId);
            _retryConnect = config.GetAsBooleanWithDefault("options.retry_connect", _retryConnect);
            _connectTimeout = config.GetAsIntegerWithDefault("options.connect_timeout", _connectTimeout);
            _reconnectTimeout = config.GetAsIntegerWithDefault("options.reconnect_timeout", _reconnectTimeout);
            _keepAliveTimeout = config.GetAsIntegerWithDefault("options.keepalive_timeout", _keepAliveTimeout);
        }

        /// <summary>
        /// SetReferences are sets references to dependent components.
        /// </summary>
        /// <param name="references">References to be set</param>
        public void SetReferences(IReferences references)
        {
            _connectionResolver.SetReferences(references);
            _logger.SetReferences(references);
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public bool IsOpen()
        {
            return _connection != null;
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async Task OpenAsync(IContext context)
        {
            var options = await _connectionResolver.ResolveAsync(context);

            var opts = new MqttClientOptionsBuilder()
                .WithClientId(_clientId);

            if (_keepAliveTimeout > 0)
            {
                opts.WithKeepAlivePeriod(TimeSpan.FromMilliseconds(_keepAliveTimeout));
            }
            else
            {
                opts.WithNoKeepAlive();
            }

            var uri = options.GetAsString("servers") ?? "";
            var servers = uri.Split(',');
            foreach (var server in servers) {
                var host = server;
                var port = 1883;

                var pos = server.IndexOf(":");
                if (pos > 0)
                {
                    host = server.Substring(0, pos);
                    Int32.TryParse(server.Substring(pos + 1), out port);
                }

                opts.WithTcpServer(host, port);
            }

            var username = options.GetAsString("username");
            if (!string.IsNullOrEmpty(username))
            {
                var password = options.GetAsString("password");
                opts.WithCredentials(username, password);
            }

            //opts.SetAutoReconnect(c.retryConnect)
            //opts.SetConnectTimeout(time.Millisecond * time.Duration(c.connectTimeout))
            //opts.SetConnectRetryInterval(time.Millisecond * time.Duration(c.reconnectTimeout))

            var client = new MqttFactory().CreateMqttClient();
            client.UseDisconnectedHandler(DisconnectedHandlerAsync);
            client.UseApplicationMessageReceivedHandler(MessageReceiveHandlerAsync);

            try
            {
                await client.ConnectAsync(opts.Build());
            }
            catch (Exception ex)
            {
                _logger.Error(context, ex, "Failed to connect to MQTT broker at " + uri);
                throw;
            }

            _connection = client;
            _clientOptions = opts.Build();

            _logger.Debug(context, "Connected to MQTT broker at " + uri);
        }

        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async Task CloseAsync(IContext context)
        {
            if (_connection == null)
            {
                return;
            }

            try
            {
                await _connection.DisconnectAsync();
            }
            finally
            {
                _connection = null;
                _subscriptions.Clear();

                _logger.Debug(context, "Disconnected from MQTT broker");
            }
        }

        private async Task DisconnectedHandlerAsync(MqttClientDisconnectedEventArgs e)
        {
            //_logger.Debug(null, "Connection failed");

            await Task.Delay(TimeSpan.FromMilliseconds(_reconnectTimeout));

            try
            {
                if (_connection != null)
                {
                    await _connection.ConnectAsync(_clientOptions);
                }
            }
            catch
            {
                // Skip...
            }
        }

        private async Task MessageReceiveHandlerAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            var message = e.ApplicationMessage;
            var topic = message.Topic;

            // Get subscriptions
            IEnumerable<MqttSubscription> subscriptions;
            lock (_lock)
            {
                subscriptions = _subscriptions.ToArray();
            }

            // Forward messages
            foreach (var subscription in subscriptions)
            {
                // Todo: Implement proper filtering by wildcards?
                if (subscription.Filter && topic != subscription.Topic)
                {
                    continue;
                }

                subscription.Listener.OnMessage(message);
            }

            await Task.Delay(0);
        }

        public IMqttClient GetConnection()
        {
            return _connection;
        }

        /// <summary>
        /// Reads names of available queues or topics.
        /// </summary>
        /// <returns>A list with queue names</returns>
        public async Task<List<string>> ReadQueueNamesAsync()
        {
            return await Task.FromResult(new List<string>());
        }

        /// <summary>
        /// Create a queue or topic with a specified name.
        /// </summary>
        /// <param name="name">A name of the queue to be created</param>
        public async Task CreateQueueAsync(string name)
        {
            await Task.Delay(0);
        }

        /// <summary>
        /// Delete a queue or topic with a specified name.
        /// </summary>
        /// <param name="name">A name of the queue to be deleted</param>
        public async Task DeleteQueueAsync(string name)
        {
            await Task.Delay(0);
        }

        private void CheckOpen()
        {
            if (_connection != null)
            {
                return;
            }

            throw new InvalidStateException(
                null,
                "NOT_OPEN",
                "Connection was not opened"
            );
        }

        public async Task PublishAsync(string topic, MqttApplicationMessage message)
        {
            CheckOpen();

            message.Topic = topic;

            await _connection.PublishAsync(message);
        }

        public async Task SubscribeAsync(string topic, MqttQualityOfServiceLevel qos, IMqttMessageListener listener)
        {
            CheckOpen();

            // Subscribe to the topic
            // Todo: Shall we skip if similar subscription already exist?
            await _connection.SubscribeAsync(topic, qos);

            lock (_lock)
            {
                var filter = topic.IndexOf("*") < 0;

                // Add subscription to the list
                var subscription = new MqttSubscription
                {
                    Topic = topic,
                    Listener = listener,
                    Filter = filter,
                    Qos = qos
                };
                _subscriptions.Add(subscription);
            }
        }

        public async Task UnsubscribeAsync(string topic, IMqttMessageListener listener)
        {
            CheckOpen();

            MqttSubscription deletedSubscription = null;
            var hasMoreSubscriptions = false;

            lock (_lock)
            {
                // Find subscription
                for (var index = 0; index < _subscriptions.Count; index++)
                {
                    var subscription = _subscriptions[index];
                    if (subscription.Topic == topic && subscription.Listener == listener)
                    {
                        deletedSubscription = subscription;
                        _subscriptions.RemoveAt(index);
                        break;
                    }
                }

                // Find subscriptions to the same topic
                foreach (var subscription in _subscriptions)
                {
                    if (subscription.Topic == topic)
                    {
                        hasMoreSubscriptions = true;
                    }
                }
            }

            // Unsubscribe if there are no more subscriptions
            if (!hasMoreSubscriptions)
            {
                await _connection.UnsubscribeAsync(topic);
            }
        }
    }
}
