using System;

using PipServices4.Expressions.IO;

namespace PipServices4.Expressions.Tokenizers.Generic
{
    /// <summary>
    /// This class is a special case of a <code>SymbolNode</code>. A <code>SymbolRootNode</code>
    /// object has no symbol of its own, but has children that represent all possible symbols.
    /// </summary>
    public class SymbolRootNode : SymbolNode
    {
        /// <summary>
        /// Creates and initializes a root node.
        /// </summary>
        public SymbolRootNode()
            : base(null, '\0')
        {
        }

        /// <summary>
        /// Add the given string as a symbol.
        /// </summary>
        /// <param name="value">The character sequence to add.</param>
        public void Add(string value, TokenType tokenType)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value must have at least 1 character");
            }
            SymbolNode childNode = EnsureChildWithChar(value[0]);
            if (childNode.TokenType == TokenType.Unknown)
            {
                childNode.Valid = true;
                childNode.TokenType = TokenType.Symbol;
            }
            childNode.AddDescendantLine(value.Substring(1), tokenType);
        }

        /// <summary>
        /// Return a symbol string from a scanner.
        /// </summary>
        /// <param name="scanner">A scanner to read from</param>
        /// <param name="firstChar">The first character of this symbol, already read from the scanner.</param>
        /// <returns>A symbol string from a scanner</returns>
        public Token NextToken(IScanner scanner)
        {
            char nextSymbol = scanner.Read();
            int line = scanner.PeekLine();
            int column = scanner.PeekColumn();

            SymbolNode childNode = FindChildWithChar(nextSymbol);
            
            if (childNode != null)
            {
                childNode = childNode.DeepestRead(scanner);
                childNode = childNode.UnreadToValid(scanner);
                return new Token(childNode.TokenType, childNode.Ancestry(), line, column);
            }
            else
            {
                return new Token(TokenType.Symbol, nextSymbol.ToString(), line, column);
            }
        }
    }
}
