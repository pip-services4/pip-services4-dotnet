using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Exec;

namespace PipServices4.Config.Config
{
	/// <summary>
	/// Config reader that stores configuration in memory.
	/// 
	/// The reader supports parameterization using Handlebars template engine.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// The configuration parameters are the configuration template
	/// </summary>
	/// <example>
	/// <code>
	/// var config = ConfigParams.FromTuples(
	/// "connection.host", "{{SERVICE_HOST}}",
	/// "connection.port", "{{SERVICE_PORT}}{{^SERVICE_PORT}}8080{{/SERVICE_PORT}}"
	/// );
	/// 
	/// var configReader = new MemoryConfigReader();
	/// configReader.Configure(config);
	/// 
	/// var parameters = ConfigParams.fromValue(process.env);
	/// configReader.ReadConfig("123", parameters);
	/// </code>
	/// </example>
	/// See <see cref="IConfigReader"/>
	public class MemoryConfigReader : IConfigReader, IReconfigurable
    {
        protected ConfigParams _config = new ConfigParams();

        /// <summary>
        /// Creates a new instance of config reader.
        /// </summary>
        /// <param name="config">(optional) component configuration parameters</param>
        public MemoryConfigReader(ConfigParams config = null)
        {
            _config = config ?? new ConfigParams();
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">Configures component by passing configuration parameters.</param>
        public virtual void Configure(ConfigParams config)
        {
            _config = config;
        }

        /// <summary>
        /// Reads configuration and parameterize it with given values.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="parameters">values to parameters the configuration or null to skip parameterization.</param>
        /// <returns>ConfigParams configuration.</returns>
        public virtual ConfigParams ReadConfig(IContext context, ConfigParams parameters)
        {
            return new ConfigParams(_config);
        }

        /// <summary>
        /// Adds a listener that will be notified when configuration is changed
        /// </summary>
        /// <param name="listener">a listener to be added.</param>
        public virtual void AddChangeListener(INotifiable listener)
        {
            // Do nothing...
        }

        /// <summary>
        /// Remove a previously added change listener.
        /// </summary>
        /// <param name="listener">a listener to be removed.</param>
        public virtual void RemoveChangeListener(INotifiable listener)
        {
            // Do nothing...
        }
    }
}
