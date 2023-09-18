using PipServices4.Commons.Errors;
using PipServices4.Commons.Reflect;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;

namespace PipServices4.Container.Config
{
	/// <summary>
	/// Configuration of a component inside a container.
	/// 
	/// The configuration includes type information or descriptor,
	/// and component configuration parameters.
	/// </summary>
	public sealed class ComponentConfig
	{
		/// <summary>
		/// Creates a new instance of the component configuration.
		/// </summary>
		public ComponentConfig() { }

		/// <summary>
		/// Creates a new instance of the component configuration.
		/// </summary>
		/// <param name="descriptor">(optional) a components descriptor (locator).</param>
		/// <param name="type">(optional) a components type descriptor.</param>
		/// <param name="config">(optional) component configuration parameters.</param>
		public ComponentConfig(Descriptor descriptor, TypeDescriptor type, ConfigParams config)
		{
			Descriptor = descriptor;
			Type = type;
			Config = config;
		}

		public Descriptor Descriptor { get; set; }
		public TypeDescriptor Type { get; set; }
		public ConfigParams Config { get; set; }

		/// <summary>
		/// Creates a new instance of ComponentConfig based on section from container configuration.
		/// </summary>
		/// <param name="config">config component parameters from container configuration</param>
		/// <returns>a newly created ComponentConfig</returns>
		public static ComponentConfig FromConfig(ConfigParams config)
		{
			var descriptor = Descriptor.FromString(config.GetAsNullableString("descriptor"));
			var type = TypeDescriptor.FromString(config.GetAsNullableString("type"));

			if (descriptor == null && type == null)
				throw new ConfigException(null, "BAD_CONFIG", "Component configuration must have descriptor or type");

			return new ComponentConfig(descriptor, type, config);
		}
	}
}
