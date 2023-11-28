using PipServices4.Components.Config;
using PipServices4.Mqtt.Connect;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Mqtt.Test.Connect
{
    public class MqttConnectionResolverTest: IDisposable
    {

		public void Dispose()
		{ }

        [Fact]
        public async Task TestSingleConnectionAsync()
        {
			var resolver = new MqttConnectionResolver();
			resolver.Configure(ConfigParams.FromTuples(
				"connection.protocol", "tcp",
				"connection.host", "localhost",
				"connection.port", 1883
			));

			var connection = await resolver.ResolveAsync(null);
			Assert.Equal("localhost:1883", connection.GetAsString("servers"));
			Assert.Null(connection.GetAsString("username"));
			Assert.Null(connection.GetAsString("password"));
		}

		[Fact]
		public async Task TestSingleConnectionWithAuthAsync()
        {
			var resolver = new MqttConnectionResolver();
			resolver.Configure(ConfigParams.FromTuples(
				"connection.protocol", "tcp",
				"connection.host", "localhost",
				"connection.port", 1883,
				"credential.username", "test",
				"credential.password", "pass123"
			));

			var connection = await resolver.ResolveAsync(null);
			Assert.Equal("localhost:1883", connection.GetAsString("servers"));
			Assert.Equal("test", connection.GetAsString("username"));
			Assert.Equal("pass123", connection.GetAsString("password"));
		}

		[Fact]
		public async Task TestClusterConnectionAsync()
        {
			var resolver = new MqttConnectionResolver();
			resolver.Configure(ConfigParams.FromTuples(
				"connections.0.protocol", "tcp",
				"connections.0.host", "server1",
				"connections.0.port", 1883,
				"connections.1.protocol", "tcp",
				"connections.1.host", "server2",
				"connections.1.port", 1883,
				"connections.2.protocol", "tcp",
				"connections.2.host", "server3",
				"connections.2.port", 1883
			));

			var connection = await resolver.ResolveAsync(null);
			Assert.Equal("server1:1883,server2:1883,server3:1883", connection.GetAsString("servers"));
			Assert.Null(connection.GetAsString("username"));
			Assert.Null(connection.GetAsString("password"));
		}

		[Fact]
		public async Task TestClusterConnectionWithAuthAsync()
		{
			var resolver = new MqttConnectionResolver();
			resolver.Configure(ConfigParams.FromTuples(
				"connections.0.protocol", "tcp",
				"connections.0.host", "server1",
				"connections.0.port", 1883,
				"connections.1.protocol", "tcp",
				"connections.1.host", "server2",
				"connections.1.port", 1883,
				"connections.2.protocol", "tcp",
				"connections.2.host", "server3",
				"connections.2.port", 1883,
				"credential.username", "test",
				"credential.password", "pass123"
			));

			var connection = await resolver.ResolveAsync(null);
			Assert.Equal("server1:1883,server2:1883,server3:1883", connection.GetAsString("servers"));
			Assert.Equal("test", connection.GetAsString("username"));
			Assert.Equal("pass123", connection.GetAsString("password"));
		}

		[Fact]
		public async Task TestUriConnectionWithAuthAsync()
		{
			var resolver = new MqttConnectionResolver();
			resolver.Configure(ConfigParams.FromTuples(
				"connection.uri", "tcp://test:pass123@server1:1883,server2:1883,server3:1883?param1=ABC"
			));

			var connection = await resolver.ResolveAsync(null);
			Assert.Equal("server1:1883,server2:1883,server3:1883", connection.GetAsString("servers"));
			Assert.Equal("test", connection.GetAsString("username"));
			Assert.Equal("pass123", connection.GetAsString("password"));
		}
	}
}
