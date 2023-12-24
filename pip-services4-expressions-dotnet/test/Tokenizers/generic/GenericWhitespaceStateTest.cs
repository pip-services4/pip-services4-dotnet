using System;
using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Generic
{
    public class GenericWhitespaceStateTest
    {
        [Fact]
        public void TestNextToken()
        {
            var state = new GenericWhitespaceState();

            var scanner = new StringScanner(" \t\n\r ");
            var token = state.NextToken(scanner, null);
            Assert.Equal(" \t\n\r ", token.Value);
            Assert.Equal(TokenType.Whitespace, token.Type);
        }
    }
}
