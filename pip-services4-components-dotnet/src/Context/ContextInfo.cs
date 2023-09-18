using System;
using Newtonsoft.Json;
using PipServices4.Commons.Data;
using PipServices4.Components.Config;

namespace PipServices4.Components.Context
{
    /// <summary>
    /// Context information component that provides detail information
    /// about execution context: container or/and process.
    /// 
    /// Most often ContextInfo is used by logging and performance counters
    /// to identify source of the collected logs and metrics.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// - name: 					the context (container or process) name
    /// - description: 		   	human-readable description of the context
    /// - properties: 			entire section of additional descriptive properties
    /// - ...
    /// </summary>
    /// <example>
    /// <code>
    /// var contextInfo = new ContextInfo();
    /// contextInfo.Configure(ConfigParams.FromTuples(
    /// "name", "MyMicroservice",
    /// "description", "My first microservice"
    /// ));
    /// 
    /// context.Name;			// Result: "MyMicroservice"
    /// context.ContextId;		// Possible result: "mylaptop"
    /// context.StartTime;		// Possible result: 2018-01-01:22:12:23.45Z
    /// context.Uptime;			// Possible result: 3454345
    /// </code>
    /// </example>
    public sealed class ContextInfo : IReconfigurable
    {
        private string _name = "unknown";
        private StringValueMap _properties = new StringValueMap();

        /// <summary>
        /// Creates a new instance of this context info.
        /// </summary>
        public ContextInfo()
            : this(null, null)
        { }

        /// <summary>
        /// Creates a new instance of this context info.
        /// </summary>
        /// <param name="name">(optional) a context name.</param>
        /// <param name="description">(optional) a human-readable description of the context.</param>
        public ContextInfo(string name = null, string description = null)
        {
            _name = name ?? "unknown";
            Description = description;
        }

        /// <summary>
        /// Gets or sets the context name.
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value ?? "unknown"; }
        }

        /// <summary>
        /// Gets or sets the human-readable description of the context.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = null;

        /// <summary>
        /// Gets or sets the unique context id. Usually it is the current host name.
        /// </summary>
        [JsonProperty("context_id")]
        public string ContextId { get; set; } = Environment.MachineName; // IdGenerator.NextLong();

        /// <summary>
        /// Gets or sets the context start time.
        /// </summary>
        [JsonProperty("start_time")]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Calculates the context uptime as from the start time.
        /// </summary>
        [JsonProperty("uptime")]
        public long Uptime
        {
            get
            {
                return DateTime.UtcNow.Ticks - StartTime.Ticks;
            }
        }

        /// <summary>
        /// Gets or sets context additional parameters.
        /// </summary>
        [JsonProperty("properties")]
        public StringValueMap Properties
        {
            get { return _properties; }
            set { _properties = value ?? new StringValueMap(); }
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public void Configure(ConfigParams config)
        {
            Name = config.GetAsStringWithDefault("name", Name);
            Name = config.GetAsStringWithDefault("info.name", Name);

            Description = config.GetAsStringWithDefault("description", Description);
            Description = config.GetAsStringWithDefault("info.description", Description);

            Properties = config.GetSection("properties");
        }

        /// <summary>
        /// Creates a new ContextInfo and sets its configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters for the new ContextInfo.</param>
        /// <returns>a newly created ContextInfo</returns>
        public static ContextInfo FromConfig(ConfigParams config)
        {
            var result = new ContextInfo();
            result.Configure(config);
            return result;
        }
    }
}
