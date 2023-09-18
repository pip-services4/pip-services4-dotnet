using PipServices4.Components.Config;
using PipServices4.Components.Exec;

namespace PipServices4.Config.Config
{
	/// <summary>
	/// Interface for configuration readers that retrieve configuration from various sources
	/// and make it available for other components.
	/// 
	/// Some IConfigReader implementations may support configuration parameterization.
	/// The parameterization allows to use configuration as a template and inject there dynamic values.
	/// The values may come from application command like arguments or environment variables.
	/// </summary>
	public interface IConfigReader
    {
        /// <summary>
        /// Reads configuration and parameterize it with given values.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="parameters">values to parameters the configuration or null to skip parameterization.</param>
        /// <returns>ConfigParams configuration.</returns>
        ConfigParams ReadConfig(string correlationId, ConfigParams parameters);

        /// <summary>
        /// Adds a listener that will be notified when configuration is changed
        /// </summary>
        /// <param name="listener">a listener to be added.</param>
        void AddChangeListener(INotifiable listener);

        /// <summary>
        /// Remove a previously added change listener.
        /// </summary>
        /// <param name="listener">a listener to be removed.</param>
        void RemoveChangeListener(INotifiable listener);
    }
}
