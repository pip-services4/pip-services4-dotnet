using Grpc.Core;
using PipServices4.Commons.Convert;
using PipServices4.Commons.Errors;
using PipServices4.Commons.Mapper;
using PipServices4.Components.Context;
using PipServices4.Components.Exec;
using PipServices4.Rpc.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Grpc.Services
{
    /// <summary>
    /// Abstract service that receives remove calls via HTTP/REST protocol
    /// to operations automatically generated for commands defined in <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_commands_1_1_i_commandable.html">ICommandable</a> components.
    /// Each command is exposed as POST operation that receives all parameters in body object.
    /// 
    /// Commandable services require only 3 lines of code to implement a robust external
    /// HTTP-based remote interface.
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
    /// - *:logger:*:*:1.0         	(optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// - *:counters:*:*:1.0        (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_count_1_1_i_counters.html">ICounters</a> components to pass collected measurements
    /// - *:discovery:*:*:1.0       (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    /// - *:endpoint:http:*:1.0     (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-rpc-dotnet/class_pip_services_1_1_rpc_1_1_services_1_1_http_endpoint.html">HttpEndpoint</a> reference
    /// </summary>
    /// <example>
    /// <code>
    /// class MyCommandableHttpService: CommandableHttpService 
    /// {
    ///     public MyCommandableHttpService()
    ///     {
    ///         base();
    ///         this._dependencyResolver.put(
    ///         "controller", new Descriptor("mygroup", "controller", "*", "*", "1.0") );
    ///     }
    /// }
    /// 
    /// var service = new MyCommandableHttpService();
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
    public class CommandableGrpcService : GrpcService
	{
		private Dictionary<string, Func<string, Parameters, Task<object>>> _commandableMethods;

		/// <summary>
		/// Creates a new instance of the service.
		/// </summary>
		/// <param name="name">service name</param>
		public CommandableGrpcService(string name = null)
			: base(name ?? "commandable.Commandable")
		{
			_dependencyResolver.Put("controller", "none");
		}

		public override Task CloseAsync(IContext context)
		{
			_commandableMethods = null;

			return base.CloseAsync(context);
		}

		/// <summary>
		/// Registers all service routes in gRPC endpoint.
		/// </summary>
		protected override void OnRegister()
		{
			RegisterMethod<InvokeRequest, InvokeReply>("invoke", null, InvokeAsync);
			RegisterCommandableMethods();
		}

		private void RegisterCommandableMethods()
		{
			var controller = _dependencyResolver.GetOneRequired<ICommandable>("controller");
			var commands = controller.GetCommandSet().Commands;

			_commandableMethods = _commandableMethods ?? new Dictionary<string, Func<string, Parameters, Task<object>>>();

			foreach (var command in commands)
			{
				var method = _serviceName + '.' + command.Name;

				_commandableMethods[method] = async (traceId, args) =>
				{
					var context = Context.FromTraceId(traceId);
					using (var timing = Instrument(context, method))
					{
						return await command.ExecuteAsync(context, args);
					}
				};
			}
		}

		protected async Task<InvokeReply> InvokeAsync(InvokeRequest request, ServerCallContext context)
		{
			var method = request.Method;
			var traceId = request.TraceId;
			var action = _commandableMethods?[method];

			// Handle method not found
			if (action == null)
			{
				var err = new InvocationException(traceId, "METHOD_NOT_FOUND", "Method " + method + " was not found")
					.WithDetails("method", method);

				return CreateErrorResponse(err);
			}

			try
			{
				// Convert arguments
				var argsEmpty = request.ArgsEmpty;
				var argsJson = request.ArgsJson;
				var args = !argsEmpty && !string.IsNullOrWhiteSpace(argsJson)
					? Parameters.FromJson(argsJson)
					: new Parameters();

				// Todo: Validate schema
				//var schema = this._commandableSchemas[method];
				//if (schema)
				//{
				//    //...
				//}

				// Call command action
				var result = await action(traceId, args);

				// Process result and generate response
				var response = new InvokeReply
				{
					Error = null,
					ResultEmpty = result == null
				};

				if (result != null)
				{
					response.ResultJson = JsonConverter.ToJson(result);
				}

				return response;
			}
			catch (Exception ex)
			{
				// Handle unexpected exception
				var err = new InvocationException(traceId, "METHOD_FAILED", "Method " + method + " failed")
					.Wrap(ex).WithDetails("method", method);

				return CreateErrorResponse(err);
			}
		}

		private InvokeReply CreateErrorResponse(Exception ex)
		{
			return new InvokeReply
			{
				Error = ObjectMapper.MapTo<ErrorDescription>(ErrorDescriptionFactory.Create(ex)),
				ResultEmpty = true,
				ResultJson = null
			};
		}
	}
}