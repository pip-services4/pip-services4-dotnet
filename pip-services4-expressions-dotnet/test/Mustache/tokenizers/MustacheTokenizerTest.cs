using System;
using System.Collections.Generic;
using System.Text;
using PipServices4.Expressions.Mustache.Tokenizers;
using PipServices4.Expressions.Test.Tokenizers;
using PipServices4.Expressions.Tokenizers;
using Xunit;

namespace PipServices4.Expressions.Test.Mustache.Tokenizers
{
    public class MustacheTokenizerTest
    {
        [Fact]
        public void TestTemplate1()
        {
            string tokenString = "Hello, {{ Name }}!";
            Token[] expectedTokens =
            {
                new Token(TokenType.Special, "Hello, ", 0, 0),
                new Token(TokenType.Symbol, "{{", 0, 0),
                new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Word, "Name", 0, 0),
                new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Symbol, "}}", 0, 0),
                new Token(TokenType.Special, "!", 0, 0),
            };

            var tokenizer = new MustacheTokenizer();
            tokenizer.SkipEof = true;
            var tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestTemplate2()
        {
            string tokenString = "Hello, {{{ Name }}}!";
            Token[] expectedTokens =
            {
                new Token(TokenType.Special, "Hello, ", 0, 0),
                new Token(TokenType.Symbol, "{{{", 0, 0),
                new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Word, "Name", 0, 0),
                new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Symbol, "}}}", 0, 0),
                new Token(TokenType.Special, "!", 0, 0),
            };

            var tokenizer = new MustacheTokenizer();
            tokenizer.SkipEof = true;
            var tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestTemplate3()
        {
            string tokenString = "{{ Name }}}";
            Token[] expectedTokens =
            {
                new Token(TokenType.Symbol, "{{", 0, 0),
                new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Word, "Name", 0, 0),
                new Token(TokenType.Whitespace, " ", 0, 0),
                new Token(TokenType.Symbol, "}}}", 0, 0)
            };

            var tokenizer = new MustacheTokenizer();
            tokenizer.SkipEof = true;
            var tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }

        [Fact]
        public void TestTemplate4()
        {
            string tokenString = "Hello, World!";
            Token[] expectedTokens =
            {
                 new Token(TokenType.Special, "Hello, World!", 0, 0)
            };

            var tokenizer = new MustacheTokenizer();
            tokenizer.SkipEof = true;
            var tokenList = tokenizer.TokenizeBuffer(tokenString);

            TokenizerFixture.AssertAreEqualsTokenLists(expectedTokens, tokenList);
        }
    }
}
