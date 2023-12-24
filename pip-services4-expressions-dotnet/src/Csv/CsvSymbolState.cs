using System;

using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;

namespace PipServices4.Expressions.Csv
{
    /// <summary>
    /// Implements a symbol state to tokenize delimiters in CSV streams.
    /// </summary>
    public class CsvSymbolState : GenericSymbolState
    {
        /// <summary>
        /// Constructs this object with specified parameters.
        /// </summary>
        public CsvSymbolState()
        {
            Add("\n", TokenType.Eol);
            Add("\r", TokenType.Eol);
            Add("\r\n", TokenType.Eol);
            Add("\n\r", TokenType.Eol);
        }

        public override Token NextToken(IScanner scanner, ITokenizer tokenizer)
        {
            // Optimization...
            char nextSymbol = scanner.Read();
            int line = scanner.PeekLine();
            int column = scanner.PeekColumn();

            if (nextSymbol != '\n' && nextSymbol != '\r')
            {
                return new Token(TokenType.Symbol, nextSymbol.ToString(), line, column);
            }
            else
            {
                scanner.Unread();
                return base.NextToken(scanner, tokenizer);
            }
        }
    }
}
