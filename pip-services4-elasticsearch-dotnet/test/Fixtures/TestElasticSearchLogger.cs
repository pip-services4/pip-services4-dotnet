using PipServices4.Elasticsearch.Log;
using PipServices4.Observability.Log;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PipServices4.Elasticsearch.Test.Fixtures
{
    /// <summary>
    /// Captures the output from TestESLoggers so the results can be inspected
    /// </summary>
    /// <remarks>
    /// We could easily make this for all CachedLoggers except ElasticSearch has an OpenAsync()
    /// and we'd need to play around with the test fixtures</remarks>
    public class TestElasticSearchLogger : ElasticSearchLogger
    {
        public ConcurrentQueue<List<LogMessage>> SavedMessages { get; } = new ConcurrentQueue<List<LogMessage>>();
        
        protected override void Save(List<LogMessage> messages)
        {
            SavedMessages.Enqueue(messages);
        }

        public void RemoveAllSavedOutput()
        {
            SavedMessages.Clear();
        }
    }
}
