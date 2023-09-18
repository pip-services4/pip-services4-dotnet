using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Config.Config;
using System.IO;

namespace PipServices4.Container.Config
{
	/// <summary>
	/// Helper class that reads container configuration from JSON or YAML file.
	/// </summary>
	public sealed class ContainerConfigReader
	{
		/// <summary>
		/// Reads container configuration from JSON or YAML file. The type of the file is determined by file extension.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="path">a path to component configuration file.</param>
		/// <param name="parameters">values to parameters the configuration or null to skip parameterization.</param>
		/// <returns>the read container configuration</returns>
		public static ContainerConfig ReadFromFile(string correlationId, string path, ConfigParams parameters)
		{
			if (path == null)
				throw new ConfigException(correlationId, "NO_PATH", "Missing config file path");

			var ext = Path.GetExtension(path);

			if (ext.Equals(".json"))
				return ReadFromJsonFile(correlationId, path, parameters);

			if (ext.Equals(".yaml") || ext.Equals(".yml"))
				return ReadFromYamlFile(correlationId, path, parameters);

			// By default read as Yaml
			return ReadFromYamlFile(correlationId, path, parameters);
		}

		/// <summary>
		/// Reads container configuration from JSON file.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="path">a path to component configuration file.</param>
		/// <param name="parameters">values to parameters the configuration or null to skip parameterization.</param>
		/// <returns>the read container configuration</returns>
		public static ContainerConfig ReadFromJsonFile(string correlationId, string path, ConfigParams parameters)
		{
			var config = JsonConfigReader.ReadConfig(correlationId, path, parameters);
			return ContainerConfig.FromConfig(config);
		}

		/// <summary>
		/// Reads container configuration from YAML file.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="path">a path to component configuration file.</param>
		/// <param name="parameters">values to parameters the configuration or null to skip parameterization.</param>
		/// <returns>the read container configuration</returns>
		public static ContainerConfig ReadFromYamlFile(string correlationId, string path, ConfigParams parameters)
		{
			var config = YamlConfigReader.ReadConfig(correlationId, path, parameters);
			return ContainerConfig.FromConfig(config);
		}
	}
}
