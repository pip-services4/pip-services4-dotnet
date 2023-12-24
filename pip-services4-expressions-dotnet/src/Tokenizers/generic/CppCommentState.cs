using System;
using System.Text;

using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers.Utilities;

namespace PipServices4.Expressions.Tokenizers.Generic
{
    /// <summary>
    /// This state will either delegate to a comment-handling state, or return a token with just a slash in it.
    /// </summary>
    public class CppCommentState : ICommentState
    {
        /// <summary>
        /// Ignore everything up to a closing star and slash, and then return the tokenizer's next token.
        /// </summary>
        /// <param name="scanner"></param>
        /// <returns></returns>
        protected static string GetMultiLineComment(IScanner scanner)
        {
            StringBuilder result = new StringBuilder();
            char lastSymbol = '\0';
            for (char nextSymbol = scanner.Read(); !CharValidator.IsEof(nextSymbol); nextSymbol = scanner.Read())
            {
                result.Append(nextSymbol);
                if (lastSymbol == '*' && nextSymbol == '/')
                {
                    break;
                }
                lastSymbol = nextSymbol;
            }
            return result.ToString();
        }

        /// <summary>
        /// Ignore everything up to an end-of-line and return the tokenizer's next token.
        /// </summary>
        /// <param name="scanner"></param>
        /// <returns></returns>
        protected static string GetSingleLineComment(IScanner scanner)
        {
            StringBuilder result = new StringBuilder();
            char nextSymbol;
            for (nextSymbol = scanner.Read();
                !CharValidator.IsEof(nextSymbol) && !CharValidator.IsEol(nextSymbol);
                nextSymbol = scanner.Read())
            {
                result.Append(nextSymbol);
            }
            if (CharValidator.IsEol(nextSymbol))
            {
                scanner.Unread();
            }
            return result.ToString();
        }

        /// <summary>
        /// Either delegate to a comment-handling state, or return a token with just a slash in it.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="tokenizer"></param>
        /// <returns>Either just a slash token, or the results of delegating to a comment-handling state.</returns>
        public virtual Token NextToken(IScanner scanner, ITokenizer tokenizer)
        {
            char firstSymbol = scanner.Read();
            int line = scanner.PeekLine();
            int column = scanner.PeekColumn();

            if (firstSymbol != '/')
            {
                scanner.Unread();
                throw new InvalidProgramException("Incorrect usage of CppCommentState.");
            }

            char secondSymbol = scanner.Read();
            if (secondSymbol == '*')
            {
                return new Token(TokenType.Comment, "/*" + GetMultiLineComment(scanner), line, column);
            }
            else if (secondSymbol == '/')
            {
                return new Token(TokenType.Comment, "//" + GetSingleLineComment(scanner), line, column);
            }
            else
            {
                if (!CharValidator.IsEof(secondSymbol))
                {
                    scanner.Unread();
                }
                if (!CharValidator.IsEof(firstSymbol))
                {
                    scanner.Unread();
                }
                return tokenizer.SymbolState.NextToken(scanner, tokenizer);
            }
        }
    }
}
