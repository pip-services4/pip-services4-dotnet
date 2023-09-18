using PipServices4.Commons.Convert;
using Xunit;

namespace PipServices4.Commons.Test.Convert
{
    //[TestClass]
    public class IntegerConverterTest
    {
        [Fact]
        public void TestToInteger()
        {
            Assert.Equal(123, IntegerConverter.ToInteger(123));
            Assert.Equal(123, IntegerConverter.ToInteger(123.456));
            Assert.Equal(124, IntegerConverter.ToInteger(123.999));
            Assert.Equal(123, IntegerConverter.ToInteger("123"));
            Assert.Equal(123, IntegerConverter.ToInteger("123.465"));
            Assert.Equal(123, IntegerConverter.ToInteger("123.999"));

            Assert.Equal(123, IntegerConverter.ToIntegerWithDefault(null, 123));
            Assert.Equal(0, IntegerConverter.ToIntegerWithDefault(false, 123));
            Assert.Equal(123, IntegerConverter.ToIntegerWithDefault("ABC", 123));

            long longValue = int.MaxValue;
            Assert.Equal(int.MaxValue, IntegerConverter.ToInteger(longValue));
            longValue = long.MaxValue;
            Assert.Equal(0, IntegerConverter.ToInteger(longValue));
        }
    }
}
