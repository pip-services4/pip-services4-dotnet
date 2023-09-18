using PipServices4.Data.Random;
using Xunit;

namespace PipServices4.Data.test.Random
{
    //[TestClass]
    public class RandomBooleanTest
    {
        [Fact]
        public void TestChance()
        {
            bool value;
            value = RandomBoolean.Chance(5, 10);
            Assert.True(value || !value);

            value = RandomBoolean.Chance(5, 5);
            Assert.True(value || !value);

            value = RandomBoolean.Chance(0, 0);
            Assert.True(!value);

            value = RandomBoolean.Chance(-1, 0);
            Assert.True(!value);

            value = RandomBoolean.Chance(-1, -1);
            Assert.True(!value);
        }
    }
}
