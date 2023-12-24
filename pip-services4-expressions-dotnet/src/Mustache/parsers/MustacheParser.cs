using System;
using System.Collections.Generic;
using System.Text;

using PipServices4.Expressions.Mustache.Tokenizers;
using PipServices4.Expressions.Tokenizers;


namespace PipServices4.Expressions.Mustache.Parsers
{
    public class MustacheParser
    {
        private ITokenizer _tokenizer = new MustacheTokenizer();
        private string _template = "";
        private IList<Token> _originalTokens = new List<Token>();
        private IList<MustacheToken> _initialTokens = new List<MustacheToken>();
        private int _currentTokenIndex;
        private IList<string> _variableNames = new List<string>();
        private IList<MustacheToken> _resultTokens = new List<MustacheToken>();

        /// <summary>
        /// The mustache template.
        /// </summary>
        public string Template
        {
            get { return _template; }
            set { ParseString(value); }

        }

        public IList<Token> OriginalTokens
        {
            get { return _originalTokens; }
            set { ParseTokens(value); }
        }

        /// <summary>
        /// The list of original mustache tokens.
        /// </summary>
        public IList<MustacheToken> InitialTokens
        {
            get { return _initialTokens; }
        }

        /// <summary>
        /// The list of parsed mustache tokens.
        /// </summary>
        public IList<MustacheToken> ResultTokens
        {
            get { return _resultTokens; }
        }

        /// <summary>
        /// The list of found variable names.
        /// </summary>
        public IList<string> VariableNames
        {
            get { return _variableNames; }
        }

        public void ParseString(string mustache)
        {
            Clear();
            _template = mustache != null ? mustache.Trim() : "";
            _originalTokens = TokenizeMustache(_template);
            PerformParsing();
        }

        public void ParseTokens(IList<Token> tokens)
        {
            Clear();
            _originalTokens = tokens;
            _template = ComposeMustache(tokens);
            PerformParsing();
        }

        /// <summary>
        /// Clears parsing results.
        /// </summary>
        public void Clear()
        {
            _template = null;
            _originalTokens.Clear();
            _initialTokens.Clear();
            _resultTokens.Clear();
            _currentTokenIndex = 0;
            _variableNames.Clear();
        }

        /// <summary>
        /// Checks are there more tokens for processing.
        /// </summary>
        /// <returns><code>true</code> if some tokens are present.</returns>
        private bool HasMoreTokens()
        {
            return _currentTokenIndex < _initialTokens.Count;
        }

        /// <summary>
        /// Checks are there more tokens available and throws exception if no more tokens available.
        /// </summary>
        private void CheckForMoreTokens()
        {
            if (!HasMoreTokens())
                throw new MustacheException(null, MustacheErrorCode.UnexpectedEnd, "Unexpected end of mustache.", 0, 0);
        }

        /// <summary>
        /// Gets the current token object.
        /// </summary>
        /// <returns>The current token object.</returns>
        private MustacheToken GetCurrentToken()
        {
            return _currentTokenIndex < _initialTokens.Count ? _initialTokens[_currentTokenIndex] : null;
        }

        /// <summary>
        /// Gets the next token object.
        /// </summary>
        /// <returns>The next token object.</returns>
        private MustacheToken GetNextToken()
        {
            return (_currentTokenIndex + 1) < _initialTokens.Count ? _initialTokens[_currentTokenIndex + 1] : null;
        }

        /// <summary>
        /// Moves to the next token object.
        /// </summary>
        private void MoveToNextToken()
        {
            _currentTokenIndex++;
        }

        /// <summary>
        /// Adds an mustache to the result list
        /// </summary>
        /// <param name="type">The type of the token to be added.</param>
        /// <param name="value">The value of the token to be added.</param>
        /// <param name="line">The line where the token is.</param>
        /// <param name="column">The column number where the token is.</param>
        /// <returns></returns>
        private MustacheToken AddTokenToResult(MustacheTokenType type, string value, int line, int column)
        {
            var token = new MustacheToken(type, value, line, column);
            _resultTokens.Add(token);
            return token;
        }

        private IList<Token> TokenizeMustache(string mustache)
        {
            mustache = mustache != null ? mustache.Trim() : "";
            if (mustache.Length > 0)
            {
                _tokenizer.SkipWhitespaces = true;
                _tokenizer.SkipComments = true;
                _tokenizer.SkipEof = true;
                _tokenizer.DecodeStrings = true;

                return _tokenizer.TokenizeBuffer(mustache);
            }
            else
                return new List<Token>();
        }

