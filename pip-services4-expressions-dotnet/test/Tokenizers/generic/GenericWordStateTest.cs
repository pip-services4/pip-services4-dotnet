using System;
using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Generic
{
    public class GenericWordStateTest
    {
        [Fact]
        public void TestNextToken()
        {
            var state = new GenericWordState();

            var scanner = new StringScanner("AB_CD=");
            var token = state.NextToken(scanner, null);
            Assert.Equal("AB_CD", token.Value);
            Assert.Equal(TokenType.Word, token.Type);
        }
    }
}
