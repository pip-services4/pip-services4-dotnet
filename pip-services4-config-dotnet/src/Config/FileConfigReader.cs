using PipServices4.Components.Config;

namespace PipServices4.Config.Config
{
	/// <summary>
	/// Abstract config reader that reads configuration from a file.
	/// Child classes add support for config files in their specific format 
	/// like JSON, YAML or property files.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - path:          path to configuration file
	/// - parameters:            this entire section is used as template parameters
	/// - ...
	/// </summary>
	/// See <see cref="IConfigReader"/>, <see cref="ConfigReader"/>
	public abstract class FileConfigReader : ConfigReader
    {
        /// <summary>
        /// Gets or sets the path to configuration file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Creates a new instance of the config reader.
        /// </summary>
        /// <param name="path">(optional) a path to configuration file.</param>
        public FileConfigReader(string path = null)
        {
            Path = path;
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public override void Configure(ConfigParams config)
        {
            base.Configure(config);

            Path = config.GetAsString("path");
        }
    }
}
