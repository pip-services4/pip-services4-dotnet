using System;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// Defines an interface for tokenizer state that processes delimiters.
    /// </summary>
    public interface ISymbolState : ITokenizerState
    {
        /// <summary>
        /// Add a multi-character symbol.
        /// </summary>
        /// <param name="value">The symbol to add, such as "=:="</param>
        void Add(string value, TokenType tokenType);
    }
}
