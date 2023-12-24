using PipServices4.Expressions.Mustache.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PipServices4.Expressions.Test.Mustache.Parsers
{
    public class MustacheParserTest
    {
        [Fact]
        public void TestLexicalAnalysis()
        {
            MustacheParser parser = new MustacheParser();
            parser.Template = "Hello, {{{NAME}}}{{ #if ESCLAMATION }}!{{/if}}{{{^ESCLAMATION}}}.{{{/ESCLAMATION}}}";

            List<MustacheToken> expectedTokens = new List<MustacheToken>
            {
                new MustacheToken(MustacheTokenType.Value, "Hello, ", 0, 0),
                new MustacheToken(MustacheTokenType.EscapedVariable, "NAME", 0, 0),
                new MustacheToken(MustacheTokenType.Section, "ESCLAMATION", 0, 0),
                new MustacheToken(MustacheTokenType.Value, "!", 0, 0),
                new MustacheToken(MustacheTokenType.SectionEnd, null, 0, 0),
                new MustacheToken(MustacheTokenType.InvertedSection, "ESCLAMATION", 0, 0),
                new MustacheToken(MustacheTokenType.Value, ".", 0, 0),
                new MustacheToken(MustacheTokenType.SectionEnd, "ESCLAMATION", 0, 0),
            };

            var tokens = parser.InitialTokens;
            Assert.Equal(expectedTokens.Count, tokens.Count);

            for (int i = 0; i < tokens.Count; i++)
            {
                Assert.Equal(expectedTokens[i].Type, tokens[i].Type);
                Assert.Equal(expectedTokens[i].Value, tokens[i].Value);
            }
        }

        [Fact]
        public void TestSyntaxAnalysis()
        {
            MustacheParser parser = new MustacheParser();
            parser.Template = "Hello, {{{NAME}}}{{ #if ESCLAMATION }}!{{/if}}{{{^ESCLAMATION}}}.{{{/ESCLAMATION}}}";

            List<MustacheToken> expectedTokens = new List<MustacheToken>
            {
                new MustacheToken(MustacheTokenType.Value, "Hello, ", 0, 0),
                new MustacheToken(MustacheTokenType.EscapedVariable, "NAME", 0, 0),
                new MustacheToken(MustacheTokenType.Section, "ESCLAMATION", 0, 0),
                new MustacheToken(MustacheTokenType.InvertedSection, "ESCLAMATION", 0, 0),
            };

            var tokens = parser.ResultTokens;
            Assert.Equal(expectedTokens.Count, tokens.Count);

            for (int i = 0; i < tokens.Count; i++)
            {
                Assert.Equal(expectedTokens[i].Type, tokens[i].Type);
                Assert.Equal(expectedTokens[i].Value, tokens[i].Value);
            }
        }

        [Fact]
        public void TestVariableNames()
        {
            MustacheParser parser = new MustacheParser();
            parser.Template = "Hello, {{{NAME}}}{{ #if ESCLAMATION }}!{{/if}}{{{^ESCLAMATION}}}.{{{/ESCLAMATION}}}";

            Assert.Equal(2, parser.VariableNames.Count);
            Assert.Equal("NAME", parser.VariableNames[0]);
            Assert.Equal("ESCLAMATION", parser.VariableNames[1]);
        }
    }
}
