using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;
using PipServices4.Observability.Count;
using PipServices4.Prometheus.Count;
using System.Threading.Tasks;

namespace PipServices4.Prometheus.Controllers
{
    /// <summary>
    /// Service that exposes "/metrics" route for Prometheus to scap performance metrics.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// dependencies:
    /// - endpoint:              override for HTTP Endpoint dependency
    /// - prometheus-counters:   override for PrometheusCounters dependency
    /// 
    /// connection(s):
    /// - discovery_key:         (optional) a key to retrieve the connection from <a href="https://pip-services3-dotnet.github.io/pip-services3-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
    /// - protocol:              connection protocol: http or https
    /// - host:                  host name or IP address
    /// - port:                  port number
    /// - uri:                   resource URI or connection string with all parameters in it
    /// 
    /// ### References ###
    /// 
    /// - *:logger:*:*:1.0               (optional) <a href="https://pip-services3-dotnet.github.io/pip-services3-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// - *:counters:*:*:1.0             (optional) <a href="https://pip-services3-dotnet.github.io/pip-services3-components-dotnet/interface_pip_services_1_1_components_1_1_count_1_1_i_counters.html">ICounters</a> components to pass collected measurements
    /// - *:discovery:*:*:1.0            (optional) <a href="https://pip-services3-dotnet.github.io/pip-services3-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    /// - *:endpoint:http:*:1.0          (optional) <a href="https://pip-services3-dotnet.github.io/pip-services3-rpc-dotnet/class_pip_services_1_1_rpc_1_1_services_1_1_http_endpoint.html">HttpEndpoint</a> reference to expose REST operation
    /// - *:counters:prometheus:*:1.0    <a href="https://pip-services3-dotnet.github.io/pip-services3-prometheus-dotnet/class_pip_services_1_1_prometheus_1_1_count_1_1_prometheus_counters.html">PrometheusCounters</a> reference to retrieve collected metrics
    /// </summary>
    /// <example>
    /// <code>
    /// var service = new PrometheusMetricsService();
    /// service.Configure(ConfigParams.FromTuples(
    /// "connection.protocol", "http",
    /// "connection.host", "localhost",
    /// "connection.port", 8080 ));
    /// 
    /// service.Open("123");
    /// Console.Out.WriteLine("The Prometheus metrics service is accessible at http://+:8080/metrics");
    /// </code>
    /// </example>
    public class PrometheusMetricsController : RestController
    {
        private CachedCounters _cachedCounters;
        private string _source;
        private string _instance;

        /// <summary>
        /// Creates a new instance of this service.
        /// </summary>
        public PrometheusMetricsController()
        {
            _dependencyResolver.Put("cached-counters", new Descriptor("pip-services", "counters", "cached", "*", "1.0"));
            _dependencyResolver.Put("prometheus-counters", new Descriptor("pip-services", "counters", "prometheus", "*", "1.0"));
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);

            _cachedCounters = _dependencyResolver.GetOneOptional<PrometheusCounters>("prometheus-counters-3");

            if (_cachedCounters == null)
                _cachedCounters = _dependencyResolver.GetOneOptional<PrometheusCounters>("prometheus-counters");

            if (_cachedCounters == null)
                _cachedCounters = _dependencyResolver.GetOneOptional<CachedCounters>("cached-counters-3");

            if (_cachedCounters == null)
                _dependencyResolver.GetOneOptional<CachedCounters>("cached-counters");

            var contextInfo = references.GetOneOptional<ContextInfo>(
                new Descriptor("pip-services3", "context-info", "default", "*", "1.0"));

            if (contextInfo == null)
                references.GetOneOptional<ContextInfo>(
                    new Descriptor("pip-services", "context-info", "default", "*", "1.0"));
            if (contextInfo != null && string.IsNullOrEmpty(_source))
                _source = contextInfo.Name;
            if (contextInfo != null && string.IsNullOrEmpty(_instance))
                _instance = contextInfo.ContextId;
        }

        /// <summary>
        /// Registers all service routes in HTTP endpoint.
        /// </summary>
        public override void Register()
        {
            RegisterRoute("get", "metrics", Metrics);
        }

        private async Task Metrics(HttpRequest request, HttpResponse response, RouteData routeData)
        {
            var counters = _cachedCounters != null ? _cachedCounters.GetAll() : null;
            var body = PrometheusCounterConverter.ToString(counters, _source, _instance);

            response.StatusCode = 200;
            response.ContentType = "text/plain";
            await response.WriteAsync(body);
        }
    }
}
