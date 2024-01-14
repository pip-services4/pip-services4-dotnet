using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Data.Validate;
using PipServices4.Http.Auth;
using PipServices4.Http.Controllers;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TypeCode = PipServices4.Commons.Convert.TypeCode;

namespace PipServices4.Http.Services
{
    public class DummyRestControllerV2 : RestController
    {
        private DummyRestOperations _operations = new DummyRestOperations();
        private int _numberOfCalls = 0;

        private string _swaggerContent;
        private string _swaggerPath;

        public override void Configure(ConfigParams config)
        {
            base.Configure(config);

            _swaggerContent = config.GetAsNullableString("swagger.content");
            _swaggerPath = config.GetAsNullableString("swagger.path");
        }

        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);

            _operations.SetReferences(references);
        }

        public int GetNumberOfCalls()
        {
            return _numberOfCalls;
        }

        private async Task IncrementNumberOfCallsAsync(HttpRequest req, HttpResponse res, ClaimsPrincipal user, RouteData rd,
            Func<HttpRequest, HttpResponse, ClaimsPrincipal, RouteData, Task> next)
        {
            _numberOfCalls++;
            await next(req, res, user, rd);
        }

        public override void Register()
        {
            var auth = new BasicAuthorizer();

            RegisterInterceptor("", IncrementNumberOfCallsAsync);

            RegisterRouteWithAuth(
                "get", "/dummies/check/trace_id",
                new ObjectSchema(),

                auth.Anybody(),
                _operations.CheckTraceId
            );

            RegisterRouteWithAuth("get", "/dummies",
                new ObjectSchema()
                    .WithOptionalProperty("skip", TypeCode.String)
                    .WithOptionalProperty("take", TypeCode.String)
                    .WithOptionalProperty("total", TypeCode.String)
                    .WithOptionalProperty("body", new FilterParamsSchema()),

                auth.Anybody(),
                _operations.GetPageByFilterAsync
            );

            RegisterRouteWithAuth("get", "/dummies/{id}",
                new ObjectSchema()
                    .WithOptionalProperty("dummy_id", TypeCode.String),

                auth.Anybody(),
                _operations.GetByIdAsync
            );

            RegisterRouteWithAuth("post", "/dummies",
                new ObjectSchema()
                    .WithRequiredProperty("body", new ObjectSchema().WithRequiredProperty("dummy", new DummySchema())),

                auth.Anybody(),
                _operations.CreateAsync
            );


            RegisterRouteWithAuth("post", "/dummies/file",
                new ObjectSchema()
                    .WithRequiredProperty("body", new ObjectSchema().WithRequiredProperty("dummy", new DummySchema())),

                auth.Anybody(),
                _operations.CreateFromFileAsync
            );

            RegisterRouteWithAuth("put", "/dummies",
                new ObjectSchema()
                    .WithRequiredProperty("body", new DummySchema()),

                auth.Anybody(),
                _operations.UpdateAsync
            );

            RegisterRouteWithAuth("put", "/dummies/{id}",
                new ObjectSchema()
                    .WithRequiredProperty("id", TypeCode.String)
                    .WithRequiredProperty("body", new DummySchema()),

                auth.Anybody(),
                _operations.UpdateAsync
            );

            RegisterRouteWithAuth("delete", "/dummies/{id}",
                new ObjectSchema()
                    .WithRequiredProperty("id", TypeCode.String),

                auth.Anybody(),
                _operations.DeleteByIdAsync
            );

            if (!string.IsNullOrWhiteSpace(_swaggerContent))
                RegisterOpenApiSpec(_swaggerContent);

            if (!string.IsNullOrWhiteSpace(_swaggerPath))
                RegisterOpenApiSpecFromFile(_swaggerPath);
        }
    }
}