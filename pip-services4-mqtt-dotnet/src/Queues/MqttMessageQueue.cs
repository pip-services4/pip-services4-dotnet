using MQTTnet;
using MQTTnet.Protocol;
using PipServices4.Commons.Convert;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Messaging.Queues;
using PipServices4.Mqtt.Connect;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PipServices4.Mqtt.Queues
{
    /// <summary>
    /// MqttMessageQueue are message queue that sends and receives messages via MQTT message broker.
    ///
    /// Configuration parameters:
    ///
    /// - topic:                         name of MQTT topic to subscribe
    /// - connection(s) :
    ///  - discovery_key:               (optional) a key to retrieve the connection from  IDiscovery
    ///  - host:                        host name or IP address
    ///  - port:                        port number
    ///  - uri:                         resource URI or connection string with all parameters in it
    /// - credential(s) :
    ///  - store_key:                   (optional) a key to retrieve the credentials from  ICredentialStore
    ///  - username:                    user name
    ///  - password:                    user password
    /// - options:
    ///  - serialize_message:    (optional) true to serialize entire message as JSON, false to send only message payload(default: true)
    ///  - autosubscribe:        (optional) true to automatically subscribe on option(default: false)
    ///  - qos:                  (optional) quality of service level aka QOS(default: 0)
    ///  - retain:               (optional) retention flag for published messages(default: false)
    ///  - retry_connect:        (optional) turns on/off automated reconnect when connection is log(default: true)
    ///  - connect_timeout:      (optional) number of milliseconds to wait for connection(default: 30000)
    ///  - reconnect_timeout:    (optional) number of milliseconds to wait on each reconnection attempt(default: 1000)
    ///  - keepalive_timeout:    (optional) number of milliseconds to ping broker while inactive(default: 3000)
    ///
    ///
    /// References:
    ///
    /// - *:logger:*:*:1.0             (optional) ILogger components to pass log messages
    /// - *:counters:*:*:1.0           (optional) ICounters components to pass collected measurements
    /// - *:discovery:*:*:1.0          (optional) IDiscovery services to resolve connections
    /// - *:credential-store:*:*:1.0   (optional) Credential stores to resolve credentials
    /// - *:connection:mqtt:*:1.0      (optional) Shared connection to MQTT service
    ///
    /// See MessageQueue
    /// See MessagingCapabilities
    ///
    /// Example:
    ///
    ///    var queue = new MqttMessageQueue("myqueue");
    ///    queue.Configure(ConfigParams.FromTuples(
    ///      "subject", "mytopic",
    ///      "connection.protocol", "mqtt"
    ///      "connection.host", "localhost"
    ///      "connection.port", 1883
    ///    ));
    ///
    ///    await queue.OpenAsync("123");
    ///
    ///    await queue.SendAsync("123", new MessageEnvelope("", "mymessage", "ABC"));
    ///
    ///    var message = await queue.ReceiveAsync("123");
    ///    if (message != null) {
    ///		...
    ///		await queue.CompleteAsync("123", message);
    ///    }
    /// </summary>
    public class MqttMessageQueue : CachedMessageQueue, IMqttMessageListener
    {
        private ConfigParams _config;
        private IReferences _references;
        private bool _opened;
        private bool _localConnection;
        private DependencyResolver _dependencyResolver = new DependencyResolver();
        private MqttConnection _connection;
        private bool _subscribed;

        private bool _serializeEnvelope;
        private string _topic;
        private int _qos;
        private bool _retain;

        public MqttMessageQueue(string name = null)
            : base(name, new MessagingCapabilities(false, true, true, true, true, false, false, false, true))
        {
            _dependencyResolver.Put("connection", new Descriptor("pip-services", "connection", "mqtt", "*", "1.0"));
        }

        /// <summary>
        /// Configure are configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">Configuration parameters to be set</param>
        public override void Configure(ConfigParams config)
        {
            base.Configure(config);
            _dependencyResolver.Configure(config);

            _config = config;

            _topic = config.GetAsStringWithDefault("topic", _topic);
            _serializeEnvelope = config.GetAsBooleanWithDefault("options.serialize_envelope", _serializeEnvelope);
            _retain = config.GetAsBooleanWithDefault("options.retain", _retain);
            _qos = config.GetAsIntegerWithDefault("options.qos", _qos);
        }

        /// <summary>
        /// SetReferences are sets references to dependent components.
        /// </summary>
        /// <param name="references">References to be set</param>
        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);

            _references = references;

            _dependencyResolver.SetReferences(references);
            // Get connection
            _connection = _dependencyResolver.GetOneOptional<MqttConnection>("connection");

            // Or create a local one
            if (_connection == null)
            {
                _connection = CreateConnection();
                _localConnection = true;
            }
            else
            {
                _localConnection = false;
            }
        }

        private MqttConnection CreateConnection()
        {
            var connection = new MqttConnection();
            if (_config != null)
            {
                connection.Configure(_config);
            }
            if (_references != null)
            {
                connection.SetReferences(_references);
            }
            return connection;
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public override bool IsOpen()
        {
            return _opened;
        }

        /// <summary>
        /// Opens the component with given connection and credential parameters.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async override Task OpenAsync(IContext context)
        {
            if (IsOpen())
            {
                return;
            }

            if (_connection == null)
            {
                _connection = CreateConnection();
                _localConnection = true;
            }

            if (_localConnection)
            {
                await _connection.OpenAsync(context);
            }

            if (!_connection.IsOpen())
            {
                throw new InvalidStateException(context != null ? ContextResolver.GetTraceId(context) : null,
                    "CONNECT_FAILED", "MQTT connection is not opened");
            }

            await base.OpenAsync(context);

            _opened = true;
        }

        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public override async Task CloseAsync(IContext context)
        {
            if (!IsOpen())
            {
                return;
            }

            if (_connection == null)
            {
                throw new InvalidStateException(context != null ? ContextResolver.GetTraceId(context) : null, 
                    "NO_CONNECTION", "MQTT connection is missing");
            }

            if (_localConnection)
            {
                await _connection.CloseAsync(context);
            }

            _opened = false;

            await base.CloseAsync(context);
        }

        private string GetTopic()
        {
            return _topic ?? Name;
        }

        protected override async Task SubscribeAsync(IContext context)
        {
            // Check if already were subscribed
            if (_subscribed)
            {
                return;
            }

            // Subscribe to the topic
            var topic = GetTopic();
            var qos = (MqttQualityOfServiceLevel)_qos;
            await _connection.SubscribeAsync(topic, qos, this);

            _subscribed = true;
        }

        protected override async Task UnsubscribeAsync(IContext context)
        {
            // Check if already were unsubscribed
            if (!_subscribed)
            {
                return;
            }

            // Unsubscribe from the topic
            var topic = GetTopic();
            await _connection.UnsubscribeAsync(topic, this);

            _subscribed = false;
        }

        private MqttApplicationMessage FromMessage(MessageEnvelope message)
        {
            var data = message.Message;
            if (_serializeEnvelope)
            {
                message.SentTime = DateTime.UtcNow;
                var json = JsonConverter.ToJson(message);
                data = Encoding.UTF8.GetBytes(json);
            }

            var qos = (MqttQualityOfServiceLevel)_qos;
            var topic = GetTopic();

            var msg = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(data)
            .WithRetainFlag(_retain)
            .WithQualityOfServiceLevel(qos)
            .Build();

            return msg;
        }

        private MessageEnvelope ToMessage(MqttApplicationMessage msg)
        {
            MessageEnvelope message;

            if (_serializeEnvelope)
            {
                var json = Encoding.UTF8.GetString(msg.Payload);
                message = JsonConverter.FromJson<MessageEnvelope>(json);
                return message;
            }
            else
            {
                message = new MessageEnvelope(null, null, msg.Payload);
            }

            if (message != null)
            {
                message.Reference = msg;
            }

            return message;
        }

        public void OnMessage(MqttApplicationMessage msg)
        {
            // Skip if it came from a wrong topic
            var expectedTopic = GetTopic();
            if (!expectedTopic.Contains("*") && expectedTopic != msg.Topic)
            {
                return;
            }

            // Deserialize message
            var message = ToMessage(msg);
            if (message == null)
            {
                _logger.Error(null, null, "Failed to read received message");
                return;
            }

            _counters.IncrementOne("queue." + Name + ".received_messages");
            _logger.Debug(Context.FromTraceId(message.TraceId), "Received message {0} via {1}", message, Name);

            if (_receiver != null)
            {
                SendMessageToReceiver(_receiver, message);
            }
            else
            {
                lock (_lock)
                {
                    _messages.Enqueue(message);
                }

                _receiveEvent.Set();
            }
        }

        private void SendMessageToReceiver(IMessageReceiver receiver, MessageEnvelope message)
        {
            var context = message == null ? null : Context.FromTraceId(message.TraceId);

            try
            {
                receiver.ReceiveMessageAsync(message, this).Wait();
            }
            catch (Exception ex)
            {
                _logger.Error(context, ex, "Failed to process the message");
            }
        }

        /// <summary>
        /// Sends a message into the queue.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="message">a message envelop to be sent.</param>
        public override async Task SendAsync(IContext context, MessageEnvelope message)
        {
            CheckOpen(context);

            _counters.IncrementOne("queue." + Name + ".sent_messages");
            _logger.Debug(Context.FromTraceId(message.TraceId), "Sent message {0} via %s", message, Name);

            var msg = FromMessage(message);

            var topic = !string.IsNullOrEmpty(Name) ? Name : _topic;

            await _connection.PublishAsync(topic, msg);
        }

        /// <summary>
        /// Renews a lock on a message that makes it invisible from other receivers in
        /// the queue.This method is usually used to extend the message processing time.
        /// </summary>
        /// <param name="message">a message to extend its lock.</param>
        /// <param name="lockTimeout">a locking timeout in milliseconds.</param>
        public override async Task RenewLockAsync(MessageEnvelope message, long lockTimeout)
        {
            // Not supported
            await Task.Delay(0);
        }

        /// <summary>
        /// Returns message into the queue and makes it available for all subscribers to
        /// receive it again.This method is usually used to return a message which could
        /// not be processed at the moment to repeat the attempt.Messages that cause
        /// unrecoverable errors shall be removed permanently or/and send to dead letter queue.
        /// </summary>
        /// <param name="message">a message to return.</param>
        public override async Task AbandonAsync(MessageEnvelope message)
        {
            // Not supported
            await Task.Delay(0);
        }

        /// <summary>
        /// Permanently removes a message from the queue. This method is usually used to
        /// remove the message after successful processing.
        /// </summary>
        /// <param name="message">a message to remove.</param>
        public override async Task CompleteAsync(MessageEnvelope message)
        {
            // Not supported
            await Task.Delay(0);
        }

        /// <summary>
        /// Permanently removes a message from the queue and sends it to dead letter queue.
        /// </summary>
        /// <param name="message">a message to be removed.</param>
        public override async Task MoveToDeadLetterAsync(MessageEnvelope message)
        {
            // Not supported
            await Task.Delay(0);
        }


    }
}
