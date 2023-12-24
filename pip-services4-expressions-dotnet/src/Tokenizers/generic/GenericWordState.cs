using System;
using System.Text;
using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers.Utilities;

namespace PipServices4.Expressions.Tokenizers.Generic
{
    /// <summary>
    /// A wordState returns a word from a scanner. Like other states, a tokenizer transfers the job
    /// of reading to this state, depending on an initial character. Thus, the tokenizer decides
    /// which characters may begin a word, and this state determines which characters may appear
    /// as a second or later character in a word. These are typically different sets of characters;
    /// in particular, it is typical for digits to appear as parts of a word, but not
    /// as the initial character of a word.
    /// <p/>
    /// By default, the following characters may appear in a word.
    /// The method <code>setWordChars()</code> allows customizing this.
    /// <blockquote><pre>
    /// From    To
    ///   'a', 'z'
    ///   'A', 'Z'
    ///   '0', '9'
    /// 
    ///    as well as: minus sign, underscore, and apostrophe.
    /// </pre></blockquote>
    /// </summary>
    public class GenericWordState : IWordState
    {
        private CharReferenceMap<bool> _map = new CharReferenceMap<bool>();

        /// <summary>
        /// Constructs a word state with a default idea of what characters
        /// are admissible inside a word (as described in the class comment).
        /// </summary>
        public GenericWordState()
        {
            SetWordChars('a', 'z', true);
            SetWordChars('A', 'Z', true);
            SetWordChars('0', '9', true);
            SetWordChars('-', '-', true);
            SetWordChars('_', '_', true);
            //SetWordChars('\u0039', '\u0039', true);
            SetWordChars('\x00c0', '\x00ff', true);
            SetWordChars('\x0100', '\xffff', true);
        }

        /// <summary>
        /// Ignore word (such as blanks and tabs), and return the tokenizer's next token.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="tokenizer"></param>
        /// <returns>The tokenizer's next token</returns>
        public virtual Token NextToken(IScanner scanner, ITokenizer tokenizer)
        {
            char nextSymbol;
            StringBuilder tokenValue = new StringBuilder();
            int line = scanner.PeekLine();
            int column = scanner.PeekColumn();
            for (nextSymbol = scanner.Read(); _map.Lookup(nextSymbol); nextSymbol = scanner.Read())
            {
                tokenValue.Append(nextSymbol);
            }

            if (!CharValidator.IsEof(nextSymbol))
            {
                scanner.Unread();
            }

            return new Token(TokenType.Word, tokenValue.ToString(), line, column);
        }

        /// <summary>
        /// Establish characters in the given range as valid characters for part of a word after
        /// the first character. Note that the tokenizer must determine which characters are valid
        /// as the beginning character of a word.
        /// </summary>
        /// <param name="fromSymbol">First character index of the interval.</param>
        /// <param name="toSymbol">Last character index of the interval.</param>
        /// <param name="enable"><code>true</code> if this state should use characters in the given range.</param>
        public void SetWordChars(char fromSymbol, char toSymbol, bool enable)
        {
            _map.AddInterval(fromSymbol, toSymbol, enable);
        }

        /// <summary>
        /// Clears definitions of word chars.
        /// </summary>
        public void ClearWordChars()
        {
            _map.Clear();
        }
    }
}