        private string ComposeMustache(IList<Token> tokens)
        {
            StringBuilder builder = new StringBuilder();

            foreach (Token token in tokens)
                builder.Append(token.Value);

            return builder.ToString();
        }

        private void PerformParsing()
        {
            if (_originalTokens.Count > 0)
            {
                CompleteLexicalAnalysis();
                PerformSyntaxAnalysis();
                if (HasMoreTokens())
                {
                    var token = GetCurrentToken();
                    throw new MustacheException(null, MustacheErrorCode.ErrorNear, "Syntax error near " + token.Value, token.Line, token.Column);
                }
                LookupVariables();
            }
        }

        /// <summary>
        /// Tokenizes the given mustache and prepares an initial tokens list.
        /// </summary>
        private void CompleteLexicalAnalysis()
        {
            MustacheLexicalState state = MustacheLexicalState.Value;
            string closingBracket = null;
            string operator1 = null;
            string operator2 = null;
            string variable = null;

            foreach (Token token in _originalTokens)
            {
                MustacheTokenType tokenType = MustacheTokenType.Unknown;
                StringBuilder tokenValue = null;

                if (state == MustacheLexicalState.Comment)
                {
                    if (token.Value == "}}" || token.Value == "}}}")
                        state = MustacheLexicalState.Closure;
                    else
                        continue;
                }

                switch (token.Type)
                {
                    case TokenType.Special:
                        if (state == MustacheLexicalState.Value)
                        {
                            tokenType = MustacheTokenType.Value;
                            tokenValue = new StringBuilder(token.Value);
                        }
                        break;
                    case TokenType.Symbol:
                        if (state == MustacheLexicalState.Value && (token.Value == "{{" || token.Value == "{{{"))
                        {
                            closingBracket = token.Value == "{{" ? "}}" : "}}}";
                            state = MustacheLexicalState.Operator1;
                            continue;
                        }
                        if (state == MustacheLexicalState.Operator1 && token.Value == "!")
                        {
                            operator1 = token.Value;
                            state = MustacheLexicalState.Comment;
                            continue;
                        }
                        if (state == MustacheLexicalState.Operator1 && (token.Value == "/" || token.Value == "#" || token.Value == "^"))
                        {
                            operator1 = token.Value;
                            state = MustacheLexicalState.Operator2;
                            continue;
                        }

                        if (state == MustacheLexicalState.Variable && (token.Value == "}}" || token.Value == "}}}"))
                        {
                            if (operator1 != "/")
                            {
                                variable = operator2;
                                operator2 = null;
                            }
                            state = MustacheLexicalState.Closure;
                            // Pass through
                        }
                        if (state == MustacheLexicalState.Closure && (token.Value == "}}" || token.Value == "}}}"))
                        {
                            if (closingBracket != token.Value)
                            {
                                throw new MustacheException(null, MustacheErrorCode.MismatchedBrackets, "Mismatched brackets. Expected '" + closingBracket + "'", token.Line, token.Column);
                            }

                            if (operator1 == "#" && (operator2 == null || operator2 == "if"))
                            {
                                tokenType = MustacheTokenType.Section;
                                tokenValue = new StringBuilder(variable);
                            }

                            if (operator1 == "#" && operator2 == "unless")
                            {
                                tokenType = MustacheTokenType.InvertedSection;
                                tokenValue = new StringBuilder(variable);
                            }

                            if (operator1 == "^" && operator2 == null)
                            {
                                tokenType = MustacheTokenType.InvertedSection;
                                tokenValue = new StringBuilder(variable);
                            }

                            if (operator1 == "/")
                            {
                                tokenType = MustacheTokenType.SectionEnd;
                                tokenValue = new StringBuilder(variable);
                            }

                            if (operator1 == null)
                            {
                                tokenType = closingBracket == "}}" ? MustacheTokenType.Variable : MustacheTokenType.EscapedVariable;
                                tokenValue = new StringBuilder(variable);
                            }

                            if (tokenType == MustacheTokenType.Unknown)
                            {
                                throw new MustacheException(null, MustacheErrorCode.Internal, "Internal error", token.Line, token.Column);
                            }

                            operator1 = null;
                            operator2 = null;
                            variable = null;
                            state = MustacheLexicalState.Value;
                        }
                        break;
                    case TokenType.Word:
                        if (state == MustacheLexicalState.Operator1)
                        {
                            state = MustacheLexicalState.Variable;
                        }
                        if (state == MustacheLexicalState.Operator2 && (token.Value == "if" || token.Value == "unless"))
                        {
                            operator2 = token.Value;
                            state = MustacheLexicalState.Variable;
                            continue;
                        }
                        if (state == MustacheLexicalState.Operator2)
                        {
                            state = MustacheLexicalState.Variable;
                        }
                        if (state == MustacheLexicalState.Variable)
                        {
                            variable = token.Value;
                            state = MustacheLexicalState.Closure;
                            continue;
                        }
                        break;
                    case TokenType.Whitespace:
                        continue;
                }

                if (tokenType == MustacheTokenType.Unknown)
                    throw new MustacheException(null, MustacheErrorCode.UnexpectedSymbol, "Unexpected symbol '" + token.Value + "'", token.Line, token.Column);

                _initialTokens.Add(new MustacheToken(tokenType, tokenValue.ToString() != "" ? tokenValue.ToString() : null, token.Line, token.Column));
            }

            if (state != MustacheLexicalState.Value)
                throw new MustacheException(null, MustacheErrorCode.UnexpectedEnd, "Unexpected end of file", 0, 0);

        }

