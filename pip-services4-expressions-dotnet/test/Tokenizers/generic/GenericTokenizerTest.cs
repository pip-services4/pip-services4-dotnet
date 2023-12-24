using System;
using System.Collections.Generic;
using System.IO;
using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers.Generic
{
    /// <summary>
    /// Contains unit tests for Tokenizer package classes.
    /// </summary>
    public class GenericTokenizerTest
    {
        [Fact]
        public void TestPushbackReader()
        {
            const string TestString = "This is a test string.";
            StringReader stringReader = new StringReader(TestString);
            TextPushbackReader reader = new TextPushbackReader(stringReader);

            Assert.Equal('T', reader.Read());
            Assert.Equal('h', reader.Read());
            Assert.Equal('i', reader.Read());
            Assert.Equal('s', reader.Read());

            Assert.Equal(' ', reader.Peek());
            Assert.Equal(' ', reader.Peek());
            Assert.Equal(' ', reader.Read());
            Assert.Equal('i', reader.Peek());

            reader.Pushback('#');
            Assert.Equal('#', reader.Peek());
            Assert.Equal('#', reader.Read());

            reader.Pushback('$');
            reader.Pushback('%');
            Assert.Equal('%', reader.Read());
            Assert.Equal('$', reader.Read());

            reader.Pushback('$');
            reader.PushbackString("%@");
            Assert.Equal('%', reader.Read());
            Assert.Equal('@', reader.Read());
            Assert.Equal('$', reader.Read());

            reader.PushbackString("ABC");
            Assert.Equal('A', reader.Peek());
            Assert.Equal('A', reader.Read());
            Assert.Equal('B', reader.Read());
            Assert.Equal('C', reader.Read());
            Assert.Equal('i', reader.Read());
        }

        [Fact]
        public void TestExpression()
        {
            string tokenString = "A+B/123 - \t 'xyz'\n <>-10.11# This is a comment";
            Token[] expectedTokens = new Token[] {
                new Token(TokenType.Word, "A", 0, 0), new Token(TokenType.Symbol, "+", 0, 0),
                new Token(TokenType.Word, "B", 0, 0), new Token(TokenType.Symbol, "/", 0, 0),
                new Token(TokenType.Integer, "123", 0, 0), new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Symbol, "-", 0, 0), new Token(TokenType.Whitespace, " \t ", 0, 0),
                new Token(TokenType.Quoted, "'xyz'", 0, 0), new Token(TokenType.Whitespace, "\n ", 0, 0),
                new Token(TokenType.Symbol, "<>", 0, 0), new Token(TokenType.Float, "-10.11", 0, 0),
                new Token(TokenType.Comment, "# This is a comment", 0, 0), new Token(TokenType.Eof, null, 0, 0)};

            ITokenizer tokenizer = new GenericTokenizer();
            IList<Token> tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestQuoteToken()
        {
            string tokenString = "A'xyz'\"abc\ndeg\" 'jkl\"def'";
            Token[] expectedTokens = new Token[] {
                new Token(TokenType.Word, "A", 0, 0), new Token(TokenType.Quoted, "xyz", 0, 0),
                new Token(TokenType.Quoted, "abc\ndeg", 0, 0), new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Quoted, "jkl\"def", 0, 0)};

            ITokenizer tokenizer = new GenericTokenizer();
            tokenizer.SkipEof = true;
            tokenizer.DecodeStrings = true;
            IList<Token> tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestWordToken()
        {
            string tokenString = "A'xyz'Ebf_2\n2x_2";
            Token[] expectedTokens = new Token[] {
                new Token(TokenType.Word, "A", 0, 0), new Token(TokenType.Quoted, "xyz", 0, 0),
                new Token(TokenType.Word, "Ebf_2", 0, 0), new Token(TokenType.Whitespace, "\n", 0, 0),
                new Token(TokenType.Integer, "2", 0, 0), new Token(TokenType.Word, "x_2", 0, 0)};

            ITokenizer tokenizer = new GenericTokenizer();
            tokenizer.SkipEof = true;
            tokenizer.DecodeStrings = true;
            IList<Token> tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestNumberToken()
        {
            string tokenString = "123-321 .543-.76-. -123.456";
            Token[] expectedTokens = new Token[] {
                new Token(TokenType.Integer, "123", 0, 0), new Token(TokenType.Integer, "-321", 0, 0),
                new Token(TokenType.Whitespace, " ", 0, 0), new Token(TokenType.Float, ".543", 0, 0),
                new Token(TokenType.Float, "-.76", 0, 0), new Token(TokenType.Symbol, "-", 0, 0),
                new Token(TokenType.Symbol, ".", 0, 0), new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Float, "-123.456", 0, 0)};

            ITokenizer tokenizer = new GenericTokenizer();
            tokenizer.SkipEof = true;
            tokenizer.DecodeStrings = true;
            IList<Token> tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestWrongToken()
        {
            var tokenString = "1>2";
            var expectedTokens = new Token[] {
                new Token(TokenType.Integer, "1", 0, 0),
                new Token(TokenType.Symbol, ">", 0, 0),
                new Token(TokenType.Integer, "2", 0, 0),

            };

            var tokenizer = new GenericTokenizer();
            tokenizer.SkipEof = true;
            var tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }
    }
}
