using System.Collections.Generic;

namespace PipServices4.Azure.Metrics.Data
{
    public class MetricCollection
    {
        public IList<Metric> Value { get; set; } = new List<Metric>();
    }
}
