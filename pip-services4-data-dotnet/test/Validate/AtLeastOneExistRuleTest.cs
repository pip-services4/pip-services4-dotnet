using Xunit;
using PipServices4.Data.Validate;

namespace PipServices4.Data.test.Validate
{
    //[TestClass]
    public class AtLeastOneExistRuleTest
    {
        [Fact]
        public void TestOnlyOneExistRule()
        {
            var obj = new TestObject();
            var schema = new Schema().WithRule(new AtLeastOneExistsRule("MissingProperty", "StringProperty", "NullProperty"));
            var results = schema.Validate(obj);
            Assert.Empty(results);

            schema = new Schema().WithRule(new AtLeastOneExistsRule("StringProperty", "NullProperty", "intField"));
            results = schema.Validate(obj);
            Assert.Empty(results);

            schema = new Schema().WithRule(new AtLeastOneExistsRule("MissingProperty", "NullProperty"));
            results = schema.Validate(obj);
            Assert.Single(results);
        }
    }
}
