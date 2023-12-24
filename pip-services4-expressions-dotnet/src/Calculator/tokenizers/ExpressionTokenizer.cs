using System;

using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;

namespace PipServices4.Expressions.Calculator.Tokenizers
{
    /// <summary>
    /// Implement tokenizer to perform lexical analysis for expressions.
    /// </summary>
    public class ExpressionTokenizer : AbstractTokenizer
    {
        /// <summary>
        /// Constructs an instance of this class.
        /// </summary>
        public ExpressionTokenizer()
        {
            DecodeStrings = false;

            WhitespaceState = new GenericWhitespaceState();

            SymbolState = new ExpressionSymbolState();
            NumberState = new ExpressionNumberState();
            QuoteState = new ExpressionQuoteState();
            WordState = new ExpressionWordState();
            CommentState = new CCommentState();

            ClearCharacterStates();
            SetCharacterState('\x0000', '\xffff', SymbolState);
            SetCharacterState('\0', ' ', WhitespaceState);

            SetCharacterState('a', 'z', WordState);
            SetCharacterState('A', 'Z', WordState);
            SetCharacterState('\x00c0', '\x00ff', WordState);
            SetCharacterState('_', '_', WordState);

            SetCharacterState('0', '9', NumberState);
            SetCharacterState('-', '-', NumberState);
            SetCharacterState('.', '.', NumberState);

            SetCharacterState('"', '"', QuoteState);
            SetCharacterState('\'', '\'', QuoteState);

            SetCharacterState('/', '/', CommentState);
        }
    }
}
