using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Grpc;
using PipServices4.Grpc.Services;
using PipServices4.Observability.Log;
using System;

namespace PipServices4.Grpc
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new DummyController();
            //var service = new DummyCommandableHttpService();
            var service = new DummyGrpcService();
            var logger = new ConsoleLogger();

            service.Configure(ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.host", "localhost",
                "connection.port", 3000,
                "swagger.enable", "true"
            ));

            service.SetReferences(References.FromTuples(
                new Descriptor("pip-services4-dummies", "controller", "default", "default", "1.0"), controller,
                new Descriptor("pip-services4-dummies", "service", "rest", "default", "1.0"), service,
                new Descriptor("pip-services4-commons", "logger", "console", "default", "1.0"), logger
            ));

            service.OpenAsync(null).Wait();

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
