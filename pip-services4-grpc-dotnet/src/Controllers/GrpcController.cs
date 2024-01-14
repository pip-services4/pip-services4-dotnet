using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Interceptors;
using PipServices4.Commons.Errors;
using PipServices4.Commons.Reflect;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Data.Validate;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Grpc.Controllers
{
    /// <summary>
    /// Abstract service that receives remove calls via HTTP/REST protocol.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// - base_route:              base route for remote URI
    /// 
    /// dependencies:
    /// - endpoint:              override for HTTP Endpoint dependency
    /// - controller:            override for Controller dependency
    /// 
    /// connection(s):
    /// - discovery_key:         (optional) a key to retrieve the connection from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
    /// - protocol:              connection protocol: http or https
    /// - host:                  host name or IP address
    /// - port:                  port number
    /// - uri:                   resource URI or connection string with all parameters in it
    /// 
    /// ### References ###
    /// 
    /// - *:logger:*:*:1.0          (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// - *:counters:*:*:1.0        (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_count_1_1_i_counters.html">ICounters</a> components to pass collected measurements
    /// - *:discovery:*:*:1.0       (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    /// - *:endpoint:http:*:1.0     (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-rpc-dotnet/class_pip_services_1_1_rpc_1_1_services_1_1_http_endpoint.html">HttpEndpoint</a/a> reference
    /// </summary>
    /// <example>
    /// <code>
    /// class MyRestService: RestService 
    /// {
    ///     private IMyController _controller;
    ///     ...
    ///     public MyRestService()
    ///     {
    ///         base();
    ///         this._dependencyResolver.put(
    ///         "controller", new Descriptor("mygroup", "controller", "*", "*", "1.0"));
    ///     }
    ///     
    ///     public void SetReferences(IReferences references)
    ///     {
    ///         base.SetReferences(references);
    ///         this._controller = this._dependencyResolver.getRequired<IMyController>("controller");
    ///     }
    ///     
    ///     public void register()
    ///     {
    ///         ...
    ///     }
    /// }
    /// 
    /// var service = new MyRestService();
    /// service.Configure(ConfigParams.fromTuples(
    /// "connection.protocol", "http",
    /// "connection.host", "localhost",
    /// "connection.port", 8080 ));
    /// 
    /// service.SetReferences(References.fromTuples(
    /// new Descriptor("mygroup","controller","default","default","1.0"), controller ));
    /// 
    /// service.Open("123");
    /// Console.Out.WriteLine("The REST service is running on port 8080");
    /// </code>
    /// </example>
    public abstract class GrpcController : IOpenable, IConfigurable, IReferenceable, IUnreferenceable, IRegisterable
    {
        private static readonly ConfigParams _defaultConfig = ConfigParams.FromTuples(
            "dependencies.endpoint", "*:endpoint:grpc:*:1.0"
        );

        /// <summary>
        /// The HTTP endpoint that exposes this service.
        /// </summary>
        protected GrpcEndpoint _endpoint;
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
        protected DependencyResolver _dependencyResolver = new DependencyResolver(_defaultConfig);

        private ConfigParams _config;
        private IReferences _references;
        private bool _localEndpoint;
        private List<Interceptor> _interceptors = new List<Interceptor>();
        private bool _opened;
        protected string _serviceName;
        private readonly ServerServiceDefinition.Builder _builder = ServerServiceDefinition.CreateBuilder();
        private readonly Dictionary<Type, object> _messageParsers = new Dictionary<Type, object>();

        public GrpcController(string serviceName)
        {
            _serviceName = serviceName;
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public virtual void Configure(ConfigParams config)
        {
            _config = config.SetDefaults(_defaultConfig);
            _dependencyResolver.Configure(config);

            _serviceName = config.GetAsStringWithDefault("service_name", _serviceName);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public virtual void SetReferences(IReferences references)
        {
            _references = references;

            _logger.SetReferences(references);
            _counters.SetReferences(references);
            _dependencyResolver.SetReferences(references);

            // Get endpoint
            _endpoint = _dependencyResolver.GetOneOptional("endpoint") as GrpcEndpoint;
            _localEndpoint = _endpoint == null;

            // Or create a local one
            if (_endpoint == null)
                _endpoint = CreateLocalEndpoint();

            // Add registration callback to the endpoint
            _endpoint.Register(this);
        }

        /// <summary>
        /// Unsets (clears) previously set references to dependent components.
        /// </summary>
        public virtual void UnsetReferences()
        {
            // Remove registration callback from endpoint
            if (_endpoint != null)
            {
                _endpoint.Unregister(this);
                _endpoint = null;
            }
        }

        private GrpcEndpoint CreateLocalEndpoint()
        {
            var endpoint = new GrpcEndpoint();

            if (_config != null)
                endpoint.Configure(_config);

            if (_references != null)
                endpoint.SetReferences(_references);

            return endpoint;
        }

        /// <summary>
        /// Adds instrumentation to log calls and measure call time. It returns a CounterTiming
        /// object that is used to end the time measurement.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
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
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
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
            return _opened;
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns></returns>
        public async virtual Task OpenAsync(IContext context)
        {
            if (IsOpen()) return;

            if (_endpoint == null)
            {
                _endpoint = CreateLocalEndpoint();
                _endpoint.Register(this);
                _localEndpoint = true;
            }

            if (_localEndpoint)
            {
                await _endpoint.OpenAsync(context).ContinueWith(task =>
                {
                    _opened = task.Exception == null;
                });
            }
            else
            {
                _opened = true;
            }
        }

        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns></returns>
        public virtual Task CloseAsync(IContext context)
        {
            if (IsOpen())
            {
                if (_endpoint == null)
                {
                    throw new InvalidStateException(context != null ? ContextResolver.GetTraceId(context) : null, "NO_ENDPOINT", "gRPC endpoint is missing");
                }

                if (_localEndpoint)
                {
                    _endpoint.CloseAsync(context);
                }

                _opened = false;
            }

            return Task.Delay(0);
        }

        protected virtual string GetTraceId<TRequest>(TRequest request)
            where TRequest : class, IMessage<TRequest>, new()
        {
            return ObjectReader.GetProperty(request, "TraceId") as string;
        }

        protected virtual object ConvertFromPublic<TRequest>(TRequest request)
            where TRequest : class, IMessage<TRequest>, new()
        {
            return request;
        }

        protected UnaryServerMethod<TRequest, TResponse> ApplyValidation<TRequest, TResponse>(Schema schema, UnaryServerMethod<TRequest, TResponse> action)
            where TRequest : class, IMessage<TRequest>, new()
            where TResponse : class, IMessage<TResponse>, new()
        {
            UnaryServerMethod<TRequest, TResponse> actionWrapper = (request, context) =>
            {
                if (schema != null)
                {
                    string traceId = GetTraceId(request); // request.TraceId
                    object value = ConvertFromPublic(request); //request.XXX ?

                    schema.ValidateAndThrowException(traceId, value);
                }

                return action.Invoke(request, context);
            };

            return actionWrapper;
        }

        protected UnaryServerMethod<TRequest, TResponse> ApplyInterceptors<TRequest, TResponse>(UnaryServerMethod<TRequest, TResponse> action)
            where TRequest : class, IMessage<TRequest>, new()
            where TResponse : class, IMessage<TResponse>, new()
        {
            var actionWrapper = action;

            for (int index = _interceptors.Count - 1; index >= 0; index--)
            {
                var interceptor = _interceptors[index];
                actionWrapper = (request, context) =>
                {
                    return interceptor.UnaryServerHandler(request, context, actionWrapper);
                };
            }

            return actionWrapper;
        }

        /// <summary>
        /// Registers a method in GRPC service.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="name"></param>
        /// <param name="schema">a validation schema to validate received parameters.</param>
        /// <param name="action">an action function that is called when operation is invoked.</param>
        protected void RegisterMethod<TRequest, TResponse>(string name, Schema schema, UnaryServerMethod<TRequest, TResponse> action)
            where TRequest : class, IMessage<TRequest>, new()
            where TResponse : class, IMessage<TResponse>, new()
        {
            var actionWrapper = ApplyValidation(schema, action);
            actionWrapper = ApplyInterceptors(actionWrapper);

            AddMethod(name, actionWrapper);
        }

        protected void RegisterMethodWithAuth<TRequest, TResponse>(string name, Schema schema, Interceptor authorize, UnaryServerMethod<TRequest, TResponse> action)
            where TRequest : class, IMessage<TRequest>, new()
            where TResponse : class, IMessage<TResponse>, new()
        {
            var actionWrapper = ApplyValidation(schema, action);
            actionWrapper = (request, context) =>
            {
                return authorize.UnaryServerHandler(request, context, actionWrapper);
            };
            actionWrapper = ApplyInterceptors(actionWrapper);

            AddMethod(name, actionWrapper);
        }

        private void AddMethod<TRequest, TResponse>(string name, UnaryServerMethod<TRequest, TResponse> action)
            where TRequest : class, IMessage<TRequest>, new()
            where TResponse : class, IMessage<TResponse>, new()
        {
            var requestParser = GetOrCreateMessageParser<TRequest>();
            var responseParser = GetOrCreateMessageParser<TResponse>();

            var method = new Method<TRequest, TResponse>(
             MethodType.Unary,
              _serviceName,
              name,
              Marshallers.Create((arg) => arg != null ? arg.ToByteArray() : Array.Empty<byte>(), requestParser.ParseFrom),
              Marshallers.Create((arg) => arg != null ? arg.ToByteArray() : Array.Empty<byte>(), responseParser.ParseFrom));

            _builder.AddMethod(method, action);
        }

        private MessageParser<T> GetOrCreateMessageParser<T>()
          where T : class, IMessage<T>, new()
        {
            if (_messageParsers.TryGetValue(typeof(T), out object o_parser))
                return o_parser as MessageParser<T>;

            MessageParser<T> parser = new MessageParser<T>(() => new T());
            _messageParsers.Add(typeof(T), parser);

            return parser;
        }

        /// <summary>
        /// Registers all service routes in gRPC endpoint.
        /// 
        /// This method is called by the service and must be overriden
        /// in child classes.
        /// </summary>
        protected virtual void OnRegister()
        {
        }

        public void Register()
        {
            OnRegister();

            var serviceDefinitions = _builder.Build();

            _endpoint.RegisterService(serviceDefinitions);
        }
    }
}
