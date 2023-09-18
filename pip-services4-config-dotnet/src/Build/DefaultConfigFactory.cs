using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Config.Auth;
using PipServices4.Config.Connect;

namespace PipServices4.Config.Config
{
	public class DefaultConfigFactory : Factory
	{
		public static readonly Descriptor MemoryCredentialStoreDescriptor = new Descriptor("pip-services", "credential-store", "memory", "*", "1.0");
		public static readonly Descriptor MemoryConfigReaderDescriptor = new Descriptor("pip-services", "config-reader", "memory", "*", "1.0");
		public static readonly Descriptor JsonConfigReaderDescriptor = new Descriptor("pip-services", "config-reader", "json", "*", "1.0");
		public static readonly Descriptor YamlConfigReaderDescriptor = new Descriptor("pip-services", "config-reader", "yaml", "*", "1.0");
		public static readonly Descriptor MemoryDiscoveryDescriptor = new Descriptor("pip-services", "discovery", "memory", "*", "1.0");

		/// <summary>
		/// Create a new instance of the factory.
		/// </summary>
		public DefaultConfigFactory()
		{
			RegisterAsType(MemoryCredentialStoreDescriptor, typeof(MemoryCredentialStore));
			RegisterAsType(MemoryConfigReaderDescriptor, typeof(MemoryConfigReader));
			RegisterAsType(JsonConfigReaderDescriptor, typeof(JsonConfigReader));
			RegisterAsType(YamlConfigReaderDescriptor, typeof(YamlConfigReader));
			RegisterAsType(MemoryDiscoveryDescriptor, typeof(MemoryDiscovery));
		}
	}
}
