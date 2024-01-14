using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Http.Services;
using PipServices4.Observability.Log;
using System;

namespace PipServices4.Http
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new DummyService();
            //var service = new DummyCommandableHttpService();
            var controller = new DummyRestController();
            var logger = new ConsoleLogger();

            controller.Configure(ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.host", "localhost",
                "connection.port", 3000,
                "swagger.enable", "true"
            ));

            controller.SetReferences(References.FromTuples(
                new Descriptor("pip-services4-dummies", "service", "default", "default", "1.0"), service,
                new Descriptor("pip-services4-dummies", "controller", "rest", "default", "1.0"), controller,
                new Descriptor("pip-services4-commons", "logger", "console", "default", "1.0"), logger
            ));

            controller.OpenAsync(null).Wait();

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
