using PipServices4.Azure.Metrics.Data;
using PipServices4.Commons.Convert;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Observability.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipServices4.Azure.Metrics
{
    public class CosmosDbMetricsService : IReferenceable, IConfigurable, ICosmosDbMetricsService
    {
        private string ClientId { get; set; }
        private string ClientSecret { get; set; }
        private string SubscriptionId { get; set; }
        private string TenantId { get; set; }
        private string ApiVersion { get; set; }
        private CosmosDbClientCredentials ClientCredentials { get; set; }
        public CosmosDbServiceClient ServiceClient { get; set; }

        private const string DocumentDBCollectionUriFormat = "subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.DocumentDB/databaseAccounts/{2}/databases/{3}/collections/{4}";

        private CompositeLogger _logger = new CompositeLogger();

        public void Configure(ConfigParams config)
        {
            ClientId = config.GetAsString("credential.client_id");
            ClientSecret = config.GetAsString("credential.client_secret");
            SubscriptionId = config.GetAsString("credential.subscription_id");
            TenantId = config.GetAsString("credential.tenant_id");
            ApiVersion = config.GetAsString("credential.api_version");
        }

        public void SetReferences(IReferences references)
        {
            try
            {
                _logger.SetReferences(references);

                ClientSecret = ExtractClientSecret(ClientSecret);

                ClientCredentials = new CosmosDbClientCredentials(ClientId, ClientSecret, TenantId);
                ServiceClient = new CosmosDbServiceClient(ClientCredentials);
            }
            catch (Exception exception)
            {
                ServiceClient = null;
                _logger.Error(Context.FromTraceId("CosmosDbMetricsService"), exception, $"SetReferences: Failed to initialize cosmos db metrics service.");
            }
        }

        public string GetResourceUri(IContext context, string resourceGroupName, string accountName, string accessKey, string databaseName, string collectionName)
        {
            if (ServiceClient == null)
            {
                return string.Empty;
            }

            try
            {
                var (databaseResourceId, collectionResourceId) = ServiceClient.GetResourceIDs(accountName, accessKey, databaseName, collectionName);
                return string.Format(DocumentDBCollectionUriFormat, SubscriptionId, resourceGroupName, accountName, databaseResourceId, collectionResourceId);
            }
            catch (Exception exception)
            {
                _logger.Error(context, exception, $"GetResourceUri: Failed to get uri for parameters: resourceGroupName = '{resourceGroupName}', " +
                    $"accountName = '{accountName}', accessKey = '{accessKey}, databaseName = '{databaseName}', collectionName = '{collectionName}'.");
                return string.Empty;
            }
        }

        public async Task<IEnumerable<Metric>> GetResourceMetricsAsync(IContext context, string resourceUri, Action<QueryBuilder> queryBuilderDelegate)
        {
            if (ServiceClient == null)
            {
                return null;
            }

            try
            {
                var jsonMetrics = await ServiceClient.GetMetricsAsync(resourceUri, ApiVersion, 
                    queryBuilder => queryBuilderDelegate.Invoke(queryBuilder));

                var metrics = JsonConverter.FromJson<MetricCollection>(jsonMetrics);
                return metrics.Value;
            }
            catch (Exception exception)
            {
                _logger.Error(context, exception, $"GetResourceMetricsAsync: Failed to get metrics for resource '{resourceUri}'.");
                return null;
            }
        }

        private string ExtractClientSecret(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
    }
}
