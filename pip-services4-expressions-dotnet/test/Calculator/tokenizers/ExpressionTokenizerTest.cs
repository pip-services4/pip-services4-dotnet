using System;
using System.Collections.Generic;
using PipServices4.Expressions.Calculator.Tokenizers;
using PipServices4.Expressions.Test.Tokenizers;
using PipServices4.Expressions.Tokenizers;
using Xunit;

namespace PipServices4.Expressions.Test.Calculator.Tokenizers
{
    /// <summary>
    /// Contains unit tests for Tokenizer package classes.
    /// </summary>
    public class ExpressionTokenizerTest
    {

        [Fact]
        public void TestQuoteToken()
        {
            string tokenString = "A'xyz'\"abc\ndeg\" 'jkl\"def'\"ab\"\"de\"'df''er'";
            Token[] expectedTokens = new Token[] {
                new Token(TokenType.Word, "A", 0, 0), new Token(TokenType.Quoted, "xyz", 0, 0),
                new Token(TokenType.Word, "abc\ndeg", 0, 0), new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Quoted, "jkl\"def", 0, 0), new Token(TokenType.Word, "ab\"de", 0, 0),
                new Token(TokenType.Quoted, "df'er", 0, 0)
            };

            ITokenizer tokenizer = new ExpressionTokenizer();
            tokenizer.SkipEof = true;
            tokenizer.DecodeStrings = true;
            IList<Token> tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestWordToken()
        {
            string tokenString = "A'xyz'Ebf_2\n2_2";
            Token[] expectedTokens = new Token[] {
                new Token(TokenType.Word, "A", 0, 0), new Token(TokenType.Quoted, "xyz", 0, 0),
                new Token(TokenType.Word, "Ebf_2", 0, 0), new Token(TokenType.Whitespace, "\n", 0, 0),
                new Token(TokenType.Integer, "2", 0, 0), new Token(TokenType.Word, "_2", 0, 0)};

            ITokenizer tokenizer = new ExpressionTokenizer();
            tokenizer.SkipEof = true;
            tokenizer.DecodeStrings = true;
            IList<Token> tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestNumberToken()
        {
            string tokenString = "123-321 .543-.76-. 123.456 123e45 543.11E+43 1e 3E-";
            Token[] expectedTokens = new Token[] {
                new Token(TokenType.Integer, "123", 0, 0), new Token(TokenType.Symbol, "-", 0, 0),
                new Token(TokenType.Integer, "321", 0, 0), new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Float, ".543", 0, 0), new Token(TokenType.Symbol, "-", 0, 0),
                new Token(TokenType.Float, ".76", 0, 0), new Token(TokenType.Symbol, "-", 0, 0),
                new Token(TokenType.Symbol, ".", 0, 0), new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Float, "123.456", 0, 0), new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Float, "123e45", 0, 0), new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Float, "543.11E+43", 0, 0), new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Integer, "1", 0, 0), new Token(TokenType.Word, "e", 0, 0),
                new Token(TokenType.Whitespace, " ", 0, 0), new Token(TokenType.Integer, "3", 0, 0),
                new Token(TokenType.Word, "E", 0, 0), new Token(TokenType.Symbol, "-", 0, 0)
            };

            ITokenizer tokenizer = new ExpressionTokenizer();
            tokenizer.SkipEof = true;
            tokenizer.DecodeStrings = true;
            IList<Token> tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestExpressionToken()
        {
            var tokenString = "A + b / (3 - Max(-123, 1)*2)";

            var tokenizer = new ExpressionTokenizer();
            var tokenList = tokenizer.TokenizeBuffer(tokenString);

            Assert.Equal(25, tokenList.Count);
        }


    }
}
