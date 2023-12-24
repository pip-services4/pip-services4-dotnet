using System;
using PipServices4.Expressions.Calculator.Parsers;
using PipServices4.Expressions.Variants;
using Xunit;

namespace PipServices4.Expressions.Test.Calculator.Parsers
{
    public class ExpressionParserTest
    {
        [Fact]
        public void TestParseString()
        {
            var parser = new ExpressionParser();
            parser.Expression = "(2+2)*ABS(-2)";

            var expectedTokens = new ExpressionToken[] {
                new ExpressionToken(ExpressionTokenType.Constant, Variant.FromInteger(2), 0, 0),
                new ExpressionToken(ExpressionTokenType.Constant, Variant.FromInteger(2), 0, 0),
                new ExpressionToken(ExpressionTokenType.Plus, Variant.Empty, 0, 0),
                new ExpressionToken(ExpressionTokenType.Constant, Variant.FromInteger(2), 0, 0),
                new ExpressionToken(ExpressionTokenType.Unary, Variant.Empty, 0, 0),
                new ExpressionToken(ExpressionTokenType.Constant, Variant.FromInteger(1), 0, 0),
                new ExpressionToken(ExpressionTokenType.Function, Variant.FromString("ABS"), 0, 0),
                new ExpressionToken(ExpressionTokenType.Star, Variant.Empty, 0, 0),
            };

            var tokens = parser.ResultTokens;
            Assert.Equal(expectedTokens.Length, tokens.Count);

            for (var i = 0; i < tokens.Count; i++)
            {
                Assert.Equal(expectedTokens[i].Type, tokens[i].Type);
                Assert.Equal(expectedTokens[i].Value.Type, tokens[i].Value.Type);
                Assert.Equal(expectedTokens[i].Value.AsObject, tokens[i].Value.AsObject);
            }
        }
    }
}
