using System;

using PipServices4.Expressions.IO;

namespace PipServices4.Expressions.Tokenizers.Generic
{
    /// <summary>
    /// The idea of a symbol is a character that stands on its own, such as an ampersand or a parenthesis.
    /// For example, when tokenizing the expression <code>(isReady)& (isWilling) </code>, a typical
    /// tokenizer would return 7 tokens, including one for each parenthesis and one for the ampersand.
    /// Thus a series of symbols such as <code>)&( </code> becomes three tokens, while a series of letters
    /// such as <code>isReady</code> becomes a single word token.
    /// <p/>
    /// Multi-character symbols are an exception to the rule that a symbol is a standalone character.  
    /// For example, a tokenizer may want less-than-or-equals to tokenize as a single token. This class
    /// provides a method for establishing which multi-character symbols an object of this class should
    /// treat as single symbols. This allows, for example, <code>"cat &lt;= dog"</code> to tokenize as 
    /// three tokens, rather than splitting the less-than and equals symbols into separate tokens.
    /// <p/>
    /// By default, this state recognizes the following multi-character symbols:
    /// <code>!=, :-, &lt;=, &gt;=</code>
    /// </summary>
    public class GenericSymbolState : ISymbolState
    {
        private SymbolRootNode _symbols = new SymbolRootNode();

        //private SymbolRootNode Symbols 
        //{ 
        //    get { return _symbols; }
        //    set { _symbols = value; } 
        //}

        /// <summary>
        /// Return a symbol token from a scanner.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="tokenizer"></param>
        /// <returns>A symbol token from a scanner.</returns>
        public virtual Token NextToken(IScanner scanner, ITokenizer tokenizer)
        {
            return _symbols.NextToken(scanner);
        }

        /// <summary>
        /// Add a multi-character symbol.
        /// </summary>
        /// <param name="value">The symbol to add, such as "=:="</param>
        public void Add(string value, TokenType tokenType)
        {
            _symbols.Add(value, tokenType);
        }
    }
}
