using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Http.Auth;
using PipServices4.Http.Controllers;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PipServices4.Swagger.Services
{
    public class DummyRestService : RestService
    {
        private DummyRestOperations _operations = new DummyRestOperations();
        private int _numberOfCalls = 0;
        private string _openApiContent;
        private string _openApiFile;
        private string _openApiResource;

        public override void Configure(ConfigParams config)
		{
			base.Configure(config);

            _openApiContent = config.GetAsNullableString("openapi_content");
            _openApiFile = config.GetAsNullableString("openapi_file");
            _openApiResource = config.GetAsNullableString("openapi_resource");
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
            
            RegisterRouteWithAuth("get", "/dummies", auth.Anybody(),
                async (request, response, user, routeData) =>
                {
                    await _operations.GetPageByFilterAsync(request, response, user, routeData);
                });
            
            RegisterRouteWithAuth("get", "/dummies/{id}", auth.Anybody(),
                async (request, response, user, routeData) =>
                {
                    await _operations.GetByIdAsync(request, response, user, routeData);
                });
            
            RegisterRouteWithAuth("post", "/dummies", auth.Anybody(),
                async (request, response, user, routeData) =>
                {
                    await _operations.CreateAsync(request, response, user, routeData);
                });
            
            RegisterRouteWithAuth("post", "/dummies/file", auth.Anybody(),
                async (request, response, user, routeData) =>
                {
                    await _operations.CreateFromFileAsync(request, response, user, routeData);
                });
            
            RegisterRouteWithAuth("put", "/dummies", auth.Anybody(),
                async (request, response, user, routeData) =>
                {
                    await _operations.UpdateAsync(request, response, user, routeData);
                });
            
            RegisterRouteWithAuth("put", "/dummies/{id}", auth.Anybody(),
                async (request, response, user, routeData) =>
                {
                    await _operations.UpdateAsync(request, response, user, routeData);
                });
            
            RegisterRouteWithAuth("delete", "/dummies/{id}", auth.Anybody(),
                async (request, response, user, routeData) =>
                {
                    await _operations.DeleteByIdAsync(request, response, user, routeData);
                });

			if (!string.IsNullOrWhiteSpace(_openApiContent))
				RegisterOpenApiSpec(_openApiContent);

            if (!string.IsNullOrWhiteSpace(_openApiFile))
                RegisterOpenApiSpecFromFile(_openApiFile);

            if (!string.IsNullOrWhiteSpace(_openApiResource))
                RegisterOpenApiSpecFromResource(_openApiResource);
        }
    }
}