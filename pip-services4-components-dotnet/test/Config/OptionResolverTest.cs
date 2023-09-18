using PipServices4.Components.Config;
using Xunit;

namespace PipServices4.Components.test.Config
{
    public class OptionResolverTest
    {
        [Fact]
        public void TestNormalOptionsResolution()
        {
            var config = ConfigParams.FromTuples("options.max_size", 1024);
            var options = OptionsResolver.Resolve(config);
            Assert.Equal(1024, options.GetAsInteger("max_size"));
        }

        [Fact]
        public void TestConfigsWithoutOptions()
        {
            var config = ConfigParams.FromTuples("name", "ABC");
            var options = OptionsResolver.Resolve(config);
            Assert.Equal(config, options);
        }
        
        [Fact]
        public void TestConfigsWithoutOptionsAndConfigAsDefault()
        {
            var config = ConfigParams.FromTuples("name", "ABC");
            var options = OptionsResolver.Resolve(config, false);
            Assert.Null(options);
        }
    }
}