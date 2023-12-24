using PipServices4.Expressions.Tokenizers.Utilities;
using System;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Utilities
{
    public class CharReferenceMapTest
    {
        [Fact]
        public void TestDefaultInterval()
        {
            CharReferenceMap<bool> map = new CharReferenceMap<bool>();
            Assert.False(map.Lookup('A'));
            Assert.False(map.Lookup('\x2045'));

            map.AddDefaultInterval(true);
            Assert.True(map.Lookup('A'));
            Assert.True(map.Lookup('\x2045'));

            map.Clear();
            Assert.False(map.Lookup('A'));
            Assert.False(map.Lookup('\x2045'));
        }

        [Fact]
        public void TestInterval()
        {
            CharReferenceMap<bool> map = new CharReferenceMap<bool>();
            Assert.False(map.Lookup('A'));
            Assert.False(map.Lookup('\x2045'));

            map.AddInterval('A', 'z', true);
            Assert.True(map.Lookup('A'));
            Assert.False(map.Lookup('\x2045'));

            map.AddInterval('\x2000', '\x20ff', true);
            Assert.True(map.Lookup('A'));
            Assert.True(map.Lookup('\x2045'));

            map.Clear();
            Assert.False(map.Lookup('A'));
            Assert.False(map.Lookup('\x2045'));

            map.AddInterval('A', '\x20ff', true);
            Assert.True(map.Lookup('A'));
            Assert.True(map.Lookup('\x2045'));
        }
    }
}
