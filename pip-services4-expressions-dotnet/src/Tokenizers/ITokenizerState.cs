using System;

using PipServices4.Expressions.IO;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// A tokenizerState returns a token, given a scanner, an initial character read from the scanner,
    /// and a tokenizer that is conducting an overall tokenization of the scanner. The tokenizer will
    /// typically have a character state table that decides which state to use, depending on an initial 
    /// character. If a single character is insufficient, a state such as <code>SlashState</code>
    /// will read a second character, and may delegate to another state, such as <code>SlashStarState</code>.
    /// This prospect of delegation is the reason that the <code>nextToken()</code>
    /// method has a tokenizer argument.
    /// </summary>
    public interface ITokenizerState
    {
        /// <summary>
        /// Gets the next token from the stream started from the character linked to this state.
        /// </summary>
        /// <param name="scanner">A textual string to be tokenized.</param>
        /// <param name="tokenizer">A tokenizer class that controls the process.</param>
        /// <returns>The next token from the top of the stream.</returns>
        Token NextToken(IScanner scanner, ITokenizer tokenizer);
    }
}
