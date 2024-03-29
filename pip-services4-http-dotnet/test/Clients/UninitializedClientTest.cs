using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Http.Clients;
using PipServices4.Http.Services;
using Xunit;

namespace PipServices4.Http.Test.Clients
{
    [Collection("Sequential")]
    public sealed class UninitializedClient
    {
        [Fact]
        public void TestImproperProtocol()
        {
            var restConfig = ConfigParams.FromTuples(
                "connection.protocol", "ftp"
            );

            DummyService _service;
            DummyCommandableHttpClient _client;
            DummyCommandableHttpController _controller;


            _service = new DummyService();

            _controller = new DummyCommandableHttpController();

            _client = new DummyCommandableHttpClient();

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "service", "default", "default", "1.0"), _service,
                new Descriptor("pip-services4-dummies", "controller", "rest", "default", "1.0"), _controller,
                new Descriptor("pip-services4-dummies", "client", "rest", "default", "1.0"), _client
            );
            _controller.Configure(restConfig);
            _client.Configure(restConfig);

            _client.SetReferences(references);
            _controller.SetReferences(references);

            _controller.OpenAsync(null).Wait();


            var response = Record.ExceptionAsync(async () => await _client.OpenAsync(null));

            Assert.NotNull(response?.Result);
            var exception = Assert.IsType<ConfigException>(response.Result);
            Assert.Equal("WRONG_PROTOCOL", exception.Code);



            var task = _client.CloseAsync(null);
            task.Wait();

            task = _controller.CloseAsync(null);
            task.Wait();
        }

        [Fact]
        public void TestNoHostPortUriSet()
        {
            var restConfig = ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.port", "0"          // default is 8080
                );

            DummyService _ctrl;
            DummyCommandableHttpClient _client;

            DummyCommandableHttpController _service;


            _ctrl = new DummyService();

            _service = new DummyCommandableHttpController();

            _client = new DummyCommandableHttpClient();

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "controller", "default", "default", "1.0"), _ctrl,
                new Descriptor("pip-services4-dummies", "service", "rest", "default", "1.0"), _service,
                new Descriptor("pip-services4-dummies", "client", "rest", "default", "1.0"), _client
            );
            _service.Configure(restConfig);
            _client.Configure(restConfig);

            _client.SetReferences(references);
            _service.SetReferences(references);

            _service.OpenAsync(null).Wait();

            
            var response = Record.ExceptionAsync(async () => await _client.OpenAsync(null));
            


            Assert.NotNull(response?.Result);
            var exception = Assert.IsType<ConfigException>(response.Result);
            Assert.Equal("NO_PORT", exception.Code);


            var task = _client.CloseAsync(null);
            task.Wait();

            task = _service.CloseAsync(null);
            task.Wait();
        }

    }
}
