using PipServices4.Data.Validate;
using Xunit;

namespace PipServices4.Data.test.Validate
{
    //[TestClass]
    public class OnlyOneExistRuleTest
    {
        [Fact]
        public void TestOnlyOneExistRule()
        {
            var obj = new TestObject();
            var schema = new Schema().WithRule(new OnlyOneExistsRule("MissingProperty", "StringProperty", "NullProperty"));
            var results = schema.Validate(obj);
            Assert.Empty(results);

            schema = new Schema().WithRule(new OnlyOneExistsRule("StringProperty", "NullProperty", "intField"));
            results = schema.Validate(obj);
            Assert.Single(results);
        }
    }
}
