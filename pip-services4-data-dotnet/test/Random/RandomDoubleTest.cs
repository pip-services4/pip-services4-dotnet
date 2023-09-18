using PipServices4.Commons.Convert;
using PipServices4.Data.Random;
using Xunit;

namespace PipServices4.Data.test.Random
{
    //[TestClass]
    public class RandomDoubleTest
    {
        [Fact]
        public void TestNextDouble()
        {
            double value = RandomDouble.NextDouble(5);
            Assert.True(value < 5);
            Assert.True(value == DoubleConverter.ToDouble(value));

            value = RandomDouble.NextDouble(2, 5);
            Assert.True(value < 5 && value > 2);
            Assert.True(value == DoubleConverter.ToDouble(value));
        }

        [Fact]
        public void TestUpdateDouble()
        {
            double value = RandomDouble.UpdateDouble(0, 5);

            Assert.True(value <= 5 && value >= -5);
            Assert.True(value == DoubleConverter.ToDouble(value));
        }
    }
}
