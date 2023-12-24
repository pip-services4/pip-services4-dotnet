using System;
using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Elasticsearch.Log;

namespace PipServices4.Elasticsearch.Build
{
    /// <summary>
    /// Creates ElasticSearch components by their descriptors.
    /// </summary>
    /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-elasticsearch-dotnet/class_pip_services_1_1_elastic_search_1_1_log_1_1_elastic_search_logger.html">ElasticSearchLogger</a>
    public class DefaultElasticSearchFactory: Factory
    {
        public static readonly Descriptor Descriptor = new Descriptor("pip-services", "factory", "elasticsearch", "default", "1.0");
        public static readonly Descriptor Descriptor3 = new Descriptor("pip-services4", "factory", "elasticsearch", "default", "1.0");
        public static readonly Descriptor ElasticSearchLoggerDescriptor = new Descriptor("pip-services", "logger", "elasticsearch", "*", "1.0");
        public static readonly Descriptor ElasticSearchLogger4Descriptor = new Descriptor("pip-services4", "logger", "elasticsearch", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultElasticSearchFactory()
        {
            RegisterAsType(ElasticSearchLoggerDescriptor, typeof(ElasticSearchLogger));
            RegisterAsType(ElasticSearchLogger4Descriptor, typeof(ElasticSearchLogger));
        }
    }
}
