using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Prometheus.Count;
using PipServices4.Prometheus.Controllers;

namespace PipServices4.Prometheus.Build
{
    /// <summary>
    /// Creates Prometheus components by their descriptors.
    /// </summary>
    /// See <a href="https://pip-services-dotnet.github.io/pip-services-components-dotnet/class_pip_services_1_1_components_1_1_build_1_1_factory.html">Factory</a>, 
    /// <a href="https://pip-services-dotnet.github.io/pip-services-prometheus-dotnet/class_pip_services_1_1_prometheus_1_1_count_1_1_prometheus_counters.html">PrometheusCounters</a>, 
    /// <a href="https://pip-services-dotnet.github.io/pip-services-prometheus-dotnet/class_pip_services_1_1_prometheus_1_1_services_1_1_prometheus_metrics_service.html">PrometheusMetricsService</a>
    public class DefaultPrometheusFactory: Factory
    {
        public static readonly Descriptor Descriptor = new Descriptor("pip-services", "factory", "prometheus", "default", "1.0");
        public static readonly Descriptor PrometheusCountersDescriptor = new Descriptor("pip-services", "counters", "prometheus", "*", "1.0");
        public static readonly Descriptor PrometheusMetricsControllerDescriptor = new Descriptor("pip-services", "metrics-controller", "prometheus", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultPrometheusFactory()
        {
            RegisterAsType(DefaultPrometheusFactory.PrometheusCountersDescriptor, typeof(PrometheusCounters));
            RegisterAsType(DefaultPrometheusFactory.PrometheusMetricsControllerDescriptor, typeof(PrometheusMetricsController));
        }
    }
}
