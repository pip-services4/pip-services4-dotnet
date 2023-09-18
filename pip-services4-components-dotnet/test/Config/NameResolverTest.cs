using PipServices4.Components.Config;
using Xunit;

namespace PipServices4.Components.test.Config
{
    //[TestClass]
    public class NameResolverTest
    {
        [Fact]
        public void TestNormalNameResolution()
        {
            var config = ConfigParams.FromTuples("Id", "ABC");
            var name = NameResolver.Resolve(config);
            Assert.Equal("ABC", name);

            config = ConfigParams.FromTuples("NAME", "ABC");
            name = NameResolver.Resolve(config);
            Assert.Equal("ABC", name);
        }

        [Fact]
        public void TestEmptyName()
        {
            var config = ConfigParams.FromTuples();
            var name = NameResolver.Resolve(config);
            Assert.Null(name);
        }
    }
}
