using PipServices4.Data.Random;
using Xunit;

namespace PipServices4.Data.test.Random
{
    //[TestClass]
    public class RandomFloatTest
    {
        [Fact]
        public void TestNextFloat()
        {
            float value = RandomFloat.NextFloat(5);
            Assert.True(value < 5);

            value = RandomFloat.NextFloat(2, 5);
            Assert.True(value < 5 && value > 2);
        }

        [Fact]
        public void TestUpdateFloat()
        {
            float value = RandomFloat.UpdateFloat(0, 5);

            Assert.True(value <= 5 && value >= -5);
        }
    }
}
