using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Generic
{
    public class GenericNumberStateTest
    {
        [Fact]
        public void TestNextToken()
        {
            var state = new GenericNumberState();

            var scanner = new StringScanner("ABC");
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

            scanner = new StringScanner("123#");
            var token = state.NextToken(scanner, null);
            Assert.Equal("123", token.Value);
            Assert.Equal(TokenType.Integer, token.Type);

            scanner = new StringScanner("-123#");
            token = state.NextToken(scanner, null);
            Assert.Equal("-123", token.Value);
            Assert.Equal(TokenType.Integer, token.Type);
            
            scanner = new StringScanner("123.#");
            token = state.NextToken(scanner, null);
            Assert.Equal("123.", token.Value);
            Assert.Equal(TokenType.Float, token.Type);

            scanner = new StringScanner("123.456#");
            token = state.NextToken(scanner, null);
            Assert.Equal("123.456", token.Value);
            Assert.Equal(TokenType.Float, token.Type);

            scanner = new StringScanner("-123.456#");
            token = state.NextToken(scanner, null);
            Assert.Equal("-123.456", token.Value);
            Assert.Equal(TokenType.Float, token.Type);
        }
    }
}
