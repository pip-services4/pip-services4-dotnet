using System;

namespace PipServices4.Expressions.Tokenizers.Generic
{
    /// <summary>
    /// Implements a default tokenizer class.
    /// </summary>
    public class GenericTokenizer : AbstractTokenizer
    {
        public GenericTokenizer()
        {
            SymbolState = new GenericSymbolState();
            SymbolState.Add("<>", TokenType.Symbol);
            SymbolState.Add("<=", TokenType.Symbol);
            SymbolState.Add(">=", TokenType.Symbol);

            NumberState = new GenericNumberState();
            QuoteState = new GenericQuoteState();
            WhitespaceState = new GenericWhitespaceState();
            WordState = new GenericWordState();
            CommentState = new GenericCommentState();

            ClearCharacterStates();
            SetCharacterState('\x0000', '\x00ff', SymbolState);
            SetCharacterState('\x0000', ' ', WhitespaceState);

            SetCharacterState('a', 'z', WordState);
            SetCharacterState('A', 'Z', WordState);
            SetCharacterState('\x00c0', '\x00ff', WordState);
            SetCharacterState('\x0100', '\xffff', WordState);

            SetCharacterState('-', '-', NumberState);
            SetCharacterState('0', '9', NumberState);
            SetCharacterState('.', '.', NumberState);

            SetCharacterState('\"', '\"', QuoteState);
            SetCharacterState('\'', '\'', QuoteState);

            SetCharacterState('#', '#', CommentState);
        }
    }
}
