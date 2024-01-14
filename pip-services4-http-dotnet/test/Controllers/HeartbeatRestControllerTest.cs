using PipServices4.Commons.Convert;
using PipServices4.Components.Config;
using PipServices4.Http.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Http.Test.Controllers
{
    public class HeartbeatRestControllerTest: IDisposable
    {
        private HeartbeatRestController _controller;

        public HeartbeatRestControllerTest()
        {
            var config = ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.host", "localhost",
                "connection.port", "3005"
            );
            _controller = new HeartbeatRestController();
            _controller.Configure(config);

            _controller.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            _controller.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestHeartbeatAsync() 
        {
            DateTime? time = await Invoke<DateTime?>("/heartbeat");
            Assert.NotNull(time);
            Assert.True((DateTime.UtcNow - time.Value).TotalMilliseconds < 1000);
        }

        private static async Task<T> Invoke<T>(string route)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var response = await httpClient.GetAsync("http://localhost:3005" + route);
                var responseValue = response.Content.ReadAsStringAsync().Result;
                return JsonConverter.FromJson<T>(responseValue);
            }
        }
    }
}
