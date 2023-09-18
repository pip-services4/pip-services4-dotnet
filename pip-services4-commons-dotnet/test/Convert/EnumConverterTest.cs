using PipServices4.Commons.Convert;
using Xunit;

namespace PipServices4.Commons.Test.Convert
{
    //[TestClass]
    public class EnumConverterTest
    {
        [Fact]
        public void TestToEnum()
        {
            Assert.Equal(LogLevel.None, EnumConverter.ToEnum<LogLevel>("ABC"));
            Assert.Equal(LogLevel.Fatal, EnumConverter.ToEnum<LogLevel>(1));
            Assert.Equal(LogLevel.Fatal, EnumConverter.ToEnum<LogLevel>(LogLevel.Fatal));
            Assert.Equal(LogLevel.Fatal, EnumConverter.ToEnum<LogLevel>("Fatal"));
        }
    }
}
