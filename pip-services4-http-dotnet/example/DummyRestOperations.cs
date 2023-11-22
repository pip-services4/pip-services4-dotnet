using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PipServices4.Commons.Convert;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PipServices4.Http.Services
{
    public class DummyRestOperations: RestOperations
    {
        private IDummyController _controller;

        public DummyRestOperations()
        {
            _dependencyResolver.Put("controller",
                new Descriptor("pip-services4-dummies", "controller", "default", "*", "*"));
        }
        
        public new void SetReferences(IReferences references)
        {
            base.SetReferences(references);

            _controller = _dependencyResolver.GetOneRequired<IDummyController>("controller");
        }

        public async Task GetPageByFilterAsync(HttpRequest request, HttpResponse response, ClaimsPrincipal user,
            RouteData routeData)
        {
            var traceId = GetTraceId(request);
            var filter = GetFilterParams(request);
            var paging = GetPagingParams(request);
            var sort = GetSortParams(request);
            var context = Context.FromTraceId(traceId);

            var result = await _controller.GetPageByFilterAsync(context, filter, paging);

            await SendResultAsync(response, result);
        }
        
        public async Task CreateAsync(HttpRequest request, HttpResponse response, ClaimsPrincipal user,
            RouteData routeData)
        {
            var traceId = GetTraceId(request);
            var parameters = GetParameters(request);
            var dummy = JsonConverter.FromJson<Dummy>(JsonConverter.ToJson(parameters.GetAsObject("dummy")));
            var context = Context.FromTraceId(traceId);

            var result = await _controller.CreateAsync(context, dummy);

            await SendResultAsync(response, result);
        }
        
        public async Task CreateFromFileAsync(HttpRequest request, HttpResponse response, ClaimsPrincipal user,
            RouteData routeData)
        {
            var traceId = GetTraceId(request);
            var parameters = GetParameters(request);
            var dummyFile = parameters.RequestFiles.Count > 0 ? parameters.RequestFiles[0] : null;
            var context = Context.FromTraceId(traceId);

            byte[] fileContent;

            using (var memoryStream = new MemoryStream())
            {
                    await dummyFile.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    fileContent = new byte[memoryStream.Length];
                    await memoryStream.ReadAsync(fileContent, 0, fileContent.Length);
            }

            var json = Encoding.UTF8.GetString(fileContent);
            var dummy = JsonConverter.FromJson<Dummy>(json);
            
            var result = await _controller.CreateAsync(context, dummy);

            await SendResultAsync(response, result);
        }
        
        public async Task UpdateAsync(HttpRequest request, HttpResponse response, ClaimsPrincipal user,
            RouteData routeData)
        {
            var traceId = GetTraceId(request);
            var parameters = GetParameters(request);
            var dummy = JsonConverter.FromJson<Dummy>(parameters.RequestBody);
            var context = Context.FromTraceId(traceId);

            var result = await _controller.UpdateAsync(context, dummy);

            await SendResultAsync(response, result);
        }
        
        public async Task GetByIdAsync(HttpRequest request, HttpResponse response, ClaimsPrincipal user,
            RouteData routeData)
        {
            var traceId = GetTraceId(request);
            var parameters = GetParameters(request);
            var id = parameters.GetAsNullableString("dummy_id") ?? parameters.GetAsNullableString("id");
            var context = Context.FromTraceId(traceId);

            var result = await _controller.GetOneByIdAsync(context, id);

            await SendResultAsync(response, result);
        }
        
        public async Task DeleteByIdAsync(HttpRequest request, HttpResponse response, ClaimsPrincipal user,
            RouteData routeData)
        {
            var traceId = GetTraceId(request);
            var parameters = GetParameters(request);
            var id = parameters.GetAsNullableString("dummy_id") ?? parameters.GetAsNullableString("id");
            var context = Context.FromTraceId(traceId);

            var result = await _controller.DeleteByIdAsync(context, id);

            await SendResultAsync(response, result);
        }

        public async Task CheckTraceId(HttpRequest request, HttpResponse response, ClaimsPrincipal user,
            RouteData routeData) 
        {
            var traceId = GetTraceId(request);
            var context = Context.FromTraceId(traceId);

            var result = await _controller.CheckTraceId(context);
            await SendResultAsync(response, result);
        }
    }
}