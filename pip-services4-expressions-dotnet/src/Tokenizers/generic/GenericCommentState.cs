using System;
using System.Text;

using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers.Utilities;

namespace PipServices4.Expressions.Tokenizers.Generic
{
    /// <summary>
    /// A CommentState object returns a comment from a reader.
    /// </summary>
    public class GenericCommentState : ICommentState
    {
        /// <summary>
        ///  Either delegate to a comment-handling state, or return a token with just a slash in it.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="tokenizer"></param>
        /// <returns>Either just a slash token, or the results of delegating to a comment-handling state</returns>
        public virtual Token NextToken(IScanner scanner, ITokenizer tokenizer)
        {
            StringBuilder tokenValue = new StringBuilder();
            char nextSymbol;
            int line = scanner.PeekLine();
            int column = scanner.PeekColumn();
            for (nextSymbol = scanner.Read(); !CharValidator.IsEof(nextSymbol)
                && nextSymbol != '\n' && nextSymbol != '\r'; nextSymbol = scanner.Read())
            {
                tokenValue.Append(nextSymbol);
            }
            if (!CharValidator.IsEof(nextSymbol))
            {
                scanner.Unread();
            }

            return new Token(TokenType.Comment, tokenValue.ToString(), line, column);
        }
    }
}
