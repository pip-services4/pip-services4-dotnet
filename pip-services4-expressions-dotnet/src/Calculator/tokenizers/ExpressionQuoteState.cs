using System;
using System.Text;

using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Utilities;

namespace PipServices4.Expressions.Calculator.Tokenizers
{
    /// <summary>
    /// Implements an Expression-specific quote string state object.
    /// </summary>
    internal class ExpressionQuoteState : IQuoteState
    {
        /// <summary>
        /// Gets the next token from the stream started from the character linked to this state.
        /// </summary>
        /// <param name="scanner">A textual string to be tokenized.</param>
        /// <param name="tokenizer">A tokenizer class that controls the process.</param>
        /// <returns>The next token from the top of the stream.</returns>
        public Token NextToken(IScanner scanner, ITokenizer tokenizer)
        {
            char firstSymbol = scanner.Read();
            int line = scanner.PeekLine();
            int column = scanner.PeekColumn();
            StringBuilder tokenValue = new StringBuilder();
            tokenValue.Append(firstSymbol);

            for (char nextSymbol = scanner.Read(); !CharValidator.IsEof(nextSymbol); nextSymbol = scanner.Read())
            {
                tokenValue.Append(nextSymbol);
                if (nextSymbol == firstSymbol)
                {
                    if (scanner.Peek() == firstSymbol)
                    {
                        nextSymbol = scanner.Read();
                        tokenValue.Append(nextSymbol);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return new Token(firstSymbol == '"' ? TokenType.Word : TokenType.Quoted,
                tokenValue.ToString(), line, column);
        }

        /// <summary>
        /// Encodes a string value.
        /// </summary>
        /// <param name="value">A string value to be encoded.</param>
        /// <param name="quoteSymbol">A string quote character.</param>
        /// <returns>An encoded string.</returns>
        public string EncodeString(string value, char quoteSymbol)
        {
            if (value != null)
            {
                StringBuilder result = new StringBuilder();
                string quoteString = Char.ToString(quoteSymbol);
                result.Append(quoteSymbol);
                result.Append(value.Replace(quoteString, quoteString + quoteString));
                result.Append(quoteSymbol);
                return result.ToString();
            }
            return null;
        }

        /// <summary>
        /// Decodes a string value.
        /// </summary>
        /// <param name="value">A string value to be decoded.</param>
        /// <param name="quoteChar">A string quote character.</param>
        /// <returns>An decoded string.</returns>
        public string DecodeString(string value, char quoteSymbol)
        {
            if (value != null)
            {
                if (value.Length >= 2 && value[0] == quoteSymbol && value[value.Length - 1] == quoteSymbol)
                {
                    string quoteString = Char.ToString(quoteSymbol);
                    return value.Substring(1, value.Length - 2).Replace(quoteString + quoteString, quoteString);
                }
                return value;
            }
            return null;
        }
    }
}
