using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Http.Clients;
using PipServices4.Http.Services;
using PipServices4.Http.Test.Fixtures;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Http.Test.Clients
{
    [Collection("Sequential")]
    public sealed class DummyCommandableHttpClientTest : IDisposable
    {
        private static readonly ConfigParams RestConfig = ConfigParams.FromTuples(
            "connection.uri", "http://localhost:3000",
            "options.timeout", 15000,
            "options.trace_id_place", "query"
            //"connection.protocol", "http",
            //"connection.host", "localhost",
            //"connection.port", 3000
        );

        private readonly DummyService _service;
        private readonly DummyCommandableHttpClient _client;
        private readonly DummyClientFixture _fixture;
        private readonly CancellationTokenSource _source;

        private readonly DummyCommandableHttpController _controller;

        public DummyCommandableHttpClientTest()
        {
            _service = new DummyService();

            _controller = new DummyCommandableHttpController();

            _client = new DummyCommandableHttpClient();

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "service", "default", "default", "1.0"), _service,
                new Descriptor("pip-services4-dummies", "controller", "rest", "default", "1.0"), _controller,
                new Descriptor("pip-services4-dummies", "client", "rest", "default", "1.0"), _client
            );
            _controller.Configure(RestConfig);
            _client.Configure(RestConfig);

            _client.SetReferences(references);
            _controller.SetReferences(references);

            _controller.OpenAsync(null).Wait();

            _fixture = new DummyClientFixture(_client);

            _source = new CancellationTokenSource();

            _client.OpenAsync(null).Wait();
        }

        [Fact]
        public void TestCrudOperations()
        {
            var task = _fixture.TestCrudOperations();
            task.Wait();
        }

        [Fact]
        public async Task TestExceptionPropagationAsync()
        {
            var errorTxt = "";
            try
            {
                await _client.RaiseExceptionAsync(Context.FromTraceId("123"));
            }
            catch (Exception ex)
            {
                errorTxt = ex.Message;
            }

            Assert.Equal("Dummy error in controller!", errorTxt);
        }

        public void Dispose()
        {
            var task = _client.CloseAsync(null);
            task.Wait();

            task = _controller.CloseAsync(null);
            task.Wait();
        }
    }
}
