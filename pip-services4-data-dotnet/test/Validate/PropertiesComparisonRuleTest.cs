using PipServices4.Data.Validate;
using Xunit;

namespace PipServices4.Data.test.Validate
{
    //[TestClass]
    public class PropertiesComparisonRuleTest
    {
        [Fact]
        public void TestPropertiesComparison()
        {
            var obj = new TestObject();
            var schema = new Schema().WithRule(new PropertiesComparisonRule("StringProperty", "EQ", "NullProperty"));

            obj.StringProperty = "ABC";
            obj.NullProperty = "ABC";
            var results = schema.Validate(obj);
            Assert.Empty(results);

            obj.NullProperty = "XYZ";
            results = schema.Validate(obj);
            Assert.Single(results);
        }
    }
}
