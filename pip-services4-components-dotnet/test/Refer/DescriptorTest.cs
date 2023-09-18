using PipServices4.Commons.Errors;
using PipServices4.Components.Refer;

using Xunit;

namespace PipServices4.Components.test.Refer
{
    //[TestClass]
    public class DescriptorTest
    {
        [Fact]
        public void TestMatch()
        {
            Descriptor descriptor = new Descriptor("pip-dummies", "controller", "default", "default", "1.0");

            // Check match by individual fields
            Assert.True(descriptor.Match(new Descriptor(null, null, null, null, null)));
            Assert.True(descriptor.Match(new Descriptor("pip-dummies", "controller", null, null, null)));
            Assert.True(descriptor.Match(new Descriptor(null, null, "default", null, null)));
            Assert.True(descriptor.Match(new Descriptor(null, null, null, null, "1.0")));

            // Check match by individual "*" fields
            Assert.True(descriptor.Match(new Descriptor("pip-dummies", "*", "*", "*", "*")));
            Assert.True(descriptor.Match(new Descriptor("*", "controller", "*", "*", "*")));
            Assert.True(descriptor.Match(new Descriptor("*", "*", "default", "*", "*")));
            Assert.True(descriptor.Match(new Descriptor("*", "*", "*", "*", "1.0")));

            // Check match by all values
            Assert.True(descriptor.Match(new Descriptor("pip-dummies", "controller", "default", "default", null)));
            Assert.True(descriptor.Match(new Descriptor(null, "controller", "default", "default", "1.0")));
            Assert.True(descriptor.Match(new Descriptor("pip-dummies", "controller", "default", "default", "1.0")));

            // Check mismatch by individual fields
            Assert.False(descriptor.Match(new Descriptor(null, "cache", null, null, null)));
            Assert.False(descriptor.Match(new Descriptor("pip-services4-commons", "controller", null, null, null)));
            Assert.False(descriptor.Match(new Descriptor(null, null, "special", null, null)));
            Assert.False(descriptor.Match(new Descriptor(null, null, null, null, "2.0")));
        }

        [Fact]
        public void TestToString()
        {
            Descriptor descriptor1 = new Descriptor("pip-dummies", "controller", "default", "default", "1.0");
            Assert.Equal("pip-dummies:controller:default:default:1.0", descriptor1.ToString());

            Descriptor descriptor2 = new Descriptor(null, "controller", null, null, null);
            Assert.Equal("*:controller:*:*:*", descriptor2.ToString());
        }

        [Fact]
	    public void TestFromString() {
		    Descriptor descriptor = Descriptor.FromString(null);
		    Assert.Null(descriptor);
		
		    descriptor = Descriptor.FromString("pip-dummies:controller:default:default:1.0");
		    Assert.True(descriptor.ExactMatch(new Descriptor("pip-dummies", "controller", "default", "default", "1.0")));
		
		    try {
			    Descriptor.FromString("xxx");
                Assert.False(true, "Descriptor.fromString shall throw an exception");
		    } catch (ConfigException) {
			    // Ok...
		    }
	    }
    }
}
