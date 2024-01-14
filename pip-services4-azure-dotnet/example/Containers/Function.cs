﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PipServices4.Components.Config;
using System;
using System.Threading.Tasks;

namespace PipServices4.Azure.Containers
{
    public static class FunctionContainer
    {
        public static DummyAzureFunction _functionService;
        public static Func<HttpRequest, Task<IActionResult>> _handler;

        [FunctionName("FunctionContainer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var config = ConfigParams.FromTuples(
                "logger.descriptor", "pip-services:logger:console:default:1.0"
            );

            if (_handler == null)
            {
                _functionService = new DummyAzureFunction();
                _functionService.Configure(config);
                await _functionService.OpenAsync(null);

                _handler = _functionService.GetHandler();
            }

            return await _handler(req);
        }
    }
}
