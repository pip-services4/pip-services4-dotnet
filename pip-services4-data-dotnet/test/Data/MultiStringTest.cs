using System;
using PipServices4.Commons.Data;
using PipServices4.Data.Data;
using Xunit;

namespace PipServices4.Data.test.Data
{
    public class MultiStringTest
    {
        [Fact]
        public void TestGetAndSet()
        {
            var value = new MultiString();
            Assert.Null(value.Get("en"));

            value.Put("ru", "Russian");
            Assert.Equal("Russian", value.Get("ru"));
            Assert.Equal("Russian", value.Get("en"));
            Assert.Equal("Russian", value.Get("pt"));

            value.Put("en", "English");
            Assert.Equal("Russian", value.Get("ru"));
            Assert.Equal("English", value.Get("en"));
            Assert.Equal("English", value.Get("pt"));

        }
    }
}
