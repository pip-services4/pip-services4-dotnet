using PipServices4.Commons.Convert;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;
using PipServices4.Http.Services;
using System;
using System.Net.Http;
using System.Text;
using Xunit;

namespace PipServices4.Http.Test.Controllers
{
    [Collection("Sequential")]
    public sealed class DummyHttpEndpointTest : IDisposable
    {
        private static readonly ConfigParams RestConfig = ConfigParams.FromTuples(
            "connection.protocol", "http",
            "connection.host", "localhost",
            "connection.port", 3002,
            "cors_headers", "trace_id"
        );

        private readonly DummyService _service;
        private readonly DummyCommandableHttpController _controllerV1;
        private readonly DummyCommandableHttpController _controllerV2;

        private readonly HttpEndpoint _httpEndpoint;

        public DummyHttpEndpointTest()
        {
            _service = new DummyService();
            _controllerV1 = new DummyCommandableHttpController();
            _controllerV2 = new DummyCommandableHttpController();

            _httpEndpoint = new HttpEndpoint();

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "service", "default", "default", "1.0"), _service,
                new Descriptor("pip-services4", "endpoint", "http", "default", "1.0"), _httpEndpoint
            );

            _controllerV1.Configure(ConfigParams.FromTuples(
                "base_route", "/api/v1/dummy"
            ));

            _controllerV2.Configure(ConfigParams.FromTuples(
                "base_route", "/api/v2/dummy"
            ));

            _httpEndpoint.Configure(RestConfig);

            _controllerV1.SetReferences(references);
            _controllerV2.SetReferences(references);

            _httpEndpoint.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            var task = _controllerV1.CloseAsync(null);
            task.Wait();
        }

        [Fact]
        public void It_Should_Perform_CRUD_Operations()
        {
            It_Should_Be_Opened();

            It_Should_Create_Dummy();

            It_Should_Get_Dummy();

            It_Should_Ping_Dummy();
        }

        private void It_Should_Be_Opened()
        {
            Assert.True(_httpEndpoint.IsOpen());
        }

        private void It_Should_Create_Dummy()
        {
            var newDummy = new Dummy("1", "Key 1", "Content 1");

            var result = SendPostRequest("/api/v1/dummy/create_dummy", new
            {
                dummy = newDummy
            });

            var resultDummy = JsonConverter.FromJson<Dummy>(result);

            Assert.NotNull(resultDummy);
            Assert.NotNull(resultDummy.Id);
            Assert.Equal(newDummy.Key, resultDummy.Key);
            Assert.Equal(newDummy.Content, resultDummy.Content);
        }

        private void It_Should_Get_Dummy()
        {
            var existingDummy = new Dummy("1", "Key 1", "Content 1");

            var result = SendPostRequest("/api/v1/dummy/get_dummy_by_id", new
            {
                dummy_id = existingDummy.Id
            });

            var resultDummy = JsonConverter.FromJson<Dummy>(result);

            Assert.NotNull(resultDummy);
            Assert.NotNull(resultDummy.Id);
            Assert.Equal(existingDummy.Key, resultDummy.Key);
            Assert.Equal(existingDummy.Content, resultDummy.Content);
        }

        private void It_Should_Ping_Dummy()
        {
            var result = SendPostRequest("/api/v2/dummy/ping_dummy", new { });

            Assert.False(string.IsNullOrWhiteSpace(result));

            var resultPing = JsonConverter.FromJson<bool>(result);

            Assert.True(resultPing);
        }

        private static string SendPostRequest(string route, dynamic request)
        {
            using (var httpClient = new HttpClient())
            {
                using (var content = new StringContent(JsonConverter.ToJson(request), Encoding.UTF8, "application/json"))
                {
                    var response = httpClient.PostAsync($"http://localhost:3002{route}", content).Result;

                    return response.Content.ReadAsStringAsync().Result;
                }
            }
        }

    }
}
