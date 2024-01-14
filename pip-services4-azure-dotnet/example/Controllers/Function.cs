﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PipServices4.Components.Config;
using System;
using System.Threading.Tasks;

namespace PipServices4.Azure.Controllers
{
    public static class Function
    {
        public static DummyAzureFunction _functionService;
        public static Func<HttpRequest, Task<IActionResult>> _handler;

        [FunctionName("Function")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var config = ConfigParams.FromTuples(
                "logger.descriptor", "pip-services:logger:console:default:1.0",
                "service.descriptor", "pip-services-dummies:service:default:default:1.0",
                "controller.descriptor", "pip-services-dummies:controller:azure-function:default:1.0"
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
