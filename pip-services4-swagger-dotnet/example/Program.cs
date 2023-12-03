using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;
using PipServices4.Observability.Log;
using PipServices4.Swagger.Services;
using System;

namespace PipServices4.Swagger
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new DummyController();
            var service = new DummyCommandableHttpService();
            //var service = new DummyRestService();
            var logger = new ConsoleLogger();
            var endpoint = new HttpEndpoint();
            var swagger = new SwaggerService();

            var config = ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.host", "localhost",
                "connection.port", 3000,
                "swagger.auto", "true",
                "swagger.enable", "true"
            );

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "controller", "default", "default", "1.0"), controller,
                new Descriptor("pip-services4-dummies", "service", "rest", "default", "1.0"), service,
                new Descriptor("pip-services", "logger", "console", "default", "1.0"), logger,
                new Descriptor("pip-services", "endpoint", "http", "default", "1.0"), endpoint,
                new Descriptor("pip-services", "swagger-service", "default", "default", "1.0"), swagger
            );

            endpoint.Configure(config);
            service.Configure(config);

            swagger.SetReferences(references);
            service.SetReferences(references);

            endpoint.OpenAsync(null).Wait();
            service.OpenAsync(null).Wait();

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
