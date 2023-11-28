using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Datadog.Clients;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PipServices4.Datadog.Count
{
    /// <summary>
    /// Performance counters that send their metrics to DataDog service.
    /// 
    /// DataDog is a popular monitoring SaaS service. It collects logs, metrics, events
    /// from infrastructure and applications and analyze them in a single place.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// - connection(s):           
    ///   - discovery_key:         (optional) a key to retrieve the connection from [[https://pip-services4-node.github.io/pip-services4-components-node/interfaces/connect.idiscovery.html IDiscovery]]
    ///     - protocol:            (optional) connection protocol: http or https (default: https)
    ///     - host:                (optional) host name or IP address (default: api.datadoghq.com)
    ///     - port:                (optional) port number (default: 443)
    ///     - uri:                 (optional) resource URI or connection string with all parameters in it
    /// - credential:
    ///     - access_key:          DataDog client api key
    /// - options:
    ///   - retries:               number of retries (default: 3)
    ///   - connect_timeout:       connection timeout in milliseconds (default: 10 sec)
    ///   - timeout:               invocation timeout in milliseconds (default: 10 sec)
    /// 
    /// ### References ###
    /// 
    /// - <code>\*:logger:\*:\*:1.0</code>         (optional) [[https://pip-services4-node.github.io/pip-services4-components-node/interfaces/log.ilogger.html ILogger]] components to pass log messages
    /// - <code>\*:counters:\*:\*:1.0</code>         (optional) [[https://pip-services4-node.github.io/pip-services4-components-node/interfaces/count.icounters.html ICounters]] components to pass collected measurements
    /// - <code>\*:discovery:\*:\*:1.0</code>        (optional) [[https://pip-services4-node.github.io/pip-services4-components-node/interfaces/connect.idiscovery.html IDiscovery]] services to resolve connection
    /// 
    /// @see [[https://pip-services4-node.github.io/pip-services4-rpc-node/classes/services.restservice.html RestService]]
    /// @see [[https://pip-services4-node.github.io/pip-services4-rpc-node/classes/services.commandablehttpservice.html CommandableHttpService]]
    /// </summary>
    /// <example>
    /// <code>
    ///      let counters = new DataDogCounters();
    ///      counters.Configure(ConfigParams.FromTuples(
    ///          "credential.access_key", "827349874395872349875493"
    ///      ));
    ///  
    ///      await counters.OpenAsync("123", (err) => {
    ///          ...
    ///      });
    ///  
    ///      counters.Increment("mycomponent.mymethod.calls");
    ///      let timing = counters.BeginTiming("mycomponent.mymethod.exec_time");
    ///      try {
    ///          ...
    ///      } finally {
    ///          timing.EndTiming();
    ///      }
    ///  
    ///      counters.Dump();
    /// </code>
    /// </example>
	public class DataDogCounters: CachedCounters, IReferenceable, IOpenable
	{
        private DataDogMetricsClient _client = new DataDogMetricsClient();
        private CompositeLogger _logger = new CompositeLogger();
        private bool _opened = false;
        private string _source;
        private string _instance = Dns.GetHostName();
        private string _requestRoute;

        public DataDogCounters()
            : base()
        { 
        }

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters to be set.</param>
		public override void Configure(ConfigParams config)
		{
			base.Configure(config);

            _client.Configure(config);

            _source = config.GetAsStringWithDefault("source", _source);
            _instance = config.GetAsStringWithDefault("instance", _instance);
        }

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies. </param>
		public void SetReferences(IReferences references)
		{
            _logger.SetReferences(references);
            _client.SetReferences(references);

            var contextInfo = references.GetOneOptional<ContextInfo>(
                new Descriptor("pip-services", "context-info", "default", "*", "1.0"));
            if (contextInfo != null && _source == null)
                _source = contextInfo.Name;
            if (contextInfo != null && _instance == null)
                _instance = contextInfo.ContextId;
        }

		/// <summary>
		/// Checks if the component is opened.
		/// </summary>
		/// <returns>true if the component has been opened and false otherwise.</returns>
		public bool IsOpen()
		{
			return _opened;
		}

		/// <summary>
		/// Opens the component.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		public async Task OpenAsync(IContext context)
		{
			if (_opened)
			{
				return;
			}

			_opened = true;

			await _client.OpenAsync(context);
		}

		/// <summary>
		/// Closes component and frees used resources.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		public async Task CloseAsync(IContext context)
		{
			_opened = false;

			await _client.CloseAsync(context);
		}

		private List<DataDogMetric> ConvertCounter(Counter counter)
		{
            switch (counter.Type)
            {
                case CounterType.Increment:
                    return new List<DataDogMetric>
                    { 
                        new DataDogMetric
                        {
                            Metric = counter.Name,
                            Type = DataDogMetricType.Gauge,
                            Host = _instance,
                            Service = _source,
                            Points = new List<DataDogMetricPoint>
                            {
                                new DataDogMetricPoint
                                {
                                    Time = counter.Time,
                                    Value = counter.Count
                                }
                            }
                        }
                    };
                case CounterType.LastValue:
                    return new List<DataDogMetric>
                    {
                        new DataDogMetric
                        {
                            Metric = counter.Name,
                            Type = DataDogMetricType.Gauge,
                            Host = _instance,
                            Service = _source,
                            Points = new List<DataDogMetricPoint>
                            {
                                new DataDogMetricPoint
                                {
                                    Time = counter.Time,
                                    Value = counter.Last
                                }
                            }
                        }
                    };
                case CounterType.Interval:
                case CounterType.Statistics:
                    return new List<DataDogMetric>
                    {
                        new DataDogMetric
                        {
                            Metric = counter.Name + ".min",
                            Type = DataDogMetricType.Gauge,
                            Host = _instance,
                            Service = _source,
                            Points = new List<DataDogMetricPoint>
                            {
                                new DataDogMetricPoint
                                {
                                    Time = counter.Time,
                                    Value = counter.Min
                                }
                            }
                        },
                        new DataDogMetric
                        {
                            Metric = counter.Name + ".average",
                            Type = DataDogMetricType.Gauge,
                            Host = _instance,
                            Service = _source,
                            Points = new List<DataDogMetricPoint>
                            {
                                new DataDogMetricPoint
                                {
                                    Time = counter.Time,
                                    Value = counter.Average
                                }
                            }
                        },
                        new DataDogMetric
                        {
                            Metric = counter.Name + ".max",
                            Type = DataDogMetricType.Gauge,
                            Host = _instance,
                            Service = _source,
                            Points = new List<DataDogMetricPoint>
                            {
                                new DataDogMetricPoint
                                {
                                    Time = counter.Time,
                                    Value = counter.Max
                                }
                            }
                        }
                    };
            }

            return new List<DataDogMetric>(0);
        }

        private List<DataDogMetric> ConvertCounters(IEnumerable<Counter> counters)
        {
            var metrics = new List<DataDogMetric>();

            counters
                .Select(c => ConvertCounter(c))
                .ToList()
                .ForEach(m => metrics.AddRange(m));

            return metrics;
        }

        /// <summary>
        /// Saves the current counters measurements.
        /// </summary>
        /// <param name="counters">current counters measurements to be saves.</param>
		protected override void Save(IEnumerable<Counter> counters)
		{
            var metrics = ConvertCounters(counters);
            if (metrics.Count == 0) return;

            try
            {
                _client.SendMetricsAsync(Context.FromTraceId("datadog-counters"), metrics).Wait();
            }
            catch (Exception ex)
            {
                _logger.Error(Context.FromTraceId("datadog-counters"), ex, "Failed to push metrics to DataDog");
            }
        }
	}
}
