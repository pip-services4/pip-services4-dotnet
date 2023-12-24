using System;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// Defines an interface for tokenizer state that processes quoted strings.
    /// </summary>
    public interface IQuoteState : ITokenizerState
    {
        /// <summary>
        /// Encodes a string value.
        /// </summary>
        /// <param name="value">A string value to be encoded.</param>
        /// <param name="quoteSymbol">A string quote character.</param>
        /// <returns>An encoded string.</returns>
        string EncodeString(string value, char quoteSymbol);

        /// <summary>
        /// Decodes a string value.
        /// </summary>
        /// <param name="value">A string value to be decoded.</param>
        /// <param name="quoteSymbol">A string quote character.</param>
        /// <returns>An decoded string.</returns>
        string DecodeString(string value, char quoteSymbol);
    }
}
