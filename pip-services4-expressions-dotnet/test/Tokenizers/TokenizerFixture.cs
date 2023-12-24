using PipServices4.Expressions.Tokenizers;
using System.Collections.Generic;
using Xunit;

namespace PipServices4.Expressions.Test.Tokenizers
{
    /// <summary>
    /// Implements test utilities to Tokenzier tests
    /// </summary>
    public class TokenizerFixture
    {
        /// <summary>
        /// Checks is expected tokens matches actual tokens.
        /// </summary>
        /// <param name="expectedTokens">An array with expected tokens.</param>
        /// <param name="actualTokens">An array with actual tokens.</param>
        public static void AssertAreEqualsTokenLists(
            Token[] expectedTokens, IList<Token> actualTokens)
        {
            Assert.Equal(expectedTokens.Length, actualTokens.Count);

            for (int i = 0; i < expectedTokens.Length; i++)
            {
                Assert.Equal(expectedTokens[i].Type, actualTokens[i].Type);
                Assert.Equal(expectedTokens[i].Value, actualTokens[i].Value);
            }
        }
    }
}
