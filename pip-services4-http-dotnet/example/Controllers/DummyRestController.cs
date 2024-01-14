using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Data.Validate;
using PipServices4.Http.Auth;
using PipServices4.Http.Controllers;
using PipServices4.Http.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TypeCode = PipServices4.Commons.Convert.TypeCode;

namespace PipServices4.Http.Services
{
    public class DummyRestController : RestController
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

            var tags = new[] { "Dummy" };
            var schema = new DummySchema();

            RegisterRouteWithAuthAndMetadata("get", "/dummies", auth.Anybody(), _operations.GetPageByFilterAsync, new RestRouteMetadata()
                    .SetsTags(tags)
                    .UsesBearerAuthentication()
                    .ReceivesTraceIdParam()
                    .ReceivesOptionalQueryParam("filter", TypeCode.Object)
                    .ReceivesOptionalQueryParam("paging", TypeCode.Object)
                    .SendsDataPage200(schema)
                );

            RegisterRouteWithAuthAndMetadata("get", "/dummies/{id}", auth.Anybody(), _operations.GetByIdAsync, new RestRouteMetadata()
                    .SetsTags(tags)
                    .ReceivesTraceIdParam()
                    .SendsData200(schema)
                );

            RegisterRouteWithAuthAndMetadata("post", "/dummies", auth.Anybody(), _operations.CreateAsync, new RestRouteMetadata()
                    .SetsTags(tags)
                    .ReceivesTraceIdParam()
                    .ReceivesBodyFromSchema(schema)
                    .SendsData200(schema)
                    .SendsData400()
                );

            RegisterRouteWithAuthAndMetadata("post", "/dummies/file", auth.Anybody(), _operations.CreateFromFileAsync, new RestRouteMetadata()
                    .SetsTags(tags)
                    .ReceivesTraceIdParam()
                    .ReceivesFile()
                    .SendsData200(schema)
                    .SendsData400()
                );

            RegisterRouteWithAuthAndMetadata("put", "/dummies", auth.Anybody(), _operations.UpdateAsync, new RestRouteMetadata()
                    .SetsTags(tags)
                    .ReceivesTraceIdParam()
                    .ReceivesBodyFromSchema(schema)
                    .SendsData200(schema)
                    .SendsData400()
                );

            RegisterRouteWithAuthAndMetadata("put", "/dummies/{id}", auth.Anybody(), _operations.UpdateAsync, new RestRouteMetadata()
                    .SetsTags(tags)
                    .ReceivesTraceIdParam()
                    .ReceivesBodyFromSchema(schema)
                    .SendsData200(schema)
                    .SendsData400()
                );

            RegisterRouteWithAuthAndMetadata("delete", "/dummies/{id}", auth.Anybody(), _operations.DeleteByIdAsync, new RestRouteMetadata()
                    .SetsTags(tags)
                    .ReceivesTraceIdParam()
                    .SendsData200(schema)
                );

            RegisterRouteWithAuthAndMetadata("get", "/test_responses", auth.Anybody(), null, new RestRouteMetadata()
                    .SetsTags(tags)
                    .ReceivesTraceIdParam()
                    .SendsData(110, "new DummySchema()", new DummySchema())
                    .SendsData(111, "new ArraySchema()", new ArraySchema())
                    .SendsData(112, "new ArraySchema(new DummySchema())", new ArraySchema(new DummySchema()))
                    .SendsData(113, "new List< DummySchema >()", new List<DummySchema>())
                    .SendsData(114, "new List< string >()", new List<string>())
                    .SendsData(115, "new byte[] { }", new byte[] { })
                    .SendsData(116, "new Dictionary< string, string >()", new Dictionary<string, string>())
                    .SendsData(117, "new Dictionary< string, bool >()", new Dictionary<string, bool>())
                    .SendsData(118, "TypeCode.String", TypeCode.String)
                );

            if (!string.IsNullOrWhiteSpace(_openApiContent))
            {
                RegisterOpenApiSpec(_openApiContent);
            }
            else if (!string.IsNullOrWhiteSpace(_openApiFile))
            {
                RegisterOpenApiSpecFromFile(_openApiFile);
            }
            else if (!string.IsNullOrWhiteSpace(_openApiResource))
            {
                RegisterOpenApiSpecFromResource(_openApiResource);
            }
            else
            {
                RegisterOpenApiSpecFromMetadata();
            }
        }
    }
}