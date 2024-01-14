using PipServices4.Azure.Config;
using PipServices4.Azure.Count;
using PipServices4.Azure.Lock;
using PipServices4.Azure.Log;
using PipServices4.Azure.Metrics;
using PipServices4.Azure.Queues;
using PipServices4.Components.Build;
using PipServices4.Components.Refer;

namespace PipServices4.Azure.Build
{
    public class DefaultAzureFactory : Factory
    {
        public static Descriptor Descriptor = new Descriptor("pip-services", "factory", "azure", "default", "1.0");

        public static Descriptor StorageMessageQueueDescriptor = new Descriptor("pip-services", "message-queue", "storage-message-queue", "default", "1.0");
        public static Descriptor ServiceBusMessageQueueDescriptor = new Descriptor("pip-services", "message-queue", "servicebus-message-queue", "*", "1.0");
        public static Descriptor ServiceBusMessageTopicDescriptor = new Descriptor("pip-services", "message-queue", "servicebus-message-topic", "*", "1.0");
        public static Descriptor KeyVaultConfigReaderDescriptor = new Descriptor("pip-services", "config-reader", "key-vault", "*", "1.0");
        public static Descriptor AppInsightsCountersDescriptor = new Descriptor("pip-services", "counters", "app-insights", "*", "1.0");
        public static Descriptor AppInsightsLoggerDescriptor = new Descriptor("pip-services", "logger", "app-insights", "*", "1.0");
        public static Descriptor CloudStorageTableLockDescriptor = new Descriptor("pip-services", "lock", "storage-table", "*", "1.0");
        public static Descriptor CosmosDbMetricsServiceDescriptor = new Descriptor("pip-services", "metrics-service", "cosmosdb", "*", "1.0");

        public DefaultAzureFactory()
        {
            RegisterAsType(StorageMessageQueueDescriptor, typeof(StorageMessageQueue));
            RegisterAsType(ServiceBusMessageQueueDescriptor, typeof(ServiceBusMessageQueue));
            RegisterAsType(ServiceBusMessageTopicDescriptor, typeof(ServiceBusMessageTopic));
            RegisterAsType(KeyVaultConfigReaderDescriptor, typeof(KeyVaultConfigReader));
            RegisterAsType(AppInsightsCountersDescriptor, typeof(AppInsightsCounters));
            RegisterAsType(AppInsightsLoggerDescriptor, typeof(AppInsightsLogger));
            RegisterAsType(CloudStorageTableLockDescriptor, typeof(CloudStorageTableLock));
            RegisterAsType(CosmosDbMetricsServiceDescriptor, typeof(CosmosDbMetricsService));
        }
    }
}
