using PipServices4.Expressions.Tokenizers.Utilities;
using System;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Utilities
{
    public class CharValidatorTest
    {
        [Fact]
        public void TestIsEof()
        {
            Assert.True(CharValidator.IsEof('\xffff'));
            Assert.False(CharValidator.IsEof('A'));
        }

        [Fact]
        public void TestIsEol()
        {
            Assert.True(CharValidator.IsEol('\r'));
            Assert.True(CharValidator.IsEol('\n'));
            Assert.False(CharValidator.IsEof('A'));
        }

        [Fact]
        public void TestIsDigit()
        {
            Assert.True(CharValidator.IsDigit('0'));
            Assert.True(CharValidator.IsDigit('7'));
            Assert.True(CharValidator.IsDigit('9'));
            Assert.False(CharValidator.IsDigit('A'));
        }

    }
}
