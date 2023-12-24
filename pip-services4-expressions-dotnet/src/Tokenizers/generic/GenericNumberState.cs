using System;
using System.Text;

using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers.Utilities;

namespace PipServices4.Expressions.Tokenizers.Generic
{
    /// <summary>
    /// A NumberState object returns a number from a scanner. This state's idea of a number allows
    /// an optional, initial minus sign, followed by one or more digits. A decimal point and another string
    /// of digits may follow these digits.
    /// </summary>
    public class GenericNumberState : INumberState
    {
        /// <summary>
        /// Gets the next token from the stream started from the character linked to this state.
        /// </summary>
        /// <param name="scanner">A textual string to be tokenized.</param>
        /// <param name="tokenizer">A tokenizer class that controls the process.</param>
        /// <returns>The next token from the top of the stream.</returns>
        public virtual Token NextToken(IScanner scanner, ITokenizer tokenizer)
        {
            bool absorbedDot = false;
            bool gotADigit = false;
            StringBuilder tokenValue = new StringBuilder("");
            char nextSymbol = scanner.Read();
            int line = scanner.PeekLine();
            int column = scanner.PeekColumn();

            // Parses leading minus.
            if (nextSymbol == '-')
            {
                tokenValue.Append('-');
                nextSymbol = scanner.Read();
            }

            // Parses digits before decimal separator.
            for (; CharValidator.IsDigit(nextSymbol)
                && !CharValidator.IsEof(nextSymbol); nextSymbol = scanner.Read())
            {
                gotADigit = true;
                tokenValue.Append(nextSymbol);
            }

            // Parses part after the decimal separator.
            if (nextSymbol == '.')
            {
                absorbedDot = true;
                tokenValue.Append('.');
                nextSymbol = scanner.Read();

                // Absorb all digits.
                for (; CharValidator.IsDigit(nextSymbol)
                    && !CharValidator.IsEof(nextSymbol); nextSymbol = scanner.Read())
                {
                    gotADigit = true;
                    tokenValue.Append(nextSymbol);
                }
            }

            // Pushback last unprocessed symbol.
            if (!CharValidator.IsEof(nextSymbol))
            {
                scanner.Unread();
            }

            // Process the result.
            if (!gotADigit)
            {
                scanner.UnreadMany(tokenValue.ToString().Length);
                if (tokenizer != null && tokenizer.SymbolState != null)
                {
                    return tokenizer.SymbolState.NextToken(scanner, tokenizer);
                }
                else
                {
                    throw new IncorrectStateException(null, "TOKENIZER_EXCEPTION", "Tokenizer must have an assigned symbol state.");
                }
            }

            return new Token(absorbedDot ? TokenType.Float : TokenType.Integer, tokenValue.ToString(), line, column);
        }
    }
}
