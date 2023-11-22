﻿using PipServices4.Commons.Convert;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Http.Test.Services
{
    public class StatusRestServiceTest : IDisposable
    {
        private StatusRestService _service;

        public StatusRestServiceTest()
        {
            var config = ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.host", "localhost",
                "connection.port", "3006"
            );
            _service = new StatusRestService();
            _service.Configure(config);

            var contextInfo = new ContextInfo();
            contextInfo.Name = "Test";
            contextInfo.Description = "This is a test container";

            var references = References.FromTuples(
                new Descriptor("pip-services", "context-info", "default", "default", "1.0"), contextInfo,
                new Descriptor("pip-services", "status-service", "http", "default", "1.0"), _service
            );
            _service.SetReferences(references);

            _service.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            _service.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestStatusAsync()
        {
            Object status = await Invoke<object>("/status");
            Assert.NotNull(status);
        }

        private static async Task<T> Invoke<T>(string route)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var response = await httpClient.GetAsync("http://localhost:3006" + route);
                var responseValue = response.Content.ReadAsStringAsync().Result;
                return JsonConverter.FromJson<T>(responseValue);
            }
        }
    }
}
