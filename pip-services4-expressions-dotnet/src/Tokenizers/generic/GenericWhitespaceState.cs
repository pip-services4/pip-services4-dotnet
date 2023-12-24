using System;
using System.Text;
using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers.Utilities;

namespace PipServices4.Expressions.Tokenizers.Generic
{
    /// <summary>
    /// A whitespace state ignores whitespace (such as blanks and tabs), and returns the tokenizer's
    /// next token. By default, all characters from 0 to 32 are whitespace.
    /// </summary>
    public class GenericWhitespaceState : IWhitespaceState
    {
        private CharReferenceMap<bool> _map = new CharReferenceMap<bool>();

        /// <summary>
        /// Constructs a whitespace state with a default idea of what characters are, in fact, whitespace.
        /// </summary>
        public GenericWhitespaceState()
        {
            SetWhitespaceChars('\0', ' ', true);
        }

        /// <summary>
        /// Ignore whitespace (such as blanks and tabs), and return the tokenizer's next token.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="tokenizer"></param>
        /// <returns>The tokenizer's next token</returns>
        public Token NextToken(IScanner scanner, ITokenizer tokenizer)
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

            return new Token(TokenType.Whitespace, tokenValue.ToString(), line, column);
        }

        /// <summary>
        /// Establish the given characters as whitespace to ignore.
        /// </summary>
        /// <param name="fromSymbol">First character index of the interval.</param>
        /// <param name="toSymbol">Last character index of the interval.</param>
        /// <param name="enable"><code>true</code> if this state should ignore characters in the given range.</param>
        public void SetWhitespaceChars(char fromSymbol, char toSymbol, bool enable)
        {
            _map.AddInterval(fromSymbol, toSymbol, enable);
        }

        /// <summary>
        /// Clears definitions of whitespace characters.
        /// </summary>
        public void ClearWhitespaceChars()
        {
            _map.Clear();
        }

    }
}
