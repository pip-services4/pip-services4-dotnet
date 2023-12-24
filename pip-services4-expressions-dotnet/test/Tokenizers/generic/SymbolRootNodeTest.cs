using System;
using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Generic
{
    public class SymbolRootNodeTest
    {
        [Fact]
        public void TestNextToken()
        {
            var node = new SymbolRootNode();
            node.Add("<", TokenType.Symbol);
            node.Add("<<", TokenType.Symbol);
            node.Add("<>", TokenType.Symbol);

            var scanner = new StringScanner("<A<<<>");

            var token = node.NextToken(scanner);
            Assert.Equal("<", token.Value);

            token = node.NextToken(scanner);
            Assert.Equal("A", token.Value);

            token = node.NextToken(scanner);
            Assert.Equal("<<", token.Value);

            token = node.NextToken(scanner);
            Assert.Equal("<>", token.Value);
        }

        [Fact]
        public void TestSingleToken()
        {
            var node = new SymbolRootNode();

            var scanner = new StringScanner("<A");

            var token = node.NextToken(scanner);
            Assert.Equal("<", token.Value);
            Assert.Equal(TokenType.Symbol, token.Type);
        }
    }
}
