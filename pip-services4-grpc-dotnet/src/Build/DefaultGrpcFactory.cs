using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Grpc.Controllers;

namespace PipServices4.Grpc.Build
{
    /// <summary>
    /// Creates GRPC components by their descriptors.
    /// </summary>
    /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_build_1_1_factory.html">Factory</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-rpc-dotnet/class_pip_services_1_1_rpc_1_1_services_1_1_http_endpoint.html">HttpEndpoint</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-rpc-dotnet/class_pip_services_1_1_rpc_1_1_services_1_1_status_rest_service.html">StatusRestService</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-rpc-dotnet/class_pip_services_1_1_rpc_1_1_services_1_1_heartbeat_rest_service.html">HeartbeatRestService</a>
    public class DefaultGrpcFactory : Factory
    {
        public static Descriptor Descriptor = new Descriptor("pip-services", "factory", "grpc", "default", "1.0");
        public static Descriptor GrpcEndpointDescriptor = new Descriptor("pip-services", "endpoint", "grpc", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultGrpcFactory()
        {
            RegisterAsType(GrpcEndpointDescriptor, typeof(GrpcEndpoint));
        }
    }
}
