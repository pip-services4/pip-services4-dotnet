using PipServices4.Components.Config;
using PipServices4.Mqtt.Queues;
using System;
using Xunit;

namespace PipServices4.Mqtt.Test.Queues
{
    public class MqttMessageQueueTest: IDisposable
    {
        private bool _enabled;
        private MqttMessageQueue _queue;
        private MessageQueueFixture _fixture;

        public MqttMessageQueueTest()
        {
            var MQTT_SERVICE_URI = Environment.GetEnvironmentVariable("MQTT_SERVICE_URI");
            var MQTT_SERVICE_HOST = Environment.GetEnvironmentVariable("MQTT_SERVICE_HOST") ?? "localhost";
            var MQTT_SERVICE_PORT = Environment.GetEnvironmentVariable("MQTT_SERVICE_PORT") ?? "1883";
            var MQTT_TOPIC = Environment.GetEnvironmentVariable("MQTT_TOPIC") ?? "test";
            var MQTT_USER = Environment.GetEnvironmentVariable("MQTT_USER"); // ?? "user";
            var MQTT_PASS = Environment.GetEnvironmentVariable("MQTT_PASS"); // ?? "pass123";

            _enabled = !string.IsNullOrEmpty(MQTT_SERVICE_URI) || !string.IsNullOrEmpty(MQTT_SERVICE_HOST);
            if (!_enabled)
            {
                return;
            }

            _queue = new MqttMessageQueue();
            _queue.Configure(ConfigParams.FromTuples(
                "topic", MQTT_TOPIC,
                "credential.protocol", "tcp",
                "connection.uri", MQTT_SERVICE_URI,
                "connection.host", MQTT_SERVICE_HOST,
                "connection.port", MQTT_SERVICE_PORT,
                "credential.username", MQTT_USER,
                "credential.password", MQTT_PASS,
                "options.autosubscribe", true,
                "options.serialize_envelope", true
            ));

            _queue.OpenAsync(null).Wait();
            _queue.ClearAsync(null).Wait();

            _fixture = new MessageQueueFixture(_queue);
        }

        public void Dispose()
        {
            if (_queue != null)
            {
                _queue.CloseAsync(null).Wait();
            }
        }

        [Fact]
        public void TestQueueSendReceiveMessage()
        {
            if (_enabled)
            {
                _fixture.TestSendReceiveMessageAsync().Wait();
            }
        }

        [Fact]
        public void TestQueueReceiveSendMessage()
        {
            if (_enabled)
            {
                _fixture.TestReceiveSendMessageAsync().Wait();
            }
        }

        [Fact]
        public void TestQueueSendPeekMessage()
        {
            if (_enabled)
                _fixture.TestSendPeekMessageAsync().Wait();
        }

        [Fact]
        public void TestQueuePeekNoMessage()
        {
            if (_enabled)
                _fixture.TestPeekNoMessageAsync().Wait();
        }

        [Fact]
        public void TestQueueOnMessage()
        {
            if (_enabled)
            {
                _fixture.TestOnMessageAsync().Wait();
            }
        }

    }
}
