using System;
using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Generic
{
    public class GenericCommentStateTest
    {
        [Fact]
        public void TestNextToken()
        {
            var state = new GenericCommentState();

            var scanner = new StringScanner("# Comment \r# Comment ");
            var token = state.NextToken(scanner, null);
            Assert.Equal("# Comment ", token.Value);
            Assert.Equal(TokenType.Comment, token.Type);

            scanner = new StringScanner("# Comment \n# Comment ");
            token = state.NextToken(scanner, null);
            Assert.Equal("# Comment ", token.Value);
            Assert.Equal(TokenType.Comment, token.Type);
        }
    }
}
