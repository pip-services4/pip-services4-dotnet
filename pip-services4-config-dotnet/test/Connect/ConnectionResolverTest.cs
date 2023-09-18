using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Config.Connect;
using System;
using System.Linq;
using Xunit;

namespace PipServices4.Config.test.Connect
{
	//[TestClass]
	public sealed class ConnectionResolverTest
    {
        private static readonly ConfigParams RestConfig = ConfigParams.FromTuples(
            "connection.protocol", "http",
            "connection.host", "localhost",
            "connection.port", 3000
        );

        private ConnectionResolver _connectionResolver;

        public ConnectionResolverTest()
        {
            _connectionResolver = new ConnectionResolver(RestConfig);
            _connectionResolver.SetReferences(new References());
        }

        [Fact]
        public void TestConfigure()
        {
            var config = _connectionResolver.GetAll().FirstOrDefault();
            Assert.Equal("http", config.Get("protocol"));
            Assert.Equal("localhost", config.Get("host"));
            Assert.Equal("3000", config.Get("port"));
        }

        [Fact]
        public void TestRegister()
        {
            var connectionParams = new ConnectionParams();
            _connectionResolver.RegisterAsync("correlationId", connectionParams).Wait();
            var configList = _connectionResolver.GetAll();

            Assert.Single(configList);

            connectionParams.DiscoveryKey = "Discovery key value";
            _connectionResolver.RegisterAsync("correlationId", connectionParams).Wait();
            configList = _connectionResolver.GetAll();

            Assert.Equal(2, configList.Count());

            _connectionResolver.RegisterAsync("correlationId", connectionParams).Wait();
            configList = _connectionResolver.GetAll();
            var configFirst = configList.FirstOrDefault();
            var configLast = configList.LastOrDefault();

            Assert.Equal(3, configList.Count());
            Assert.Equal("http", configFirst.Get("protocol"));
            Assert.Equal("localhost", configFirst.Get("host"));
            Assert.Equal("3000", configFirst.Get("port"));
            Assert.Equal("Discovery key value", configLast.Get("discovery_key"));
        }

        [Fact]
        public void TestResolve()
        {
            var connectionParams = _connectionResolver.ResolveAsync("correlationId").Result;

            Assert.Equal("http", connectionParams.Get("protocol"));
            Assert.Equal("localhost", connectionParams.Get("host"));
            Assert.Equal("3000", connectionParams.Get("port"));

            var restConfigDiscovery = ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.host", "localhost",
                "connection.port", 3000,
                "connection.discovery_key", "Discovery key value"
            );

            IReferences references = new References();
            _connectionResolver = new ConnectionResolver(restConfigDiscovery, references);
            try
            {
                _connectionResolver.ResolveAsync("correlationId").Wait();
            }
            catch (Exception ex)
            {
                Assert.IsType<ConfigException>(ex.InnerException);
            }
        }
    }
}
