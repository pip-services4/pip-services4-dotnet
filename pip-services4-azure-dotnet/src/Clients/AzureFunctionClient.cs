using PipServices4.Azure.Connect;
using PipServices4.Commons.Convert;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using PipServices4.Observability.Trace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PipServices4.Azure.Clients
{
    /// <summary>
    /// Abstract client that calls Azure Functions.
    /// 
    /// When making calls "cmd" parameter determines which what action shall be called, while
    /// other parameters are passed to the action itself.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// - connections:                   
    ///     - uri:           full connection uri with specific app and function name
    ///     - protocol:      connection protocol
    ///     - project_id:    is your Azure Platform project ID
    ///     - region:        is the region where your function is deployed
    ///     - function:      is the name of the HTTP function you deployed
    ///     - org_id:        organization name
    /// - options:
    ///     - retries:               number of retries(default: 3)
    ///     - connect_timeout:       connection timeout in milliseconds(default: 10 sec)
    ///     - timeout:               invocation timeout in milliseconds(default: 10 sec)
    /// - credentials:   
    ///     - account: the service account name
    ///     - auth_token:    Azure-generated ID token or null if using custom auth(IAM)
    ///     
    /// 
    /// ### References ###
    ///     - *:logger:*:*:1.0         (optional) <a href="https://pip-services3-dotnet.github.io/pip-services3-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    ///     - *:counters:*:*:1.0         (optional) <a href="https://pip-services3-dotnet.github.io/pip-services3-components-dotnet/interface_pip_services_1_1_components_1_1_count_1_1_i_counters.html">ICounters</a> components to pass collected measurements
    ///     - *:discovery:*:*:1.0        (optional) <a href="https://pip-services3-dotnet.github.io/pip-services3-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    ///     - *:credential-store:*:*:1.0  (optional) Credential stores to resolve credentials
    ///     
    /// See <see cref="CommandableAzureClient"/>, <see cref="AzureFunction"/> 
    /// </summary>
    /// <example>
    /// <code>
    /// 
    /// class MyAzureFunctionClient: AzureFunctionClient, IMyClient
    /// {
    /// ...
    /// 
    ///     public async Task<MyData> GetDataAsync(IContext context, string id) {
    ///         var timing = this.Instrument(context, "myclient.get_data");
    ///         var result = await this.CallAsync<MyData>("get_data", context, new { id=id });
    ///         timing.EndTiming();
    ///         return result;
    ///     }
    ///     ...
    /// 
    ///     public async Task Main()
    ///     {
    ///         var client = new MyAzureFunctionClient();
    ///         client.Configure(ConfigParams.FromTuples(
    ///             "connection.uri", "http://<APP_NAME>.azurewebsites.net/api/<FUNCTION_NAME>",
    ///             "connection.protocol", protocol,
    ///             "connection.app_name", appName,
    ///             "connection.function_name", functionName,
    ///             "credential.auth_code", authCode
    ///         ));
    /// 
    ///         var  result = await client.GetDataAsync("123", "1");
    ///     }
    /// }
    /// 
    /// </code>
    /// </example>
    public abstract class AzureFunctionClient : IOpenable, IConfigurable, IReferenceable
    {
        /// <summary>
        /// The HTTP client.
        /// </summary>
        protected HttpClient _client;

        /// <summary>
        /// The Azure Function connection parameters
        /// </summary>
        protected AzureFunctionConnectionParams _connection;

        protected int _retries = 3;

        /// <summary>
        /// The default headers to be added to every request.
        /// </summary>
        protected Dictionary<string, string> _headers = new();

        /// <summary>
        /// The connection timeout in milliseconds.
        /// </summary>
        protected int _connectTimeout = 10000;

        /// <summary>
        /// The invocation timeout in milliseconds.
        /// </summary>
        protected int _timeout = 10000;

        /// <summary>
        /// The remote service uri which is calculated on open.
        /// </summary>
        protected string _uri;

        /// <summary>
        /// The dependencies resolver.
        /// </summary>
        protected DependencyResolver _dependencyResolver = new();

        /// <summary>
        /// The connection resolver.
        /// </summary>
        protected AzureFunctionConnectionResolver _connectionResolver = new();

        /// <summary>
        /// The logger.
        /// </summary>
        protected CompositeLogger _logger = new();

        /// <summary>
        /// The performance counters.
        /// </summary>
        protected CompositeCounters _counters = new();

        /// <summary>
        /// The tracer.
        /// </summary>
        protected CompositeTracer _tracer = new();

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public void Configure(ConfigParams config)
        {
            _connectionResolver.Configure(config);
            _dependencyResolver.Configure(config);

            _connectTimeout = config.GetAsIntegerWithDefault("options.connect_timeout", _connectTimeout);
            _timeout = config.GetAsIntegerWithDefault("options.timeout", _timeout);
            _retries = config.GetAsIntegerWithDefault("options.retries", _retries);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public void SetReferences(IReferences references)
        {
            _logger.SetReferences(references);
            _counters.SetReferences(references);
            _connectionResolver.SetReferences(references);
            _dependencyResolver.SetReferences(references);
        }

        /// <summary>
        /// Adds instrumentation to log calls and measure call time. It returns a CounterTiming
        /// object that is used to end the time measurement.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="methodName">a method name.</param>
        /// <returns>CounterTiming object to end the time measurement.</returns>
        protected CounterTiming Instrument(IContext context, string methodName)
        {
            _logger.Trace(context, "Executing {0} method", methodName);
            _counters.IncrementOne(methodName + ".exec_count");
            return _counters.BeginTiming(methodName + ".exec_time");
        }

        /// <summary>
        /// Adds instrumentation to error handling.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="methodName">a method name.</param>
        /// <param name="ex">Error that occured during the method call</param>
        /// <param name="rethrow">True to throw the exception</param>
        protected void InstrumentError(IContext context, string methodName, Exception ex, bool rethrow = false)
        {
            _logger.Error(context, ex, "Failed to execute {0} method", methodName);
            _counters.IncrementOne(methodName + ".exec_errors");

            if (rethrow)
                throw ex;
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public bool IsOpen()
        {
            return _client != null;
        }

        public async Task OpenAsync(IContext context)
        {
            if (IsOpen())
                return;

            _connection = await _connectionResolver.ResolveAsync(context);

            if (!string.IsNullOrEmpty(_connection.AuthCode))
                _headers["x-functions-key"] = _connection.AuthCode;
            _uri = _connection.FunctionUri;

            _client?.Dispose();

            try
            {
                _client = new HttpClient(new HttpClientHandler
                {
                    CookieContainer = new CookieContainer(),
                    AllowAutoRedirect = true,
                    UseCookies = true
                });

                _client.Timeout = TimeSpan.FromMilliseconds(_timeout + _connectTimeout);
                _client.DefaultRequestHeaders.ConnectionClose = true;

                _logger.Debug(context, "Azure Function client connected to %s", _connection.FunctionUri);
            }
            catch (Exception ex)
            {
                _client?.Dispose();
                _client = null;

                throw new ConnectionException(
                    context != null ? ContextResolver.GetTraceId(context) : null, 
                    "CANNOT_CONNECT", "Connection to Azure Function service failed"
                ).Wrap(ex).WithDetails("url", _uri);
            }
        }

        public Task CloseAsync(IContext context)
        {
            if (!IsOpen())
                return Task.CompletedTask;

            // Eat exceptions
            try
            {
                _client?.Dispose();
                _client = null;
                _uri = null;

                _logger.Debug(context, "Closed Azure Function service at %s", _uri);
            }
            catch (Exception ex)
            {
                _logger.Warn(context, "Failed while closing Azure Function service: %s", ex);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Performs Azure Function invocation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">>an action name to be called.</param>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="args">action arguments</param>
        /// <returns>action result.</returns>
        protected async Task<T> InvokeAsync<T>(string cmd, IContext context, object args)
        {
            if (string.IsNullOrEmpty(cmd))
                throw new UnknownException(null, "NO_COMMAND", "Missing command: " + cmd);

            // TODO: optimize this conversion
            args = JsonConverter.ToMap(JsonConverter.ToJson(args));

            JsonConverter.ToMap(JsonConverter.ToJson(new { cmd, context }))
                .ToList()
                .ForEach((el) => ((IDictionary)args).Add(el.Key, el.Value));

            // Set headers
            foreach (var key in _headers.Keys)
            {
                if (!_client.DefaultRequestHeaders.Contains(key))
                {
                    _client.DefaultRequestHeaders.Add(key, _headers[key]);
                }
            }

            HttpResponseMessage result = null;

            var retries = _retries;

            // args to request entity
            using (var requestContent = CreateEntityContent(args))
            {

                while (retries > 0)
                {
                    try
                    {
                        result = await _client.PostAsync(_uri, requestContent);

                        retries = 0;
                    }
                    catch (HttpRequestException ex)
                    {
                        retries--;
                        if (retries > 0)
                        {
                            throw new ConnectionException(context != null ? ContextResolver.GetTraceId(context) : null,
                                null, "Unknown communication problem on REST client", ex);
                        }
                        else
                        {
                            _logger.Trace(context, $"Connection failed to uri '{_uri}'. Retrying...");
                        }
                    }
                }
            }
            if (result == null)
            {
                throw ApplicationExceptionFactory.Create(ErrorDescriptionFactory.Create(
                    new UnknownException(context != null ? ContextResolver.GetTraceId(context) : null,
                        $"Unable to get a result from uri '{_uri}' with method '{HttpMethod.Post}'")));
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
                        errorObject = ErrorDescriptionFactory.Create(new UnknownException(context != null ? ContextResolver.GetTraceId(context) : null,
                            $"UNKNOWN_ERROR with result status: '{result.StatusCode}'", responseContent));
                    }
                }

                throw ApplicationExceptionFactory.Create(errorObject);
            }

            var value = await result.Content.ReadAsStringAsync();
            return JsonConverter.FromJson<T>(value);
        }

        /// <summary>
        /// Calls a Azure Function action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">an action name to be called.</param>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="args">(optional) action parameters.</param>
        /// <returns>action result.</returns>
        protected async Task<T> CallAsync<T>(string cmd, IContext context, object args)
        {
            return await this.InvokeAsync<T>(cmd, context, args);
        }

        private HttpContent CreateEntityContent(object value)
        {
            if (value == null) return null;

            var content = JsonConverter.ToJson(value);
            var result = new StringContent(content, Encoding.UTF8, "application/json");
            return result;
        }
    }
}