        /// <summary>
        /// Performs a syntax analysis at level 0.
        /// </summary>
        private void PerformSyntaxAnalysis()
        {
            CheckForMoreTokens();
            while (HasMoreTokens())
            {
                var token = GetCurrentToken();
                MoveToNextToken();

                if (token.Type == MustacheTokenType.SectionEnd)
                {
                    throw new MustacheException(null, MustacheErrorCode.UnexpectedSectionEnd, "Unexpected section end for variable '" + token.Value + "'", token.Line, token.Column);
                }

                var result = AddTokenToResult(token.Type, token.Value, token.Line, token.Column);

                if (token.Type == MustacheTokenType.Section || token.Type == MustacheTokenType.InvertedSection)
                {
                    ((List<MustacheToken>)result.Tokens).AddRange(PerformSyntaxAnalysisForSection(token.Value));
                }
            }

        }

        /// <summary>
        /// Performs a syntax analysis for section
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        private IList<MustacheToken> PerformSyntaxAnalysisForSection(string variable)
        {
            List<MustacheToken> result = new List<MustacheToken>();
            MustacheToken token;
            CheckForMoreTokens();
            while (HasMoreTokens())
            {
                token = GetCurrentToken();
                MoveToNextToken();

                if (token.Type == MustacheTokenType.SectionEnd && (token.Value == variable || token.Value == null))
                {
                    return result;
                }

                if (token.Type == MustacheTokenType.SectionEnd)
                {
                    throw new MustacheException(null, MustacheErrorCode.UnexpectedSectionEnd, "Unexpected section end for variable '" + variable + "'", token.Line, token.Column);
                }

                var resultToken = new MustacheToken(token.Type, token.Value, token.Line, token.Column);

                if (token.Type == MustacheTokenType.Section || token.Type == MustacheTokenType.InvertedSection)
                {
                    ((List<MustacheToken>)resultToken.Tokens).AddRange(PerformSyntaxAnalysisForSection(token.Value));
                }

                result.Add(resultToken);
            }

            token = GetCurrentToken();
            throw new MustacheException(null, MustacheErrorCode.NotClosedSection, "Not closed section for variable '" + variable + "'", token.Line, token.Column);
        }

        /// <summary>
        /// Retrieves variables from the parsed output.
        /// </summary>
        private void LookupVariables()
        {
            if (_originalTokens == null)
                return;

            _variableNames = new List<string>();
            foreach (var token in _initialTokens)
            {
                if (token.Type != MustacheTokenType.Value && token.Type != MustacheTokenType.Comment && token.Value != null)
                {
                    string variableName = token.Value.ToLower();
                    bool found = ((List<string>)_variableNames).FindAll(v => v.ToLower() == variableName).Count > 0;
                    if (!found)
                    {
                        _variableNames.Add(token.Value);
                    }
                }
            }
        }
    }
}
