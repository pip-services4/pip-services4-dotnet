using Elasticsearch.Net;
using PipServices4.Commons.Convert;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Exec;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Config.Connect;
using PipServices4.Data.Keys;
using PipServices4.Observability.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Elasticsearch.Log
{
    /// <summary>
    /// Logger that dumps execution logs to ElasticSearch service.
    /// 
    /// ElasticSearch is a popular search index.It is often used 
    /// to store and index execution logs by itself or as a part of
    /// ELK (ElasticSearch - Logstash - Kibana) stack.
    /// 
    /// Authentication is not supported in this version.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// - level:             maximum log level to capture
    /// - source:            source (context) name
    /// 
    /// connection(s):
    /// - discovery_key:         (optional) a key to retrieve the connection from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
    /// - protocol:              connection protocol: http or https
    /// - host:                  host name or IP address
    /// - port:                  port number
    /// - uri:                   resource URI or connection string with all parameters in it
    /// 
    /// options:
    /// - interval:        interval in milliseconds to save log messages (default: 10 seconds)
    /// - max_cache_size:  maximum number of messages stored in this cache (default: 100)
    /// - index:           ElasticSearch index name (default: "log")
    /// - date_format      The date format to use when creating the index name. Eg. log-yyyyMMdd (default: "yyyyMMdd"). See <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings">Custom date and time format strings</a>
    /// - daily:           true to create a new index every day by adding date suffix to the index name(default: false)
    /// - reconnect:       reconnect timeout in milliseconds(default: 60 sec)
    /// - timeout:         invocation timeout in milliseconds(default: 30 sec)
    /// - max_retries:     maximum number of retries(default: 3)
    /// - index_message:   true to enable indexing for message object (default: false)
    /// 
    /// ### References ###
    /// 
    /// - *:context-info:*:*:1.0      (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_info_1_1_context_info.html">ContextInfo</a> to detect the context id and specify counters source
    /// - *:discovery:*:*:1.0         (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services to resolve connection
    /// </summary>
    /// <example>
    /// <code>
    /// var logger = new ElasticSearchLogger();
    /// logger.Configure(ConfigParams.FromTuples(
    /// "connection.protocol", "http",
    /// "connection.host", "localhost",
    /// "connection.port", 9200 ));
    /// logger.Open("123");
    /// 
    /// logger.Error("123", ex, "Error occured: %s", ex.message);
    /// logger.Debug("123", "Everything is OK.");
    /// </code>
    /// </example>
    public class ElasticSearchLogger : CachedLogger, IReferenceable, IOpenable
    {
        private ConsoleLogger _errorConsoleLogger = new ConsoleLogger { Level = LogLevel.Trace };
        private FixedRateTimer _timer;
        private HttpConnectionResolver _connectionResolver = new HttpConnectionResolver();
        private ElasticLowLevelClient _client;

        private string _indexName = "log";
        private string _dateFormat = "yyyyMMdd";
        private bool _dailyIndex = false;
        private string _currentIndexName;
        private double _reconnect = 60000;
        private double _timeout = 30000;
        private int _maxRetries = 3;
        private bool _indexMessage = false;

        /// <summary>
        /// Creates a new instance of the logger.
        /// </summary>
        public ElasticSearchLogger()
        { }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public override void Configure(ConfigParams config)
        {
            base.Configure(config);
            _connectionResolver.Configure(config);
            _errorConsoleLogger.Configure(config);

            _indexName = config.GetAsStringWithDefault("index", _indexName);
            _dateFormat = config.GetAsStringWithDefault ("date_format", _dateFormat);
            _dailyIndex = config.GetAsBooleanWithDefault("daily", _dailyIndex);
            _reconnect = config.GetAsDoubleWithDefault("options.reconnect", _reconnect);
            _timeout = config.GetAsDoubleWithDefault("options.timeout", _timeout);
            _maxRetries = config.GetAsIntegerWithDefault("options.max_retries", _maxRetries);
            _indexMessage = config.GetAsBooleanWithDefault("options.index_message", _indexMessage);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);
            _connectionResolver.SetReferences(references);
            _errorConsoleLogger.SetReferences(references);
        }

        /// <summary>
        /// Writes a log message to the logger destination.
        /// </summary>
        /// <param name="level">a log level.</param>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="error">an error object associated with this message.</param>
        /// <param name="message">a human-readable message to log.</param>
        protected override void Write(LogLevel level, IContext context, Exception error, string message)
        {
            if (Level < level)
            {
                return;
            }

            base.Write(level, context, error, message);
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public bool IsOpen()
        {
            return _timer != null;
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async Task OpenAsync(IContext context)
        {
            if (IsOpen()) return;

            var connection = await _connectionResolver.ResolveAsync(context);
            var uri = new Uri(connection.Uri);

            try
            {
                // Create client
                var settings = new ConnectionConfiguration(uri)
                    .RequestTimeout(TimeSpan.FromMilliseconds(_timeout))
                    .DeadTimeout(TimeSpan.FromMilliseconds(_reconnect))
                    .MaximumRetries(_maxRetries)
                    .ThrowExceptions(true);

                _client = new ElasticLowLevelClient(settings);

                // Create index if it doesn't exist
                await CreateIndex(context, true);

                if (_timer == null)
                {
                    _timer = new FixedRateTimer(OnTimer, _interval, _interval);
                    _timer.Start();
                }
            }
            catch
            {
                // Do nothing if elastic search client was not initialized
                _errorConsoleLogger.Error(context, $"Failed to initialize Elastic Search Logger with uri='{uri}'");
            }
        }

        private string GetCurrentIndex() 
        {
            if (!_dailyIndex) return _indexName;

            var today = DateTime.UtcNow.Date;
            var datePattern = today.ToString(_dateFormat);
            return _indexName + "-" + datePattern;
        }

        private async Task CreateIndex(IContext context, bool force)
        {
            var newIndex = GetCurrentIndex();
            if (!force && _currentIndexName == newIndex) return;

            _currentIndexName = newIndex;
            var response = await _client.Indices.ExistsAsync<StringResponse>(_currentIndexName);
            if (response.HttpStatusCode == 404)
            {
                var request = new
                {
                    settings = new
                    {
                        number_of_shards = 1
                    },
                    mappings = new
                    {
                        log_message = new
                        {
                            properties = new
                            {
                                time = new { type = "date", index = true },
                                source = new { type = "keyword", index = true },
                                level = new { type = "keyword", index = true },
                                correlation_id = new { type = "text", index = true },
                                error = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        type = new { type = "keyword", index = true },
                                        category = new { type = "keyword", index = true },
                                        status = new { type = "integer", index = false },
                                        code = new { type = "keyword", index = true },
                                        message = new { type = "text", index = false },
                                        details = new { type = "object" },
                                        correlation_id = new { type = "text", index = false },
                                        cause = new { type = "text", index = false },
                                        stack_trace = new { type = "text", index = false }
                                    }
                                },
                                message = new { type = "text", index = _indexMessage }
                            }
                        }
                    }
                };
                var json = JsonConverter.ToJson(request);
                try
                {
                    response = await _client.Indices.CreateAsync<StringResponse>(_currentIndexName, PostData.String(json));
                    if (!response.Success)
                        throw new ConnectionException(context != null ? ContextResolver.GetTraceId(context) : null,
                            "CANNOT_CREATE_INDEX", response.Body);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("resource_already_exists"))
                        throw;
                }
            }
            else if (!response.Success)
            {
                throw new ConnectionException(context != null ? ContextResolver.GetTraceId(context) : null,
                    "CONNECTION_FAILED", response.Body);
            }
        }

        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async Task CloseAsync(IContext context)
        {
            // Log all remaining messages before closing
            Dump();

            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            _client = null;

            await Task.Delay(0);
        }

        private void OnTimer()
        {
            Dump();
        }

        /// <summary>
        /// Saves log messages from the cache.
        /// </summary>
        /// <param name="messages">a list with log messages</param>
        protected override void Save(List<LogMessage> messages)
        {
            if (messages == null || messages.Count == 0) return;

            if (_client == null)
            {
                throw new InvalidStateException("elasticsearch_logger", "NOT_OPENED", "ElasticSearchLogger is not opened");
            }

            lock (_lock)
            {
                CreateIndex(Context.FromTraceId("elasticsearch_logger"), false).Wait();

                var bulk = new List<string>();
                foreach (var message in messages)
                {
                    bulk.Add(JsonConverter.ToJson(new { index = new { _index = _currentIndexName, _type = "log_message", _id = IdGenerator.NextLong() } }));
                    bulk.Add(JsonConverter.ToJson(message));
                }

                try
                {
                    var response = _client.Bulk<StringResponse>(PostData.MultiJson(bulk));
                    if (!response.Success)
                    {
                        throw new InvocationException("elasticsearch_logger", "REQUEST_FAILED", response.Body);
                    }
                }
                catch
                {
                    // Do nothing if elastic search client was not enable to process bulk of messages
                    _errorConsoleLogger.Error(null, "Failed to bulk messages with Elastic Search Logger.");
                }
            }
        }
    }
}
