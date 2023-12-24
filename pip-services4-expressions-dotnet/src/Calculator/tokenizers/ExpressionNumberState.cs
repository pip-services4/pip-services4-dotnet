using System;
using System.Text;

using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;
using PipServices4.Expressions.Tokenizers.Utilities;

namespace PipServices4.Expressions.Calculator.Tokenizers
{
    /// <summary>
    /// Implements an Expression-specific number state object.
    /// </summary>
    internal class ExpressionNumberState : GenericNumberState
    {
        /// <summary>
        /// Gets the next token from the stream started from the character linked to this state.
        /// </summary>
        /// <param name="scanner">A textual string to be tokenized.</param>
        /// <param name="tokenizer">A tokenizer class that controls the process.</param>
        /// <returns>The next token from the top of the stream.</returns>
        public override Token NextToken(IScanner scanner, ITokenizer tokenizer)
        {
            int line = scanner.PeekLine();
            int column = scanner.PeekColumn();

            // Process leading minus.
            if (scanner.Peek() == '-')
            {
                return tokenizer.SymbolState.NextToken(scanner, tokenizer);
            }

            // Process numbers using base class algorithm.
            Token token = base.NextToken(scanner, tokenizer);

            // Exit if number was not detected.
            if (token.Type != TokenType.Integer && token.Type != TokenType.Float)
            {
                return token;
            }

            // Exit if number is not in scientific format.
            char nextChar = scanner.Peek();
            if (nextChar != 'e' && nextChar != 'E')
            {
                return token;
            }

            StringBuilder tokenValue = new StringBuilder();
            tokenValue.Append(scanner.Read());

            // Process '-' or '+' in mantissa
            nextChar = scanner.Peek();
            if (nextChar == '-' || nextChar == '+')
            {
                tokenValue.Append(scanner.Read());
                nextChar = scanner.Peek();
            }

            // Exit if mantissa has no digits.
            if (!CharValidator.IsDigit(nextChar))
            {
                scanner.UnreadMany(tokenValue.ToString().Length);
                return token;
            }

            // Process matissa digits
            for (; CharValidator.IsDigit(nextChar); nextChar = scanner.Peek())
            {
                tokenValue.Append(scanner.Read());
            }

            return new Token(TokenType.Float, token.Value + tokenValue.ToString(), line, column);
        }
    }
}
