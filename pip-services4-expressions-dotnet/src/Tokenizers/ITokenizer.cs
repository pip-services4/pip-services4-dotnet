using System.Collections.Generic;

using PipServices4.Expressions.IO;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// A tokenizer divides a string into tokens. This class is highly customizable with regard
    /// to exactly how this division occurs, but it also has defaults that are suitable for many
    /// languages. This class assumes that the character values read from the string lie in
    /// the range 0-255. For example, the Unicode value of a capital A is 65,
    /// so <code> System.out.println((char)65); </code> prints out a capital A.
    /// <p>
    /// The behavior of a tokenizer depends on its character state table. This table is an array
    /// of 256 <code>TokenizerState</code> states. The state table decides which state to enter
    /// upon reading a character from the input string.
    /// <p>
    /// For example, by default, upon reading an 'A', a tokenizer will enter a "word" state.
    /// This means the tokenizer will ask a <code>WordState</code> object to consume the 'A',
    /// along with the characters after the 'A' that form a word. The state's responsibility
    /// is to consume characters and return a complete token.
    /// <p>
    /// The default table sets a SymbolState for every character from 0 to 255,
    /// and then overrides this with:<blockquote><pre>
    /// From    To     State
    /// 0     ' '    whitespaceState
    /// 'a'    'z'    wordState
    /// 'A'    'Z'    wordState
    /// 160     255    wordState
    /// '0'    '9'    numberState
    /// '-'    '-'    numberState
    /// '.'    '.'    numberState
    /// '"'    '"'    quoteState
    /// '\''   '\''    quoteState
    /// '/'    '/'    slashState
    /// </pre></blockquote>
    /// In addition to allowing modification of the state table, this class makes each of the states
    /// above available. Some of these states are customizable. For example, wordState allows customization
    /// of what characters can be part of a word, after the first character.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Skips unknown characters.
        /// </summary>
        bool SkipUnknown { get; set; }

        /// <summary>
        /// Skips whitespaces.
        /// </summary>
        bool SkipWhitespaces { get; set; }

        /// <summary>
        /// Skips comments.
        /// </summary>
        bool SkipComments { get; set; }

        /// <summary>
        /// Skips End-Of-File token at the end of stream.
        /// </summary>
        bool SkipEof { get; set; }

        /// <summary>
        /// Merges whitespaces.
        /// </summary>
        bool MergeWhitespaces { get; set; }

        /// <summary>
        /// Unifies numbers: "Integers" and "Floats" makes just "Numbers"
        /// </summary>
        bool UnifyNumbers { get; set; }

        /// <summary>
        /// Decodes quoted strings.
        /// </summary>
        bool DecodeStrings { get; set; }

        /// <summary>
        /// A token state to process comments.
        /// </summary>
        ICommentState CommentState { get; }

        /// <summary>
        /// A token state to process numbers.
        /// </summary>
        INumberState NumberState { get; }

        /// <summary>
        /// A token state to process quoted strings.
        /// </summary>
        IQuoteState QuoteState { get; }

        /// <summary>
        /// A token state to process symbols (single like "=" or muti-character like "<>")
        /// </summary>
        ISymbolState SymbolState { get; }

        /// <summary>
        /// A token state to process white space delimiters.
        /// </summary>
        IWhitespaceState WhitespaceState { get; }

        /// <summary>
        /// A token state to process words or indentificators.
        /// </summary>
        IWordState WordState { get; }

        /// <summary>
        /// The stream scanner to tokenize.
        /// </summary>
        IScanner Scanner { get; set; }

        /// <summary>
        /// Checks if there is the next token exist.
        /// </summary>
        /// <returns><code>true</code> if scanner has the next token.</returns>
        bool HasNextToken();

        /// <summary>
        /// Gets the next token from the scanner.
        /// </summary>
        /// <returns>Next token of <code>null</code> if there are no more tokens left.</returns>
        Token NextToken();

        /// <summary>
        /// Tokenizes a textual stream into a list of token structures.
        /// </summary>
        /// <param name="scanner">A textual stream to be tokenized.</param>
        /// <returns>A list of token structures.</returns>
        IList<Token> TokenizeStream(IScanner scanner);

        /// <summary>
        /// Tokenizes a string buffer into a list of tokens structures.
        /// </summary>
        /// <param name="buffer">A string buffer to be tokenized.</param>
        /// <returns>A list of token structures.</returns>
        IList<Token> TokenizeBuffer(string buffer);

        /// <summary>
        /// Tokenizes a textual stream into a list of strings.
        /// </summary>
        /// <param name="scanner">A textual stream to be tokenized.</param>
        /// <returns>A list of token strings.</returns>
        IList<string> TokenizeStreamToStrings(IScanner scanner);

        /// <summary>
        /// Tokenizes a string buffer into a list of strings.
        /// </summary>
        /// <param name="buffer">A string buffer to be tokenized.</param>
        /// <returns>A list of token strings.</returns>
        IList<string> TokenizeBufferToStrings(string buffer);
    }

}

