using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using Xunit;

namespace PipServices4.Components.test.Refer
{
    //[TestClass]
    public class DependencyResolverTest
    {
        [Fact]
        public void TestDependencies()
        {
            var ref1 = new object();
            var ref2 = new object();
            var refs = References.FromTuples(
                "Reference1", ref1,
                new Descriptor("pip-services4-commons", "reference", "object", "ref2", "1.0"), ref2
            );

            var resolver = DependencyResolver.FromTuples(
                "ref1", "Reference1",
                "ref2", new Descriptor("pip-services4-commons", "reference", "*", "*", "*")
            );
            resolver.SetReferences(refs);

            Assert.Equal(ref1, resolver.GetOneRequired("ref1"));
            Assert.Equal(ref2, resolver.GetOneRequired("ref2"));
            Assert.Null(resolver.GetOneOptional("ref3"));
	    }

        [Fact]
        public void TestDependenciesConfig()
        {
            var ref1 = new object();
            var ref2 = new object();
            var refs = References.FromTuples(
                "Reference1", ref1,
                new Descriptor("pip-services4-commons", "reference", "object", "ref2", "1.0"), ref2
            );

            var config = ConfigParams.FromTuples(
                "dependencies.ref1", "Reference1",
                "dependencies.ref2", "pip-services4-commons:reference:*:*:*",
                "dependencies.ref3", null
            );

            var resolver = new DependencyResolver(config);
            resolver.SetReferences(refs);

            Assert.Equal(ref1, resolver.GetOneRequired("ref1"));
            Assert.Equal(ref2, resolver.GetOneRequired("ref2"));
            Assert.Null(resolver.GetOneOptional("ref3"));
	    }
    }
}
