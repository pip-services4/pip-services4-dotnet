using PipServices4.Commons.Convert;
using PipServices4.Commons.Errors;
using PipServices4.Commons.Mapper;
using PipServices4.Components.Context;
using PipServices4.Grpc.Services;
using System;
using System.Threading.Tasks;

namespace PipServices4.Grpc.Clients
{
    /// <summary>
    /// Abstract client that calls commandable HTTP service.
    /// Commandable services are generated automatically for <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_commands_1_1_i_commandable.html">ICommandable</a> objects. 
    /// Each command is exposed as POST operation that receives all parameters
    /// in body object.
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
    /// - retries:               number of retries (default: 3)
    /// - connect_timeout:       connection timeout in milliseconds(default: 10 sec)
    /// - timeout:               invocation timeout in milliseconds(default: 10 sec)
    /// 
    /// ### References ###
    /// 
    /// - *:logger:*:*:1.0         (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// - *:counters:*:*:1.0         (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_count_1_1_i_counters.html">ICounters</a> components to pass collected measurements
    /// - *:discovery:*:*:1.0        (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    /// </summary>
    /// <example>
    /// <code>
    /// class MyCommandableHttpClient: CommandableHttpClient, IMyClient 
    /// {
    ///     ...
    ///     public MyData GetData(IContext context, string id)
    ///     {
    ///         return await CallCommandAsync<DataPage<MyData>>(        
    ///         "get_data",
    ///         context,
    ///         new {mydata.id = id}
    ///     );        
    ///     }
    /// ...
    /// }
    /// 
    /// var client = new MyCommandableHttpClient();
    /// client.Configure(ConfigParams.fromTuples(
    /// "connection.protocol", "http",
    /// "connection.host", "localhost",
    /// "connection.port", 8080 ));
    /// 
    /// var data = client.GetData("123", "1");
    /// ...
    /// </code>
    /// </example>
    public class CommandableGrpcClient: GrpcClient
    {
        protected string _name;

        public CommandableGrpcClient()
            : this("commandable.Commandable")
        { 
        }
        /// <summary>
        /// Creates a new instance of the client.
        /// </summary>
        public CommandableGrpcClient(string name)
            : base(name)
        {
            _name = name;
        }

        /// <summary>
        /// Calls a remote method via gRPC commadable protocol.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <param name="name">a name of the command to call.</param>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="requestEntity">body object.</param>
        /// <returns>result of the command.</returns>
        public async Task<T> CallCommandAsync<T>(string name, IContext context, object requestEntity)
            where T : class
        {
            var method = _name + '.' + name;
            var request = new InvokeRequest
            {
                Method = method,
                TraceId = context != null ? ContextResolver.GetTraceId(context) : null,
                ArgsEmpty = requestEntity == null,
                ArgsJson = requestEntity == null ? null : JsonConverter.ToJson(requestEntity)
            };

            InvokeReply response;

            var timing = Instrument(context, method);
            try
            {
                response = await CallAsync<InvokeRequest, InvokeReply>("invoke", request);
            }
            catch (Exception ex)
            {
                InstrumentError(context, method, ex);
                throw ex;
            }
            finally
            {
                timing.EndTiming();
            }

            // Handle error response
            if (response.Error != null)
            {
                var errorDescription = ObjectMapper.MapTo<Commons.Errors.ErrorDescription>(response.Error);
                throw ApplicationExceptionFactory.Create(errorDescription);
            }

            // Handle empty response
            if (response.ResultEmpty || response.ResultJson == null)
            {
                return null;
            }

            // Handle regular response
            var result = JsonConverter.FromJson<T>(response.ResultJson);

            return result;
        }
    }
}