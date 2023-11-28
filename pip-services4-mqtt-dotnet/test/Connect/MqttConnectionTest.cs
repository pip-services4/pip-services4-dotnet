using PipServices4.Components.Config;
using PipServices4.Mqtt.Connect;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Mqtt.Test.Connect
{
    public class MqttConnectionTest : IDisposable
    {
        private bool _enabled;
        private MqttConnection _connection;

        public MqttConnectionTest()
        {
            var MQTT_SERVICE_URI = Environment.GetEnvironmentVariable("MQTT_SERVICE_URI");
            var MQTT_SERVICE_HOST = Environment.GetEnvironmentVariable("MQTT_SERVICE_HOST") ?? "localhost";
            var MQTT_SERVICE_PORT = Environment.GetEnvironmentVariable("MQTT_SERVICE_PORT") ?? "1883";
            var MQTT_TOPIC = Environment.GetEnvironmentVariable("MQTT_TOPIC") ?? "test";
            var MQTT_USER = Environment.GetEnvironmentVariable("MQTT_USER") ?? "user";
            var MQTT_PASS = Environment.GetEnvironmentVariable("MQTT_PASS") ?? "pass123";

            _enabled = !string.IsNullOrEmpty(MQTT_SERVICE_URI) || !string.IsNullOrEmpty(MQTT_SERVICE_HOST);
            if (!_enabled)
            {
                return;
            }

            _connection = new MqttConnection();
            _connection.Configure(ConfigParams.FromTuples(
                "connection.uri", MQTT_SERVICE_URI,
                "connection.protocol", "tcp",
                "connection.host", MQTT_SERVICE_HOST,
                "connection.port", MQTT_SERVICE_PORT,
                "credential.username", MQTT_USER,
                "credential.password", MQTT_PASS
            ));
        }

        public void Dispose()
        {
        }

        [Fact]
        public async Task TestConnectionOpenCloseAsync()
        {
            if (_enabled)
            {
                await _connection.OpenAsync(null);

                Assert.True(_connection.IsOpen());
                Assert.NotNull(_connection.GetConnection());

                await _connection.CloseAsync(null);

                Assert.False(_connection.IsOpen());
                Assert.Null(_connection.GetConnection());
            }
        }

    }
}
