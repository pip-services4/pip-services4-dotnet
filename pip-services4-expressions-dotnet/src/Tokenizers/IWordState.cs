using System;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// Defines an interface for tokenizer state that processes words, identificators or keywords
    /// </summary>
    public interface IWordState : ITokenizerState
    {
        /// <summary>
        /// Establish characters in the given range as valid characters for part of a word after
        /// the first character. Note that the tokenizer must determine which characters are valid
        /// as the beginning character of a word.
        /// </summary>
        /// <param name="fromSymbol">First character index of the interval.</param>
        /// <param name="toSymbol">Last character index of the interval.</param>
        /// <param name="enable"><code>true</code> if this state should use characters in the given range.</param>
        void SetWordChars(char fromSymbol, char toSymbol, bool enable);

        /// <summary>
        /// Clears definitions of word chars.
        /// </summary>
        void ClearWordChars();
    }
}
