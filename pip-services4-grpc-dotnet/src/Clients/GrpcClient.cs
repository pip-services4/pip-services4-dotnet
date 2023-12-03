using Google.Protobuf;
using Grpc.Core;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Config.Connect;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PipServices4.Grpc.Clients
{
    public class GrpcClient: IOpenable, IConfigurable, IReferenceable
	{
		private static readonly ConfigParams _defaultConfig = ConfigParams.FromTuples(
			"connection.protocol", "http",
			"connection.host", "0.0.0.0",
			"connection.port", 3000,

			"options.request_max_size", 1024 * 1024,
			"options.connect_timeout", 10000,
			"options.timeout", 10000,
			"options.retries", 3,
			"options.debug", true
		);

		protected CallInvoker _callInvoker;

		protected Channel _channel;

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
		/// he connection timeout in milliseconds.
		/// </summary>
		protected int _connectTimeout = 10000;

		protected string _serviceName;

		private readonly Dictionary<Type, object> _messageParsers = new Dictionary<Type, object>();
		private readonly Dictionary<string, object> _methods = new Dictionary<string, object>();

		public GrpcClient(string name = null)
		{
			_serviceName = name;
		}

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies.</param>
		public virtual void SetReferences(IReferences references)
		{
			_logger.SetReferences(references);
			_counters.SetReferences(references);
			_connectionResolver.SetReferences(references);
		}

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters to be set.</param>
		public virtual void Configure(ConfigParams config)
		{
			config = config.SetDefaults(_defaultConfig);
			_connectionResolver.Configure(config);
			_options = _options.Override(config.GetSection("options"));

			_connectTimeout = config.GetAsIntegerWithDefault("options.connect_timeout", _connectTimeout);
			//_timeout = config.GetAsIntegerWithDefault("options.timeout", _timeout);
		}

		/// <summary>
		/// Checks if the component is opened.
		/// </summary>
		/// <returns>true if the component has been opened and false otherwise.</returns>
		public virtual bool IsOpen()
		{
			return _channel != null;
		}

		/// <summary>
		/// Opens the component.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		public async virtual Task OpenAsync(IContext context)
		{
			var connection = await _connectionResolver.ResolveAsync(context);

			var host = connection.Host;
			var port = connection.Port;

			_channel = new Channel(string.Format("{0}:{1}", host, port), ChannelCredentials.Insecure);

			_callInvoker = new DefaultCallInvoker(_channel);
		}

		protected async Task<TResponse> CallAsync<TRequest, TResponse>(string name, TRequest request)
			where TRequest : class, IMessage<TRequest>, new()
			where TResponse : class, IMessage<TResponse>, new()
		{
			var method = GetOrCreateMethod<TRequest, TResponse>(name);
			var options = new CallOptions();

			return await _callInvoker.AsyncUnaryCall(method, null, options, request);
		}

		/// <summary>
		/// Creates a method definition to be called using GRPC.
		/// </summary>
		/// <typeparam name="TRequest">type of request message</typeparam>
		/// <typeparam name="TResponse">type of response message</typeparam>
		/// <param name="name">name of gRPC method</param>
		protected Method<TRequest, TResponse> GetOrCreateMethod<TRequest, TResponse>(string name)
			where TRequest : class, IMessage<TRequest>, new()
			where TResponse : class, IMessage<TResponse>, new()
		{
			if (!_methods.TryGetValue(name, out object method))
			{
				var requestParser = GetOrCreateMessageParser<TRequest>();
				var responseParser = GetOrCreateMessageParser<TResponse>();

				var clientMethod = new Method<TRequest, TResponse>(
				 MethodType.Unary,
				  _serviceName,
				  name,
				  Marshallers.Create((arg) => arg != null ? MessageExtensions.ToByteArray(arg) : Array.Empty<byte>(), requestParser.ParseFrom),
				  Marshallers.Create((arg) => arg != null ? MessageExtensions.ToByteArray(arg) : Array.Empty<byte>(), responseParser.ParseFrom));

				_methods.Add(name, clientMethod);
				return clientMethod;
			}

			return (Method<TRequest, TResponse>)method;
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
		/// Closes component and frees used resources.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		public async virtual Task CloseAsync(IContext context)
		{
			await _channel.ShutdownAsync();
		}

		/// <summary>
		/// Adds instrumentation to log calls and measure call time. It returns a CounterTiming
		/// object that is used to end the time measurement.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		/// <param name="methodName">a method name.</param>
		/// <returns>CounterTiming object to end the time measurement.</returns>
		protected CounterTiming Instrument(IContext context, [CallerMemberName] string methodName = null)
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
		protected void InstrumentError(IContext context, [CallerMemberName] string methodName = null, Exception ex = null, bool rethrow = false)
		{
			var typeName = GetType().Name;
			_logger.Error(context, ex, "Failed to call {0} method of {1}", methodName, typeName);
			_counters.IncrementOne(typeName + "." + methodName + ".call_errors");

			if (rethrow)
				throw ex;
		}

	}
}
