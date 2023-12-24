using System;

using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;

namespace PipServices4.Expressions.Calculator.Tokenizers
{
    /// <summary>
    /// Implements a symbol state object.
    /// </summary>
    internal class ExpressionSymbolState : GenericSymbolState
    {
        /// <summary>
        /// Constructs an instance of this class.
        /// </summary>
        public ExpressionSymbolState()
        {
            Add("<=", TokenType.Symbol);
            Add(">=", TokenType.Symbol);
            Add("<>", TokenType.Symbol);
            Add("!=", TokenType.Symbol);
            Add(">>", TokenType.Symbol);
            Add("<<", TokenType.Symbol);
        }
    }
}
