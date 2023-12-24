using System;
using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Generic
{
    public class GenericSymbolStateTest
    {
        [Fact]
        public void TestNextToken()
        {
            var state = new GenericSymbolState();
            state.Add("<", TokenType.Symbol);
            state.Add("<<", TokenType.Symbol);
            state.Add("<>", TokenType.Symbol);

            var scanner = new StringScanner("<A<<<>");

            var token = state.NextToken(scanner, null);
            Assert.Equal("<", token.Value);
            Assert.Equal(TokenType.Symbol, token.Type);

            token = state.NextToken(scanner, null);
            Assert.Equal("A", token.Value);
            Assert.Equal(TokenType.Symbol, token.Type);

            token = state.NextToken(scanner, null);
            Assert.Equal("<<", token.Value);
            Assert.Equal(TokenType.Symbol, token.Type);

            token = state.NextToken(scanner, null);
            Assert.Equal("<>", token.Value);
            Assert.Equal(TokenType.Symbol, token.Type);
        }
    }
}
