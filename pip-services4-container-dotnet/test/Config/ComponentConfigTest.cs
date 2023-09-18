using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Container.Config;
using Xunit;
using TypeDescriptor = PipServices4.Commons.Reflect.TypeDescriptor;

namespace PipServices4.Container.test.Config
{
	public sealed class ComponentConfigTest
	{
		[Fact]
		public void TestType()
		{
			var componentConfig = new ComponentConfig();
			Assert.Null(componentConfig.Type);

			var type = new TypeDescriptor("new name", null);
			componentConfig.Type = type;
			Assert.Equal(componentConfig.Type, type);
		}

		[Fact]
		public void TestDescriptor()
		{
			var componentConfig = new ComponentConfig();
			Assert.Null(componentConfig.Descriptor);

			var descriptor = new Descriptor("group", "type", "id", "default", "version");
			componentConfig.Descriptor = descriptor;
			Assert.Equal(componentConfig.Descriptor, descriptor);
		}

		[Fact]
		public void TestConfigParams()
		{
			var componentConfig = new ComponentConfig();
			Assert.Null(componentConfig.Config);

			var config = ConfigParams.FromTuples(
				"config.key", "key",
				"config.key2", "key2"
				);
			componentConfig.Config = config;
			Assert.Equal(componentConfig.Config, config);
		}

		[Fact]
		public void TestFromConfig()
		{
			var config = ConfigParams.FromTuples();
			ComponentConfig componentConfig;
			try
			{
				componentConfig = ComponentConfig.FromConfig(config);
			}
			catch (ConfigException e)
			{
				Assert.Equal("Component configuration must have descriptor or type", e.Message);
			}

			config = ConfigParams.FromTuples(
				"descriptor", "descriptor_name",
				"type", "type",
				"config.key", "key",
				"config.key2", "key2"
				);

			try
			{
				componentConfig = ComponentConfig.FromConfig(config);
			}
			catch (ConfigException e)
			{
				Assert.Equal("Descriptor descriptor_name is in wrong format", e.Message);
			}

			var descriptor = new Descriptor("group", "type", "id", "name", "version");
			var type = new TypeDescriptor("type", null);
			config = ConfigParams.FromTuples(
				"descriptor", "group:type:id:name:version",
				"type", "type",
				"config.key", "key",
				"config.key2", "key2"
				);
			componentConfig = ComponentConfig.FromConfig(config);

			Assert.Equal(componentConfig.Descriptor, descriptor);
			Assert.Equal(componentConfig.Type, type);
		}
	}
}
