using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Tokenizers.Generic;

namespace PipServices4.Expressions.Mustache.Tokenizers
{

    public class MustacheTokenizer : AbstractTokenizer
    {
        private bool _special = true;
        private ITokenizerState _specialState;

        /// <summary>
        /// Constructs this object with default parameters.
        /// </summary>
        public MustacheTokenizer() : base()
        {
            SymbolState = new GenericSymbolState();
            SymbolState.Add("{{", TokenType.Symbol);
            SymbolState.Add("}}", TokenType.Symbol);
            SymbolState.Add("{{{", TokenType.Symbol);
            SymbolState.Add("}}}", TokenType.Symbol);

            NumberState = null;
            QuoteState = new GenericQuoteState();
            WhitespaceState = new GenericWhitespaceState();
            WordState = new GenericWordState();
            CommentState = null;
            _specialState = new MustacheSpecialState();

            ClearCharacterStates();
            SetCharacterState('\x0000', '\x00ff', SymbolState);
            SetCharacterState('\x0000', ' ', WhitespaceState);

            SetCharacterState('a', 'z', WordState);
            SetCharacterState('A', 'Z', WordState);
            SetCharacterState('0', '9', WordState);
            SetCharacterState('_', '_', WordState);
            SetCharacterState('\x00c0', '\x00ff', WordState);
            SetCharacterState('\x0100', '\xfffe', WordState);

            SetCharacterState('\"', '\"', WordState);
            SetCharacterState('\'', '\'', WordState);

            SkipWhitespaces = true;
            SkipComments = true;
            SkipEof = true;

        }

        protected override Token ReadNextToken()
        {
            Token token;

            if (_scanner == null)
                return null;

            // Check for initial state
            if (_nextToken == null && _lastTokenType == TokenType.Unknown)
                _special = true;

            // Process quotes
            if (_special)
            {
                token = _specialState.NextToken(_scanner, this);
                if (token != null && token.Value != "")
                {
                    return token;
                }
            }

            // Proces other tokens
            _special = false;
            token = base.ReadNextToken();
            // Switch to quote when '{{' or '{{{' symbols found
            if (token != null && (token.Value == "}}" || token.Value == "}}}"))
                _special = true;

            return token;
        }

    }
}
