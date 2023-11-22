using PipServices4.Commons.Convert;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Data.Keys;
using PipServices4.Data.Query;
using PipServices4.Http.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Http.Test.Services
{
    [Collection("Sequential")]
    public class DummyRestServiceV2Test : IDisposable
    {
        private static int testPort = 3004;

        private static readonly ConfigParams restConfig = ConfigParams.FromTuples(
            "connection.protocol", "http",
            "connection.host", "localhost",
            "connection.port", testPort,
            "swagger.enable", "true",
            "swagger.content", "swagger yaml or json content"  // for test only
        );

        private DummyRestServiceV2 _service;
        private HttpClient _httpClient;

        private readonly Dummy DUMMY1 = new(null, "Key 1", "Content 1");
        private readonly Dummy DUMMY2 = new(null, "Key 2", "Content 2");

        public DummyRestServiceV2Test()
        {
            _httpClient = new HttpClient();
            _service = new DummyRestServiceV2();

            var ctrl = new DummyController();

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "controller", "default", "default", "1.0"), ctrl,
                new Descriptor("pip-services-dummies", "service", "rest", "default", "1.0"), _service
            );

            _service.Configure(restConfig);
            _service.SetReferences(references);

            _service.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            _service.CloseAsync(null).Wait();
        }

        [Fact(Skip = "TODO: Doesn't work")]
        public async Task Test_CRUD_OperationsAsync()
        {
            // Create one dummy
            var result = await SendRequestAsync("post", "/dummies", new
            {
                dummy=DUMMY1
            });

            var dummy = JsonConverter.FromJson<Dummy>(result);
            Assert.NotNull(dummy);
            Assert.NotNull(dummy.Id);
            Assert.Equal(dummy.Key, DUMMY1.Key);
            Assert.Equal(dummy.Content, DUMMY1.Content);

            var dummy1 = dummy;

            // Create another dummy
            result = await SendRequestAsync("post", "/dummies", new
            {
                dummy = DUMMY2
            });

            dummy = JsonConverter.FromJson<Dummy>(result);
            Assert.NotNull(dummy);
            Assert.NotNull(dummy.Id);
            Assert.Equal(dummy.Key, DUMMY2.Key);
            Assert.Equal(dummy.Content, DUMMY2.Content);

            // Get all dummies
            result = await SendRequestAsync("get", "/dummies", new
            {
            });

            var page = JsonConverter.FromJson<DataPage<Dummy>>(result);

            Assert.NotNull(page);
            Assert.NotNull(page.Data);
            Assert.Equal(2, page.Data.Count());

            // Update the dummy
            dummy1.Content = "Updated Content 1";

            result = await SendRequestAsync("put", "/dummies", dummy1);

            dummy = JsonConverter.FromJson<Dummy>(result);

            Assert.NotNull(dummy);
            Assert.NotNull(dummy.Id);
            Assert.Equal(dummy.Key, dummy1.Key);
            Assert.Equal(dummy.Content, dummy1.Content);

            dummy1 = dummy;

            // Delete dummy
            result = await SendRequestAsync("delete", $"/dummies/{dummy1.Id}", new
            {
            });

            var deletedDummy = JsonConverter.FromJson<Dummy>(result);

            Assert.NotNull(deletedDummy);
            Assert.NotNull(deletedDummy.Id);
            Assert.Equal(dummy1.Key, deletedDummy.Key);
            Assert.Equal(dummy1.Content, deletedDummy.Content);

            result = await SendRequestAsync("get", $"/dummies/{dummy1.Id}", new
            {
            });

            Assert.Empty(result);

            // Check interceptor
            Assert.Equal(6, _service.GetNumberOfCalls());

            // Failed validation
            result = await SendRequestAsync("post", "/dummies", new
            {
            });

            var res = JsonConverter.FromJson<IDictionary<string, object>>(result);
            Assert.Equal("INVALID_DATA", res["code"]);
        }

        [Fact(Skip = "TODO: Doesn't work")]
        public async Task Test_Check_TraceIdAsync()
        {
            // check transmit correllationId over params
            var result = await SendRequestAsync("get", "/dummies/check/trace_id?trace_id=test_trace_id", new
            {
            });

            var traceId = JsonConverter.FromJson<string>(result);

            Assert.Equal("test_trace_id", traceId);

            // check transmit correllationId over header
            _httpClient.DefaultRequestHeaders.Add("trace_id", "test_trace_id_header");
            result = await SendRequestAsync("get", "/dummies/check/trace_id", new
            {
            });

            traceId = JsonConverter.FromJson<string>(result);

            Assert.Equal(2, _service.GetNumberOfCalls()); // Check interceptor
            Assert.Equal("test_trace_id_header", traceId);
        }

        [Fact]
        public async Task Get_OpenApi_Spec_From_String()
        {
            // check transmit correllationId over params
            var result = await SendRequestAsync("get", "/swagger", new
            {
            });

            var openApiContent = restConfig.GetAsString("swagger.content");
            Assert.Equal(openApiContent, result);
        }

        [Fact]
        public async Task Get_OpenApi_Spec_From_File() 
        {
            var openApiContent = "swagger yaml content from file";
            var filename = "dummy_" + IdGenerator.NextLong() + ".tmp";

            try
            {
                // create temp file
                using (FileStream fstream = new FileStream(filename, FileMode.Create))
                {
                    byte[] buffer = Encoding.Default.GetBytes(openApiContent);
                    await fstream.WriteAsync(buffer, 0, buffer.Length);
                }

                // recreate service with new configuration
                await _service.CloseAsync(null);

                var serviceConfig = ConfigParams.FromTuples(
                    "connection.protocol", "http",
                    "connection.host", "localhost",
                    "connection.port", testPort,
                    "swagger.enable", "true",
                    "swagger.path", filename  // for test only
                );

                var ctrl = new DummyController();
                _service = new DummyRestServiceV2();
                _service.Configure(serviceConfig);

                References references = References.FromTuples(
                    new Descriptor("pip-services4-dummies", "controller", "default", "default", "1.0"), ctrl,
                    new Descriptor("pip-services-dummies", "service", "rest", "default", "1.0"), _service
                );

                _service.SetReferences(references);
                await _service.OpenAsync(null);

                var content = await SendRequestAsync("get", "/swagger", new { });

                Assert.Equal(openApiContent, content);
            } finally
            {
                // delete temp file
                File.Delete(filename);
            }
        }



        private async Task<string> SendRequestAsync(string method, string route, dynamic request, bool formData = false)
        {
            HttpContent content;
            if (formData)
            {
                content = new MultipartFormDataContent()
                {
                    {new StringContent(JsonConverter.ToJson(request)), "file", "test_file.json"}
                };
            }
            else
            {
                content = new StringContent(JsonConverter.ToJson(request), Encoding.UTF8, "application/json");
            }

            var response = new HttpResponseMessage();

                switch (method)
                {
                    case "get":
                        response = await _httpClient.GetAsync($"http://localhost:{testPort}{route}");
                        break;
                    case "post":
                        response = await _httpClient.PostAsync($"http://localhost:{testPort}{route}", content);
                        break;
                    case "put":
                        response = await _httpClient.PutAsync($"http://localhost:{testPort}{route}", content);
                        break;
                    case "delete":
                        response = await _httpClient.DeleteAsync($"http://localhost:{testPort}{route}");
                        break;
                }

            return await response.Content.ReadAsStringAsync();
        }
    }
}