using PipServices4.Components.Config;
using System.Collections.Generic;

namespace PipServices4.Container.Config
{
	/// <summary>
	/// Container configuration defined as a list of component configurations.
	/// </summary>
	/// See <see cref="ComponentConfig"/>
	public sealed class ContainerConfig : List<ComponentConfig>
	{
		/// <summary>
		/// Creates a new instance of container configuration.
		/// </summary>
		public ContainerConfig() { }

		/// <summary>
		/// Creates a new instance of container configuration.
		/// </summary>
		/// <param name="components">(optional) a list of component configurations.</param>
		public ContainerConfig(IEnumerable<ComponentConfig> components)
		{
			if (components != null)
				AddRange(components);
		}

		/// <summary>
		/// Creates a new ContainerConfig object filled with key-value pairs from
		/// specified object. The value is converted into ConfigParams object which is
		/// used to create the object.
		/// </summary>
		/// <param name="value">an object with key-value pairs used to initialize a new ContainerConfig.</param>
		/// <returns>a new ContainerConfig object.</returns>
		public static ContainerConfig FromObject(object value)
		{
			var config = ConfigParams.FromValue(value);
			return FromConfig(config);
		}

		/// <summary>
		/// Creates a new ContainerConfig object based on configuration parameters. Each 
		/// section in the configuration parameters is converted into a component configuration.
		/// </summary>
		/// <param name="config">an object with key-value pairs used to initialize a new ContainerConfig.</param>
		/// <returns>a new ContainerConfig object.</returns>
		public static ContainerConfig FromConfig(ConfigParams config)
		{
			var result = new ContainerConfig();
			if (config == null) return result;

			foreach (var section in config.GetSectionNames())
			{
				var componentConfig = config.GetSection(section);
				result.Add(ComponentConfig.FromConfig(componentConfig));
			}

			return result;
		}
	}
}
