using PipServices4.Config.Connect;
using Xunit;

namespace PipServices4.Config.test.Connect
{
    //[TestClass]
    public sealed class ConnectionParamsTest
    {
        [Fact]
        public void TestDiscovery()
        {
            var connection = new ConnectionParams();
            connection.DiscoveryKey = null;
            Assert.Null(connection.DiscoveryKey);

            connection.DiscoveryKey = "Discovery key value";
            Assert.Equal("Discovery key value", connection.DiscoveryKey);
            Assert.True(connection.UseDiscovery);
        }

        [Fact]
        public void TestProtocol()
        {
            var connection = new ConnectionParams();
            connection.Protocol = null;
            Assert.Equal("http", connection.Protocol);
            Assert.Null(connection.GetProtocol());
            Assert.Equal("https", connection.GetProtocolWithDefault("https"));
            connection.Protocol = "https";

            Assert.Equal("https", connection.Protocol);
        }

        [Fact]
        public void TestHost()
        {
            var connection = new ConnectionParams();
            Assert.Equal("localhost", connection.Host);

            connection.Host = null;
            Assert.Equal("localhost", connection.Host);

            connection.Host = "localhost1";
            Assert.Equal("localhost1", connection.Host);
        }

        [Fact]
        public void TestPort()
        {
            var connection = new ConnectionParams();
            Assert.Equal(8080, connection.Port);

            connection.Port = 3000;
            Assert.Equal(3000, connection.Port);
        }

        [Fact]
        public void TestUri()
        {
            var connection = new ConnectionParams();
            Assert.Null(connection.Uri);

            connection.Protocol = "https";
            connection.Port = 3000;
            connection.Host = "pipgoals";
            Assert.Null(connection.Uri);
        }
    }
}
