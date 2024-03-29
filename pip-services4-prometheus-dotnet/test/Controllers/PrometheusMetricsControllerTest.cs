using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Prometheus.Count;
using PipServices4.Prometheus.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Prometheus.Test.Controllers
{
    public class PrometheusMetricsControllerTest : IDisposable
    {
        private PrometheusMetricsController _controller;
        private PrometheusCounters _counters;

        public PrometheusMetricsControllerTest()
        {
            var config = ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.host", "localhost",
                "connection.port", "3000"
            );
            _controller = new PrometheusMetricsController();
            _controller.Configure(config);

            var contextInfo = new ContextInfo();
            contextInfo.Name = "Test";
            contextInfo.Description = "This is a test container";

            _counters = new PrometheusCounters();

            var references = References.FromTuples(
                new Descriptor("pip-services", "context-info", "default", "default", "1.0"), contextInfo,
                new Descriptor("pip-services", "counters", "prometheus", "default", "1.0"), _counters,
                new Descriptor("pip-services", "metrics-controller", "prometheus", "default", "1.0"), _controller
            );
            _controller.SetReferences(references);


            _counters.OpenAsync(null).Wait();
            _controller.OpenAsync(null).Wait();

            Task.Delay(500).Wait();
        }

        public void Dispose()
        {
            _controller.CloseAsync(null).Wait();
            _counters.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestMetricsAsync()
        {
            _counters.IncrementOne("test.counter1");
            _counters.Stats("test.counter2", 2);
            _counters.Last("test.counter3", 3);
            _counters.TimestampNow("test.counter4");

            String status = await Invoke("/metrics");
            Assert.NotNull(status);
            Assert.True(status.Length > 0);
        }

        private static async Task<string> Invoke(string route)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var response = await httpClient.GetAsync("http://localhost:3000" + route);
                var responseValue = response.Content.ReadAsStringAsync().Result;
                return await Task.FromResult(responseValue);
            }
        }
    }
}
