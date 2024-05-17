using Microsoft.AspNetCore.Http;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Data.Query;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PipServices4.Http.Controllers
{
    public class RestOperations : IConfigurable, IReferenceable
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected CompositeLogger _logger = new CompositeLogger();

        /// <summary>
        /// The performance counters.
        /// </summary>
        protected CompositeCounters _counters = new CompositeCounters();

        /// <summary>
        /// The dependency resolver.
        /// </summary>
        protected DependencyResolver _dependencyResolver = new DependencyResolver();

        public virtual void Configure(ConfigParams config)
        {
            _dependencyResolver.Configure(config);
        }

        public virtual void SetReferences(IReferences references)
        {
            _logger.SetReferences(references);
            _counters.SetReferences(references);
            _dependencyResolver.SetReferences(references);
        }

        protected IContext GetContext(HttpRequest request)
        {
            var traceId = GetTraceId(request);
            return Context.FromTraceId(traceId);
        }

        protected string GetTraceId(HttpRequest request)
        {
            return HttpRequestHelper.GetTraceId(request);
        }

        public static string GetId(HttpRequest request)
        {
            return HttpRequestHelper.ExtractFromQuery("id", request);
        }

        protected FilterParams GetFilterParams(HttpRequest request)
        {
            return HttpRequestHelper.GetFilterParams(request);
        }

        protected PagingParams GetPagingParams(HttpRequest request)
        {
            return HttpRequestHelper.GetPagingParams(request);
        }

        protected static ProjectionParams GetProjectionParams(HttpRequest request)
        {
            return ProjectionParams.FromValues(HttpRequestHelper.ExtractFromQuery("projection", request));
        }

        protected RestOperationParameters GetParameters(HttpRequest request)
        {
            return HttpRequestHelper.GetParameters(request);
        }

        protected SortParams GetSortParams(HttpRequest request)
        {
            return HttpRequestHelper.GetSortParams(request);
        }

        public static T GetContextItem<T>(HttpRequest request, string name)
            where T : class
        {
            return HttpRequestHelper.GetContextItem<T>(request, name);
        }

        protected async Task SendResultAsync(HttpResponse response, object result)
        {
            await HttpResponseSender.SendResultAsync(response, result);
        }

        protected async Task SendEmptyResultAsync(HttpResponse response)
        {
            await HttpResponseSender.SendEmptyResultAsync(response);
        }

        protected async Task SendCreatedResultAsync(HttpResponse response, object result)
        {
            await HttpResponseSender.SendCreatedResultAsync(response, result);
        }

        protected async Task SendDeletedResultAsync(HttpResponse response, object result)
        {
            await HttpResponseSender.SendDeletedResultAsync(response, result);
        }

        protected async Task SendErrorAsync(HttpResponse response, Exception error)
        {
            await HttpResponseSender.SendErrorAsync(response, error);
        }

        protected async Task SendBadRequestAsync(HttpRequest request, HttpResponse response, string message)
        {
            var traceId = GetTraceId(request);
            var error = new BadRequestException(traceId, "BAD_REQUEST", message);
            await SendErrorAsync(response, error);
        }

        protected async Task SendUnauthorizedAsync(HttpRequest request, HttpResponse response, string message)
        {
            var traceId = GetTraceId(request);
            var error = new UnauthorizedException(traceId, "UNAUTHORIZED", message);
            await SendErrorAsync(response, error);
        }

        protected async Task SendNotFoundAsync(HttpRequest request, HttpResponse response, string message)
        {
            var traceId = GetTraceId(request);
            var error = new NotFoundException(traceId, "NOT_FOUND", message);
            await SendErrorAsync(response, error);
        }

        protected async Task SendConflictAsync(HttpRequest request, HttpResponse response, string message)
        {
            var traceId = GetTraceId(request);
            var error = new ConflictException(traceId, "CONFLICT", message);
            await SendErrorAsync(response, error);
        }

        protected async Task SendSessionExpiredASync(HttpRequest request, HttpResponse response, string message)
        {
            var traceId = GetTraceId(request);
            var error = new PipServices4.Commons.Errors.ApplicationException(null, traceId, "SESSION_EXPIRED", message) 
            {
                Status = 440
            };
            await SendErrorAsync(response, error);
        }

        protected async Task SendInternalErrorAsync(HttpRequest request, HttpResponse response, string message)
        {
            var traceId = GetTraceId(request);
            var error = new InternalException(traceId, "INTERNAL", message);
            await SendErrorAsync(response, error);
        }

        protected async Task SendServerUnavailableAsync(HttpRequest request, HttpResponse response, string message)
        {
            var traceId = GetTraceId(request);
            var error = new PipServices4.Commons.Errors.ApplicationException(null, traceId, "SERVER_UNAVAILABLE", message)
            {
                Status = StatusCodes.Status503ServiceUnavailable
            };
            await SendErrorAsync(response, error);
        }

        protected async Task SendTooManyRequestsAsync(HttpRequest request, HttpResponse response, string message)
        {
            var traceId = GetTraceId(request);
            var error = new TooManyRequestsException(traceId, "TOO_MANY_REQUESTS", message);
            await SendErrorAsync(response, error);
        }

        public async Task InvokeAsync(string operation, object[] parameters)
        {
            Type t = GetType();
            MethodInfo method = t.GetMethod(operation);

            if (method != null) await Task.FromResult(method.Invoke(this, parameters));
        }

        public async Task<dynamic> InvokeWithResponseAsync(string operation, object[] parameters)
        {
            Type t = GetType();
            MethodInfo method = t.GetMethod(operation);

            if (method != null) return await Task.FromResult(method.Invoke(this, parameters));
            else return null;
        }

        protected virtual void HandleError(IContext context, string methodName, Exception ex)
        {
            _logger.Error(context, ex, $"Failed to execute {methodName}");
        }

        protected virtual CounterTiming Instrument(IContext context, string methodName, string message = "")
        {
            _logger.Trace(context, $"Executed {methodName} {message}");
            return _counters.BeginTiming(methodName + ".exec_time");
        }

        protected virtual async Task SafeInvokeAsync(string methodName, HttpRequest request, HttpResponse response, Func<string, Task> invokeFunc)
        {
            var traceId = GetTraceId(request);
            var context = Context.FromTraceId(traceId);

            using (var timing = Instrument(context, methodName))
            {
                try
                {
                    await invokeFunc(traceId);
                }
                catch (BadRequestException e)
                {
                    HandleError(context, methodName, e);

                    await SendBadRequestAsync(request, response, $"Incorrect body: {e.Message}");
                }
                catch (Exception ex)
                {
                    HandleError(context, methodName, ex);

                    await SendErrorAsync(response, ex);
                }
            }
        }
    }
}