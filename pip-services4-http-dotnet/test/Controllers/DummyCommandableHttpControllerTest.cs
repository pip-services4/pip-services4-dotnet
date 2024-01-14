using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Http.Services;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Http.Test.Controllers
{
    [Collection("Sequential")]
    public sealed class DummyCommandableHttpControllerTest
    {
        private static readonly ConfigParams RestConfig = ConfigParams.FromTuples(
            "connection.uri", "http://localhost:3000",
            "options.timeout", 15000,
            "swagger.enable", "true"
            //"connection.protocol", "http",
            //"connection.host", "localhost",
            //"connection.port", 3000
        );

        public DummyCommandableHttpControllerTest()
        {
        }

        private DummyCommandableHttpController CreateAndOpenController(ConfigParams config)
        {
            var controller = new DummyCommandableHttpController();

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "service", "default", "default", "1.0"), new DummyService(),
                new Descriptor("pip-services4-dummies", "controller", "rest", "default", "1.0"), controller
            );

            controller.Configure(config);
            controller.SetReferences(references);

            controller.OpenAsync(null).Wait();

            return controller;
        }
        
        [Fact]
        public async Task TestOpenApiAsync()
        {
            DummyCommandableHttpController service = CreateAndOpenController(RestConfig);

            try
            {
                var serviceUri = RestConfig.GetAsString("connection.uri");

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{serviceUri}/dummy/swagger");
                var openApiContent = await response.Content.ReadAsStringAsync();

                Assert.StartsWith("openapi:", openApiContent);
            }
            finally
            {
                await service.CloseAsync(null);
            }
		}

        [Fact]
        public async Task TestOpenApiOverrideAsync()
        {
            // turn off and override standard swagger document formation
            var config = RestConfig.SetDefaults(ConfigParams.FromTuples("swagger.auto", false));

            DummyCommandableHttpController controller = CreateAndOpenController(config);

            try
            {
                var serviceUri = RestConfig.GetAsString("connection.uri");

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{serviceUri}/dummy/swagger");
                var openApiContent = await response.Content.ReadAsStringAsync();

                Assert.StartsWith("swagger yaml content from resource", openApiContent);
            }
            finally
            {
                await controller.CloseAsync(null);
            }
        }
    }
}
