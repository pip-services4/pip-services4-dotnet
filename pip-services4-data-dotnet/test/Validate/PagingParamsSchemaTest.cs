using PipServices4.Data.Query;
using PipServices4.Data.Validate;
using Xunit;


namespace PipServices4.Data.test.Validate
{
    //[TestClass]
    public class PagingParamsSchemaTest
    {
        [Fact]
        public void TestEmptyPagingParams()
        {
            var schema = new PagingParamsSchema();
            var pagingParams = new PagingParams();

            var results = schema.Validate(pagingParams);
            Assert.Empty(results);
        }

        [Fact]
        public void TestNonEmptyPagingParams()
        {
            var schema = new PagingParamsSchema();
            var pagingParams = new PagingParams(1, 1, true);

            var results = schema.Validate(pagingParams);
            Assert.Empty(results);
        }

        [Fact]
        public void TestOptionalPagingParams()
        {
            var schema = new PagingParamsSchema();
            var pagingParams = new PagingParams()
            {
                Skip = null,
                Take = null
            };

            var results = schema.Validate(pagingParams);
            Assert.Empty(results);
        }
    }
}
