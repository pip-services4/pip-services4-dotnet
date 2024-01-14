using PipServices4.Azure.Metrics.Data;
using PipServices4.Components.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Azure.Metrics
{
    public interface ICosmosDbMetricsService
    {
        string GetResourceUri(IContext context, string resourceGroupName, string accountName, string accessKey, string databaseName, string collectionName);
        Task<IEnumerable<Metric>> GetResourceMetricsAsync(IContext context, string resourceUri, Action<QueryBuilder> oDataQueryBuilderDelegate);
    }
}