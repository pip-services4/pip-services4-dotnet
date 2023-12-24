using System;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// Defines an interface for tokenizer state that processes whitespaces (' ', '\t')
    /// </summary>
    public interface IWhitespaceState : ITokenizerState
    {
        /// <summary>
        /// Establish the given characters as whitespace to ignore.
        /// </summary>
        /// <param name="fromSymbol">First character index of the interval.</param>
        /// <param name="toSymbol">Last character index of the interval.</param>
        /// <param name="enable"><code>true</code> if this state should ignore characters in the given range.</param>
        void SetWhitespaceChars(char fromSymbol, char toSymbol, bool enable);

        /// <summary>
        /// Clears definitions of whitespace characters.
        /// </summary>
        void ClearWhitespaceChars();
    }
}
