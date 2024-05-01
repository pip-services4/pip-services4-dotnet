using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;

namespace PipServices4.Http.Build
{
    /// <summary>
    /// Creates RPC components by their descriptors.
    /// </summary>
    /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_build_1_1_factory.html">Factory</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-http-dotnet/class_pip_services_1_1_http_1_1_services_1_1_http_endpoint.html">HttpEndpoint</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-http-dotnet/class_pip_services_1_1_http_1_1_services_1_1_status_rest_controller.html">StatusRestController</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-http-dotnet/class_pip_services_1_1_http_1_1_services_1_1_heartbeat_rest_controller.html">HeartbeatRestController</a>
    public class DefaultHttpFactory : Factory
    {
        public static Descriptor Descriptor = new Descriptor("pip-services", "factory", "rpc", "default", "1.0");
        public static Descriptor HttpEndpointDescriptor = new Descriptor("pip-services", "endpoint", "http", "*", "1.0");
        public static Descriptor StatusControllerDescriptor = new Descriptor("pip-services", "status-controller", "http", "*", "1.0");
        public static Descriptor HeartbeatControllerDescriptor = new Descriptor("pip-services", "heartbeat-controller", "http", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultHttpFactory()
        {
            RegisterAsType(HttpEndpointDescriptor, typeof(HttpEndpoint));
            RegisterAsType(StatusControllerDescriptor, typeof(StatusRestController));
            RegisterAsType(HeartbeatControllerDescriptor, typeof(HeartbeatRestController));
        }
    }
}
