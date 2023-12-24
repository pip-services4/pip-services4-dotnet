using System;
using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Generic
{
    public class CCommentStateTest
    {
        [Fact]
        public void TestNextToken()
        {
            var state = new CCommentState();

            var scanner = new StringScanner("// Comment \n Comment ");
            var failed = false;
            try
            {
                state.NextToken(scanner, null);                
            }
            catch
            {
                failed = true;
            }
            Assert.True(failed);

            scanner = new StringScanner("/* Comment \n Comment */#");
            var token = state.NextToken(scanner, null);
            Assert.Equal("/* Comment \n Comment */", token.Value);
            Assert.Equal(TokenType.Comment, token.Type);
        }
    }
}
