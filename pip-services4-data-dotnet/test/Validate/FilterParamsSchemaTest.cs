using PipServices4.Data.Query;
using PipServices4.Data.Validate;
using Xunit;


namespace PipServices4.Data.test.Validate
{
    //[TestClass]
    public class FilterParamsSchemaTest
    {
        [Fact]
        public void TestEmptyFilterParamsSchema()
        {
            var schema = new FilterParamsSchema();
            var filterParams = new FilterParams();

            var results = schema.Validate(filterParams);
            Assert.Empty(results);
        }

        [Fact]
        public void TestNonEmptyFilterParamsSchema()
        {
            var schema = new FilterParamsSchema();
            var filterParams = new FilterParams()
            {
                {"key", "test"}
            };

            var results = schema.Validate(filterParams);
            Assert.Empty(results);
        }
    }
}
