using PipServices4.Commons.Convert;
using PipServices4.Commons.Data;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Config.Connect;
using PipServices4.Data.Query;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PipServices4.Http.Clients
{
    /// <summary>
    /// Abstract client that calls remove endpoints using HTTP/REST protocol.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// - base_route:              base route for remote URI
    /// 
    /// connection(s):
    /// - discovery_key:         (optional) a key to retrieve the connection from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
    /// - protocol:              connection protocol: http or https
    /// - host:                  host name or IP address
    /// - port:                  port number
    /// - uri:                   resource URI or connection string with all parameters in it 
    /// 
    /// options:
    /// - retries:               number of retries(default: 3)
    /// - connect_timeout:       connection timeout in milliseconds(default: 10 sec)
    /// - timeout:               invocation timeout in milliseconds(default: 10 sec)
    /// - trace_id               place for adding correalationId, query - in query string, headers - in headers, both - in query and headers (default: query)
    /// 
    /// ### References ###
    /// 
    /// - *:logger:*:*:1.0         (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// - *:counters:*:*:1.0         (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_count_1_1_i_counters.html">ICounters</a> components to pass collected measurements
    /// - *:discovery:*:*:1.0        (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    /// </summary>
    /// <example>
    /// <code>
    /// class MyRestClient: RestClient, IMyClient 
    /// {
    ///     ...
    /// 
    ///     public MyData GetData(IContext context, string id)
    ///     {
    ///         var timing = this.Instrument(context, 'myclient.get_data');
    ///         try
    ///         {
    ///           var result = this.ExecuteAsync<MyData>(context, HttpMethod.Post, "/get_data", new MyData(id));
    ///         }
    ///         catch (Exception ex)
    ///         {
    ///           this.InstrumentError(context, "myclient.get_data", ex, true);
    ///         }
    ///         finally
    ///         {
    ///           timing.EndTiming();
    ///         }
    ///         return result;        
    ///     }
    ///     ...
    /// }
    /// 
    /// var client = new MyRestClient();
    /// client.Configure(ConfigParams.fromTuples(
    /// "connection.protocol", "http",
    /// "connection.host", "localhost",
    /// "connection.port", 8080 ));
    /// 
    /// var data = client.GetData("123", "1");
    /// ...
    /// </code>
    /// </example>
    public class RestClient : IOpenable, IConfigurable, IReferenceable
    {
        private static readonly ConfigParams _defaultConfig = ConfigParams.FromTuples(
            "connection.protocol", "http",
            //"connection.host", "localhost",
            //"connection.port", 3000,

            "options.request_max_size", 1024*1024,
            "options.connect_timeout", 60000,
            "options.retries", 1,
            "options.debug", true,
            "options.trace_id_place", "query"
        );

        /// <summary>
        /// The connection resolver.
        /// </summary>
        protected HttpConnectionResolver _connectionResolver = new HttpConnectionResolver();
        /// <summary>
        /// The logger.
        /// </summary>
        protected CompositeLogger _logger = new CompositeLogger();
        /// <summary>
        /// The performance counters.
        /// </summary>
        protected CompositeCounters _counters = new CompositeCounters();
        /// <summary>
        /// The configuration options.
        /// </summary>
        protected ConfigParams _options = new ConfigParams();
        /// <summary>
        /// The base route.
        /// </summary>
        protected string _baseRoute;
        /// <summary>
        /// The number of retries.
        /// </summary>
        protected int _retries = 1;
        /// <summary>
        /// The invocation timeout (ms).
        /// </summary>
        protected int _timeout = 100000;

        /// <summary>
        /// The HTTP client.
        /// </summary>
        protected HttpClient _client;
        /// <summary>
        /// The remote service uri which is calculated on open.
        /// </summary>
        protected string _address;

        /// <summary>
        /// The default headers to be added to every request.
        /// </summary>
        protected StringValueMap _headers = new StringValueMap();

        /// <summary>
        /// The place for adding correalationId: query - in query string, headers - in headers, both - in query and headers (default: query)
        /// </summary>
        protected string _contextPlace = "query";

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public virtual void Configure(ConfigParams config)
        {
            config = config.SetDefaults(_defaultConfig);
            _connectionResolver.Configure(config);
            _options = _options.Override(config.GetSection("options"));

            _retries = config.GetAsIntegerWithDefault("options.retries", _retries);
            _timeout = config.GetAsIntegerWithDefault("options.timeout", _timeout); ;

            _baseRoute = config.GetAsStringWithDefault("base_route", _baseRoute);
            _contextPlace = config.GetAsStringWithDefault("options.trace_id_place", _contextPlace);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public virtual void SetReferences(IReferences references)
        {
            _connectionResolver.SetReferences(references);
            _logger.SetReferences(references);
            _counters.SetReferences(references);
        }

        /// <summary>
        /// Adds instrumentation to log calls and measure call time. It returns a CounterTiming
        /// object that is used to end the time measurement.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="methodName">a method name.</param>
        /// <returns>CounterTiming object to end the time measurement.</returns>
        protected CounterTiming Instrument(IContext context, [CallerMemberName]string methodName = null)
        {
            var typeName = GetType().Name;
            _logger.Trace(context, "Calling {0} method of {1}", methodName, typeName);
            _counters.IncrementOne(typeName + "." + methodName + ".call_count");
            return _counters.BeginTiming(typeName + "." + methodName + ".call_time");
        }

        /// <summary>
        /// Adds instrumentation to error handling.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="methodName">a method name.</param>
        /// <param name="ex">Error that occured during the method call</param>
        /// <param name="rethrow">True to throw the exception</param>
        protected void InstrumentError(IContext context, [CallerMemberName]string methodName = null, Exception ex = null, bool rethrow = false)
        {
            var typeName = GetType().Name;
            _logger.Error(context, ex, "Failed to call {0} method of {1}", methodName, typeName);
            _counters.IncrementOne(typeName + "." + methodName + ".call_errors");

            if (rethrow)
                throw ex;
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public virtual bool IsOpen()
        {
            return _client != null;
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async virtual Task OpenAsync(IContext context)
        {
            var connection = await _connectionResolver.ResolveAsync(context);

            var protocol = connection.Protocol;
            var host = connection.Host;
            var port = connection.Port;

            _address = protocol + "://" + host + ":" + port;

            _client?.Dispose();

            _client = new HttpClient(new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                AllowAutoRedirect = true,
                UseCookies = true
            });

            _client.Timeout = TimeSpan.FromMilliseconds(_timeout);
            _client.DefaultRequestHeaders.ConnectionClose = true;

            _logger.Debug(context, "Connected via REST to {0}", _address);
        }

        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public virtual Task CloseAsync(IContext context)
        {
            _client?.Dispose();
            _client = null;

            _address = null;

            _logger.Debug(context, "Disconnected from {0}", _address);

            return Task.CompletedTask;
        }

        protected HttpContent CreateEntityContent(object value)
        {
            if (value == null) return null;

            var content = JsonConverter.ToJson(value);
            var result = new StringContent(content, Encoding.UTF8, "application/json");
            return result;
        }

        protected Uri CreateRequestUri(string route)
        {
            var builder = new StringBuilder(_address);

            if (!string.IsNullOrEmpty(_baseRoute))
            {
                if (_baseRoute[0] != '/')
                {
                    builder.Append('/');
                }
                builder.Append(_baseRoute);
            }

            if (!string.IsNullOrWhiteSpace(route))
            {
                if (route[0] != '?' && route[0] != '/')
                {
                    builder.Append('/');
                }
                builder.Append(route);
            }

            var uri = builder.ToString();

            var result = new Uri(uri, UriKind.Absolute);

            return result;
        }

        protected string ConstructQueryString(NameValueCollection parameters)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string name in parameters)
            {
                if (builder.Length > 0)
                    builder.Append('&');
                builder.Append(name);
                builder.Append('=');
                builder.Append(System.Web.HttpUtility.UrlEncode(parameters[name]));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Adds a trace id (trace_id) to invocation parameter map.
        /// </summary>
        /// <param name="route">invocation parameters.</param>
        /// <param name="context">(optional) a trace id to be added.</param>
        /// <returns>invocation parameters with added trace id.</returns>
        protected string AddTraceId(string route, IContext context)
        {
            var traceId = ContextResolver.GetTraceId(context); 
            if (_contextPlace == "headers" || _contextPlace == "both")
            {
                _headers["trace_id"] = traceId;
            }

            if (_contextPlace != "query" && _contextPlace != "both")
            {
                return route;
            }

            var pos = route.IndexOf('?');
            var path = pos >= 0 ? route.Substring(0, pos) : route;
            var query = pos >= 0 ? route.Substring(pos) : "";

            var parameters = HttpUtility.ParseQueryString(query);
            parameters["trace_id"] = traceId;
            query = ConstructQueryString(parameters);
            return path + "?" + query;
        }

        /// <summary>
        /// Adds filter parameters (with the same name as they defined) to invocation parameter map.
        /// </summary>
        /// <param name="route">invocation parameters.</param>
        /// <param name="filter">(optional) filter parameters</param>
        /// <returns>invocation parameters with added filter parameters.</returns>
        protected string AddFilterParams(string route, FilterParams filter)
        {
            var pos = route.IndexOf('?');
            var path = pos >= 0 ? route.Substring(0, pos) : route;
            var query = pos >= 0 ? route.Substring(pos) : "";

            var parameters = HttpUtility.ParseQueryString(query);

            if (filter != null)
            {
                foreach (var key in filter.Keys)
                {
                    parameters[key] = filter[key];
                }
            }

            query = ConstructQueryString(parameters);

            if (string.IsNullOrWhiteSpace(query))
            {
                return path;
            }

            return path + "?" + query;
        }

        /// <summary>
        /// Adds paging parameters (skip, take, total) to invocation parameter map.
        /// </summary>
        /// <param name="route">invocation parameters.</param>
        /// <param name="paging">(optional) paging parameters</param>
        /// <returns>invocation parameters with added paging parameters.</returns>
        protected string AddPagingParams(string route, PagingParams paging)
        {
            var pos = route.IndexOf('?');
            var path = pos >= 0 ? route.Substring(0, pos) : route;
            var query = pos >= 0 ? route.Substring(pos) : "";

            var parameters = HttpUtility.ParseQueryString(query);

            if (paging != null)
            {
                if (paging.Skip.HasValue)
                {
                    parameters["skip"] = paging.Skip.Value.ToString();
                }
                if (paging.Take.HasValue)
                {
                    parameters["take"] = paging.Take.Value.ToString();
                }
                if (paging.Total)
                {
                    parameters["total"] = StringConverter.ToString(paging.Take);
                }
            }

            query = ConstructQueryString(parameters);

            if (string.IsNullOrWhiteSpace(query))
            {
                return path;
            }

            return path + "?" + query;
        }

        private async Task<HttpResponseMessage> ExecuteRequestAsync(
            IContext context, HttpMethod method, Uri uri, HttpContent content = null)
        {
            if (_client == null)
                throw new InvalidOperationException("REST client is not configured");

            // Set headers
            foreach (var key in _headers.Keys)
            {
                if (!_client.DefaultRequestHeaders.Contains(key))
                {
                    _client.DefaultRequestHeaders.Add(key, _headers[key]);
                }
            }

            HttpResponseMessage result = null;
            string traceId = context != null ? ContextResolver.GetTraceId(context) : null;

            var retries = Math.Min(1, Math.Max(5, _retries));
            while (retries > 0)
            {
                try
                {
                    if (method == HttpMethod.Get)
                        result = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                    else if (method == HttpMethod.Post)
                        result = await _client.PostAsync(uri, content);
                    else if (method == HttpMethod.Put)
                        result = await _client.PutAsync(uri, content);
                    else if (method == HttpMethod.Delete)
                        result = await _client.DeleteAsync(uri);
                    else if (method == HttpMethod.Head)
                        result = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, uri));
#if !NETSTANDARD2_0
                    else if (method == HttpMethod.Patch)
                        result = await _client.PatchAsync(uri, content);
#endif
                    else
                        throw new InvalidOperationException("Invalid request type");

                    retries = 0;
                }
                catch (HttpRequestException ex)
                {
                    retries--;
                    if (retries > 0)
                    {
                        throw new ConnectionException(traceId, null, "Unknown communication problem on REST client", ex);
                    }
                    else
                    {
                        _logger.Trace(context, $"Connection failed to uri '{uri}'. Retrying...");
                    }
                }
            }

            if (result == null)
            {
                throw ApplicationExceptionFactory.Create(ErrorDescriptionFactory.Create(
                    new UnknownException(traceId, $"Unable to get a result from uri '{uri}' with method '{method}'")));
            }

            if ((int)result.StatusCode >= 400)
            {
                var responseContent = await result.Content.ReadAsStringAsync();

                ErrorDescription errorObject = null;
                try
                {
                    errorObject = JsonConverter.FromJson<ErrorDescription>(responseContent);
                }
                finally
                {
                    if (errorObject == null)
                    {
                        errorObject = ErrorDescriptionFactory.Create(new UnknownException(traceId, $"UNKNOWN_ERROR with result status: '{result.StatusCode}'", responseContent));
                    }
                }

                throw ApplicationExceptionFactory.Create(errorObject);
            }

            return result;
        }

        /// <summary>
        /// Executes a remote method via HTTP/REST protocol.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "head", "post", "put", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        protected async Task ExecuteAsync(IContext context, HttpMethod method, string route)
        {
            route = AddTraceId(route, context);
            var uri = CreateRequestUri(route);

            await ExecuteRequestAsync(context, method, uri);
        }

        /// <summary>
        /// Executes a remote method via HTTP/REST protocol.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "head", "post", "put", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <param name="requestEntity">request body object.</param>
        protected async Task ExecuteAsync(IContext context, HttpMethod method, string route, object requestEntity)
        {
            route = AddTraceId(route, context);
            var uri = CreateRequestUri(route);

            using (var requestContent = CreateEntityContent(requestEntity))
            {
                await ExecuteRequestAsync(context, method, uri, requestContent);
            }
        }

        private static async Task<T> ExtractContentEntityAsync<T>(IContext context, HttpContent content)
        {
            var value = await content.ReadAsStringAsync();

            try
            {
                return JsonConverter.FromJson<T>(value);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(
                    context != null ? ContextResolver.GetTraceId(context) : null, 
                    null, "Unexpected protocol format", ex);
            }
        }

        /// <summary>
        /// Executes a remote method via HTTP/REST protocol.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "head", "post", "put", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <returns>result object.</returns>
        protected async Task<T> ExecuteAsync<T>(IContext context, HttpMethod method, string route)
            where T : class
        {
            route = AddTraceId(route, context);
            var uri = CreateRequestUri(route);

            using (var response = await ExecuteRequestAsync(context, method, uri))
            {
                return await ExtractContentEntityAsync<T>(context, response.Content);
            }
        }

        protected async Task<string> ExecuteStringAsync(IContext context, HttpMethod method, string route)
        {
            route = AddTraceId(route, context);
            var uri = CreateRequestUri(route);

            using (var response = await ExecuteRequestAsync(context, method, uri))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        protected async Task<string> ExecuteStringAsync(IContext context, HttpMethod method, string route, object requestEntity)
        {
            route = AddTraceId(route, context);
            var uri = CreateRequestUri(route);

            using (var requestContent = CreateEntityContent(requestEntity))
            {
                using (var response = await ExecuteRequestAsync(context, method, uri, requestContent))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        /// <summary>
        /// Executes a remote method via HTTP/REST protocol.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "head", "post", "put", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <param name="requestEntity">request body object.</param>
        /// <returns>result object.</returns>
        protected async Task<T> ExecuteAsync<T>(
            IContext context, HttpMethod method, string route, object requestEntity)
            where T : class
        {
            route = AddTraceId(route, context);
            var uri = CreateRequestUri(route);

            using (var requestContent = CreateEntityContent(requestEntity))
            {
                using (var response = await ExecuteRequestAsync(context, method, uri, requestContent))
                {
                    return await ExtractContentEntityAsync<T>(context, response.Content);
                }
            }
        }

        /// <summary>
        /// Safely executes a remote method via HTTP/REST protocol and logs execution time.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "post", "put", "patch"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <param name="requestEntity">request body object.</param>
        /// <returns>result object.</returns>
        protected async Task<T> SafeExecuteAsync<T>(IContext context, HttpMethod method, string route, object requestEntity)
            where T : class
        {
            var pos = !string.IsNullOrWhiteSpace(route) ? route.IndexOf('?') : -1;
            var methodName = pos >= 0 ? route.Substring(0, pos) : route;

            using (var timing = Instrument(context, methodName))
            {
                try
                {
                    return await ExecuteAsync<T>(context, method, route, requestEntity);
                }
                catch (Exception ex)
                {
                    InstrumentError(context, _baseRoute + "." + route, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Safely executes a remote method via HTTP/REST protocol and logs execution time.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <returns>result object.</returns>
        protected async Task<T> SafeExecuteAsync<T>(IContext context, HttpMethod method, string route)
            where T : class
        {
            var pos = !string.IsNullOrWhiteSpace(route) ? route.IndexOf('?') : -1;
            var methodName = pos >= 0 ? route.Substring(0, pos) : route;

            using (var timing = Instrument(context, methodName))
            {
                try
                {
                    return await ExecuteAsync<T>(context, method, route);
                }
                catch (Exception ex)
                {
                    InstrumentError(context, _baseRoute + "." + route, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Calls a remote method via HTTP/REST protocol.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "head", "post", "put", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        protected async Task CallAsync(IContext context, HttpMethod method, string route)
        {
            await ExecuteAsync(context, method, route);
        }

        /// <summary>
        /// Calls a remote method via HTTP/REST protocol.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "head", "post", "put", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <param name="requestEntity">request body object.</param>
        protected async Task CallAsync(IContext context, HttpMethod method, string route, object requestEntity)
        {
            await ExecuteAsync(context, method, route, requestEntity);
        }

        /// <summary>
        /// Calls a remote method via HTTP/REST protocol.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "head", "post", "put", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <returns>result object.</returns>
        protected async Task<T> CallAsync<T>(IContext context, HttpMethod method, string route)
            where T : class
        {
            return await ExecuteAsync<T>(context, method, route);
        }

        protected async Task<string> CallStringAsync(IContext context, HttpMethod method, string route)
        {
            return await ExecuteStringAsync(context, method, route);
        }

        protected async Task<string> CallStringAsync(IContext context, HttpMethod method, string route, object requestEntity)
        {
            return await ExecuteStringAsync(context, method, route);
        }

        /// <summary>
        /// Calls a remote method via HTTP/REST protocol.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "head", "post", "put", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <param name="requestEntity">request body object.</param>
        /// <returns>result object.</returns>
        protected async Task<T> CallAsync<T>(IContext context, HttpMethod method, string route, object requestEntity)
            where T : class
        {
            return await ExecuteAsync<T>(context, method, route, requestEntity);
        }

        /// <summary>
        /// Safely calls a remote method via HTTP/REST protocol and logs execution time.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "post", "put", "patch"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <param name="requestEntity">request body object.</param>
        /// <returns>result object.</returns>
        protected async Task<T> SafeCallAsync<T>(IContext context, HttpMethod method, string route, object requestEntity)
            where T : class
        {
            return await SafeExecuteAsync<T>(context, method, route, requestEntity);
        }

        /// <summary>
        /// Safely calls a remote method via HTTP/REST protocol and logs execution time.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="method">HTTP method: "get", "delete"</param>
        /// <param name="route">a command route. Base route will be added to this route</param>
        /// <returns>result object.</returns>
        protected async Task<T> SafeCallAsync<T>(IContext context, HttpMethod method, string route)
            where T : class
        {
            return await SafeExecuteAsync<T>(context, method, route);
        }

    }
}

