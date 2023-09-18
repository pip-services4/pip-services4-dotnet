using PipServices4.Components.Config;
using PipServices4.Config.Connect;
using System.Linq;
using Xunit;

namespace PipServices4.Config.test.Connect
{
	public sealed class MemoryDiscoveryTest
    {
        private ConfigParams config = ConfigParams.FromTuples(
            "connections.key1.host", "10.1.1.100",
            "connections.key1.port", "8080",
            "connections.key2.host", "10.1.1.101",
            "connections.key2.port", "8082"
        );

        [Fact]
        public async void TestResolveConnections()
        {
            var discovery = new MemoryDiscovery();
            discovery.Configure(config);

            // Resolve one
            var connection = await discovery.ResolveOneAsync("123", "key1");

            Assert.Equal("10.1.1.100", connection.Host);
            Assert.Equal(8080, connection.Port);

            connection = await discovery.ResolveOneAsync("123", "key2");

            Assert.Equal("10.1.1.101", connection.Host);
            Assert.Equal(8082, connection.Port);


            // Resolve all
            await discovery.RegisterAsync(null, "key1",
                ConnectionParams.FromTuples("host", "10.3.3.151")
            );

            var connections = await discovery.ResolveAllAsync("123", "key1");

            Assert.True(connections.Count > 1);
        }
    }
}
