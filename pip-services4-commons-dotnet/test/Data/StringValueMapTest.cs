using PipServices4.Commons.Data;
using System;
using Xunit;

namespace PipServices4.Commons.Test.Data
{
	//[TestClass]
	public class StringValueMapTest
	{
        [Fact]
        public void TestGetAsString()
        {
            var map = StringValueMap.FromString("key1=1;key2=123.456;key3=2018-01-01;key4=null");

			Assert.True(map.GetAsBoolean("key1"));
			Assert.Equal(123, map.GetAsInteger("key2"));
			Assert.Equal(123.456, map.GetAsFloat("key2"), 3);
			Assert.Equal(new DateTime(2018, 1, 1).ToUniversalTime(), map.GetAsDateTime("key3").ToUniversalTime());
			Assert.Equal("null", map.GetAsNullableString("key4"));
		}
    }
}
