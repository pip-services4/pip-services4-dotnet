using System;
using System.Collections.Generic;
using System.Text;

using PipServices4.Expressions.Calculator.Tokenizers;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Variants;

namespace PipServices4.Expressions.Calculator.Parsers
{
    /// <summary>
    /// Implements an expression parser class.
    /// </summary>
    public sealed class ExpressionParser
    {
        /// <summary>
        /// Defines a list of operators.
        /// </summary>
        private static string[] Operators = new string[28]
        {
            "(", ")", "[", "]", "+", "-", "*", "/", "%", "^",
            "=", "<>", "!=", ">", "<", ">=", "<=", "<<", ">>",
            "AND", "OR", "XOR", "NOT", "IS", "IN", "NULL", "LIKE", ","
        };

        /// <summary>
        /// Defines a list of operator token types.
        /// </summary>
        private static ExpressionTokenType[] OperatorTypes = new ExpressionTokenType[28]
        {
            ExpressionTokenType.LeftBrace, ExpressionTokenType.RightBrace,
            ExpressionTokenType.LeftSquareBrace, ExpressionTokenType.RightSquareBrace,
            ExpressionTokenType.Plus, ExpressionTokenType.Minus,
            ExpressionTokenType.Star, ExpressionTokenType.Slash,
            ExpressionTokenType.Procent, ExpressionTokenType.Power,
            ExpressionTokenType.Equal, ExpressionTokenType.NotEqual,
            ExpressionTokenType.NotEqual, ExpressionTokenType.More,
            ExpressionTokenType.Less, ExpressionTokenType.EqualMore,
            ExpressionTokenType.EqualLess, ExpressionTokenType.ShiftLeft,
            ExpressionTokenType.ShiftRight, ExpressionTokenType.And,
            ExpressionTokenType.Or, ExpressionTokenType.Xor,
            ExpressionTokenType.Not, ExpressionTokenType.Is,
            ExpressionTokenType.In, ExpressionTokenType.Null,
            ExpressionTokenType.Like, ExpressionTokenType.Comma
        };

        private ITokenizer _tokenizer = new ExpressionTokenizer();
        private string _expression = "";
        private IList<Token> _originalTokens = new List<Token>();
        private IList<ExpressionToken> _initialTokens = new List<ExpressionToken>();
        private int _currentTokenIndex;
        private IList<string> _variableNames = new List<string>();
        private IList<ExpressionToken> _resultTokens = new List<ExpressionToken>();

        /// <summary>
        /// The expression string.
        /// </summary>
        public string Expression
        {
            get { return _expression; }
            set { ParseString(value); }
        }

        public IList<Token> OriginalTokens
        {
            get { return _originalTokens; }
            set { ParseTokens(value); }
        }

        /// <summary>
        /// The list of original expression tokens.
        /// </summary>
        public IList<ExpressionToken> InitialTokens
        {
            get { return _initialTokens; }
        }

        /// <summary>
        /// The list of parsed expression tokens.
        /// </summary>
        public IList<ExpressionToken> ResultTokens
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

        /// <summary>
        /// Sets a new expression string and parses it into internal byte code.
        /// </summary>
        /// <param name="expression">A new expression string.</param>
        public void ParseString(string expression)
        {
            Clear();
            _expression = expression != null ? expression.Trim() : "";
            _originalTokens = TokenizeExpression(_expression);
            PerformParsing();
        }

        public void ParseTokens(IList<Token> tokens)
        {
            Clear();
            _originalTokens = tokens;
            _expression = ComposeExpression(tokens);
            PerformParsing();
        }

        /// <summary>
        /// Clears parsing results.
        /// </summary>
        public void Clear()
        {
            _expression = null;
            _originalTokens = new List<Token>();
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
            {
                throw new SyntaxException(null, SyntaxErrorCode.UnexpectedEnd, "Unexpected end of expression.");
            }
        }

        /// <summary>
        /// Gets the current token object.
        /// </summary>
        /// <returns>The current token object.</returns>
        private ExpressionToken GetCurrentToken()
        {
            return _currentTokenIndex < _initialTokens.Count ? _initialTokens[_currentTokenIndex] : null;
        }

        /// <summary>
        /// Gets the next token object.
        /// </summary>
        /// <returns>The next token object.</returns>
        private ExpressionToken GetNextToken()
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
        /// Adds an expression to the result list
        /// </summary>
        /// <param name="type">The type of the token to be added.</param>
        /// <param name="value">The value of the token to be added.</param>
        /// <param name="line"> The line number where the token is.</param>
        /// <param name="column">The column number where the token is.</param>
        private void AddTokenToResult(ExpressionTokenType type, Variant value, int line, int column)
        {
            _resultTokens.Add(new ExpressionToken(type, value, line, column));
        }

        /// <summary>
        /// Matches available tokens types with types from the list.
        /// If tokens matchs then shift the list.
        /// </summary>
        /// <param name="types">A list of token types to compare.</param>
        /// <returns><code>true</code> if token types match.</returns>
        private bool MatchTokensWithTypes(params ExpressionTokenType[] types)
        {
            bool matches = false;
            for (int i = 0; i < types.Length; i++)
            {
                if (_currentTokenIndex + i < _initialTokens.Count)
                {
                    matches = _initialTokens[_currentTokenIndex + i].Type == types[i];
                }
                else
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                _currentTokenIndex += types.Length;
            }
            return matches;
        }

        private IList<Token> TokenizeExpression(string expression)
        {
            expression = expression != null ? expression.Trim() : "";
            if (expression.Length > 0)
            {
                _tokenizer.SkipWhitespaces = true;
                _tokenizer.SkipComments = true;
                _tokenizer.SkipEof = true;
                _tokenizer.DecodeStrings = true;
                return _tokenizer.TokenizeBuffer(expression);
            }
            else
            {
                return new List<Token>();
            }
        }

        private string ComposeExpression(IList<Token> tokens)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Token token in tokens)
            {
                builder.Append(token.Value);
            }
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
                    ExpressionToken token = GetCurrentToken();
                    throw new SyntaxException(null, SyntaxErrorCode.ErrorNear, string.Format("Syntax error near {0}", token.Value));
                }
            }
        }

        /// <summary>
        /// Tokenizes the given expression and prepares an initial tokens list.
        /// </summary>
        private void CompleteLexicalAnalysis()
        {
            foreach (Token token in _originalTokens)
            {
                ExpressionTokenType tokenType = ExpressionTokenType.Unknown;
                Variant tokenValue = Variant.Empty;

                switch (token.Type)
                {
                    case TokenType.Comment:
                    case TokenType.Whitespace:
                        continue;
                    case TokenType.Keyword:
                        {
                            string temp = token.Value.ToUpper();
                            if (temp.Equals("TRUE"))
                            {
                                tokenType = ExpressionTokenType.Constant;
                                tokenValue = Variant.FromBoolean(true);
                            }
                            else if (temp.Equals("FALSE"))
                            {
                                tokenType = ExpressionTokenType.Constant;
                                tokenValue = Variant.FromBoolean(false);
                            }
                            else
                            {
                                for (int index = 0; index < Operators.Length; index++)
                                {
                                    if (temp.Equals(Operators[index]))
                                    {
                                        tokenType = OperatorTypes[index];
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    case TokenType.Word:
                        {
                            tokenType = ExpressionTokenType.Variable;
                            tokenValue = new Variant(token.Value);
                            break;
                        }
                    case TokenType.Integer:
                        {
                            tokenType = ExpressionTokenType.Constant;
                            tokenValue = new Variant(Int32.Parse(token.Value));
                            break;
                        }
                    case TokenType.Float:
                        {
                            tokenType = ExpressionTokenType.Constant;
                            tokenValue = new Variant(Single.Parse(token.Value));
                            break;
                        }
                    case TokenType.Quoted:
                        {
                            tokenType = ExpressionTokenType.Constant;
                            tokenValue = new Variant(token.Value);
                            break;
                        }
                    case TokenType.Symbol:
                        {
                            string temp = token.Value.ToUpper();
                            for (int i = 0; i < Operators.Length; i++)
                            {
                                if (temp.Equals(Operators[i]))
                                {
                                    tokenType = OperatorTypes[i];
                                    break;
                                }
                            }
                            break;
                        }
                }
                if (tokenType == ExpressionTokenType.Unknown)
                {
                    throw new SyntaxException(null, SyntaxErrorCode.UnknownSymbol, string.Format("Unknown symbol {0}.", token.Value), token.Line, token.Column);
                }
                _initialTokens.Add(new ExpressionToken(tokenType, tokenValue, token.Line, token.Column));
            }
        }

        /// <summary>
        /// Performs a syntax analysis at level 0.
        /// </summary>
        private void PerformSyntaxAnalysis()
        {
            CheckForMoreTokens();
            PerformSyntaxAnalysisAtLevel1();
            while (HasMoreTokens())
            {
                ExpressionToken token = GetCurrentToken();
                if (token.Type == ExpressionTokenType.And
                    || token.Type == ExpressionTokenType.Or
                    || token.Type == ExpressionTokenType.Xor)
                {
                    MoveToNextToken();
                    PerformSyntaxAnalysisAtLevel1();
                    AddTokenToResult(token.Type, Variant.Empty, token.Line, token.Column);
                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// Performs a syntax analysis at level 1.
        /// </summary>
        private void PerformSyntaxAnalysisAtLevel1()
        {
            CheckForMoreTokens();
            ExpressionToken token = GetCurrentToken();
            if (token.Type == ExpressionTokenType.Not)
            {
                MoveToNextToken();
                PerformSyntaxAnalysisAtLevel2();
                AddTokenToResult(token.Type, Variant.Empty, token.Line, token.Column);
            }
            else
            {
                PerformSyntaxAnalysisAtLevel2();
            }
        }

        /// <summary>
        /// Performs a syntax analysis at level 2.
        /// </summary>
        private void PerformSyntaxAnalysisAtLevel2()
        {
            CheckForMoreTokens();
            PerformSyntaxAnalysisAtLevel3();
            while (HasMoreTokens())
            {
                ExpressionToken token = GetCurrentToken();
                if (token.Type == ExpressionTokenType.Equal
                    || token.Type == ExpressionTokenType.NotEqual
                    || token.Type == ExpressionTokenType.More
                    || token.Type == ExpressionTokenType.Less
                    || token.Type == ExpressionTokenType.EqualMore
                    || token.Type == ExpressionTokenType.EqualLess)
                {
                    MoveToNextToken();
                    PerformSyntaxAnalysisAtLevel3();
                    AddTokenToResult(token.Type, Variant.Empty, token.Line, token.Column);
                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// Performs a syntax analysis at level 3.
        /// </summary>
        private void PerformSyntaxAnalysisAtLevel3()
        {
            CheckForMoreTokens();
            PerformSyntaxAnalysisAtLevel4();
            while (HasMoreTokens())
            {
                ExpressionToken token = GetCurrentToken();
                if (token.Type == ExpressionTokenType.Plus
                    || token.Type == ExpressionTokenType.Minus
                    || token.Type == ExpressionTokenType.Like)
                {
                    MoveToNextToken();
                    PerformSyntaxAnalysisAtLevel4();
                    AddTokenToResult(token.Type, Variant.Empty, token.Line, token.Column);
                }
                else if (MatchTokensWithTypes(ExpressionTokenType.Not, ExpressionTokenType.Like))
                {
                    PerformSyntaxAnalysisAtLevel4();
                    AddTokenToResult(ExpressionTokenType.NotLike, Variant.Empty, token.Line, token.Column);
                }
                else if (MatchTokensWithTypes(ExpressionTokenType.Is, ExpressionTokenType.Null))
                {
                    AddTokenToResult(ExpressionTokenType.IsNull, Variant.Empty, token.Line, token.Column);
                }
                else if (MatchTokensWithTypes(ExpressionTokenType.Is, ExpressionTokenType.Not,
                    ExpressionTokenType.Null))
                {
                    AddTokenToResult(ExpressionTokenType.IsNotNull, Variant.Empty, token.Line, token.Column);
                }
                else if (MatchTokensWithTypes(ExpressionTokenType.Not, ExpressionTokenType.In))
                {
                    PerformSyntaxAnalysisAtLevel4();
                    AddTokenToResult(ExpressionTokenType.NotIn, Variant.Empty, token.Line, token.Column);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Performs a syntax analysis at level 4.
        /// </summary>
        private void PerformSyntaxAnalysisAtLevel4()
        {
            CheckForMoreTokens();
            PerformSyntaxAnalysisAtLevel5();
            while (HasMoreTokens())
            {
                ExpressionToken token = GetCurrentToken();
                if (token.Type == ExpressionTokenType.Star
                    || token.Type == ExpressionTokenType.Slash
                    || token.Type == ExpressionTokenType.Procent)
                {
                    MoveToNextToken();
                    PerformSyntaxAnalysisAtLevel5();
                    AddTokenToResult(token.Type, Variant.Empty, token.Line, token.Column);
                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// Performs a syntax analysis at level 5.
        /// </summary>
        private void PerformSyntaxAnalysisAtLevel5()
        {
            CheckForMoreTokens();
            PerformSyntaxAnalysisAtLevel6();
            while (HasMoreTokens())
            {
                ExpressionToken token = GetCurrentToken();
                if (token.Type == ExpressionTokenType.Power
                    || token.Type == ExpressionTokenType.In
                    || token.Type == ExpressionTokenType.ShiftLeft
                    || token.Type == ExpressionTokenType.ShiftRight)
                {
                    MoveToNextToken();
                    PerformSyntaxAnalysisAtLevel6();
                    AddTokenToResult(token.Type, Variant.Empty, token.Line, token.Column);
                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// Performs a syntax analysis at level 6.
        /// </summary>
        private void PerformSyntaxAnalysisAtLevel6()
        {
            CheckForMoreTokens();
            // Process unary '+' or '-'.
            ExpressionToken unaryToken = GetCurrentToken();
            if (unaryToken.Type == ExpressionTokenType.Plus)
            {
                unaryToken = null;
                MoveToNextToken();
            }
            else if (unaryToken.Type == ExpressionTokenType.Minus)
            {
                unaryToken = new ExpressionToken(ExpressionTokenType.Unary, unaryToken.Value, unaryToken.Line, unaryToken.Column);
                MoveToNextToken();
            }
            else
            {
                unaryToken = null;
            }

            CheckForMoreTokens();

            // Identify function calls.
            ExpressionToken primitiveToken = GetCurrentToken();
            ExpressionToken nextToken = GetNextToken();
            if (primitiveToken.Type == ExpressionTokenType.Variable
                && nextToken != null && nextToken.Type == ExpressionTokenType.LeftBrace)
            {
                primitiveToken = new ExpressionToken(ExpressionTokenType.Function, primitiveToken.Value, primitiveToken.Line, primitiveToken.Column);
            }

            if (primitiveToken.Type == ExpressionTokenType.Constant)
            {
                MoveToNextToken();
                AddTokenToResult(primitiveToken.Type, primitiveToken.Value, primitiveToken.Line, primitiveToken.Column);
            }
            else if (primitiveToken.Type == ExpressionTokenType.Variable)
            {
                MoveToNextToken();

                string temp = primitiveToken.Value.AsString;
                if (_variableNames.IndexOf(temp) < 0)
                {
                    _variableNames.Add(temp);
                }

                AddTokenToResult(primitiveToken.Type, primitiveToken.Value, primitiveToken.Line, primitiveToken.Column);
            }
            else if (primitiveToken.Type == ExpressionTokenType.LeftBrace)
            {
                MoveToNextToken();
                PerformSyntaxAnalysis();
                CheckForMoreTokens();
                primitiveToken = GetCurrentToken();
                if (primitiveToken.Type != ExpressionTokenType.RightBrace)
                {
                    throw new SyntaxException(null, SyntaxErrorCode.MissedCloseParenthesis, "Expected ')' was not found", primitiveToken.Line, primitiveToken.Column);
                }
                MoveToNextToken();
            }
            else if (primitiveToken.Type == ExpressionTokenType.Function)
            {
                MoveToNextToken();
                ExpressionToken token = GetCurrentToken();
                if (token.Type != ExpressionTokenType.LeftBrace)
                {
                    throw new SyntaxException(null, SyntaxErrorCode.Internal, "Internal error.", token.Line, token.Column);
                }
                int paramCount = 0;
                do
                {
                    MoveToNextToken();
                    token = GetCurrentToken();
                    if (token == null || token.Type == ExpressionTokenType.RightBrace)
                    {
                        break;
                    }
                    paramCount++;
                    PerformSyntaxAnalysis();
                    token = GetCurrentToken();
                }
                while (token != null && token.Type == ExpressionTokenType.Comma);

                CheckForMoreTokens();

                if (token.Type != ExpressionTokenType.RightBrace)
                {
                    throw new SyntaxException(null, SyntaxErrorCode.MissedCloseParenthesis, "Expected ')' was not found.");
                }
                MoveToNextToken();

                AddTokenToResult(ExpressionTokenType.Constant, new Variant(paramCount), primitiveToken.Line, primitiveToken.Column);
                AddTokenToResult(primitiveToken.Type, primitiveToken.Value, primitiveToken.Line, primitiveToken.Column);
            }
            else
            {
                throw new SyntaxException(null, SyntaxErrorCode.ErrorAt, string.Format("Syntax error at {0}", primitiveToken.Value), primitiveToken.Line, primitiveToken.Column);
            }

            if (unaryToken != null)
            {
                AddTokenToResult(unaryToken.Type, Variant.Empty, unaryToken.Line, unaryToken.Column);
            }

            // Process [] operator.
            if (HasMoreTokens())
            {
                primitiveToken = GetCurrentToken();
                if (primitiveToken.Type == ExpressionTokenType.LeftSquareBrace)
                {
                    MoveToNextToken();
                    PerformSyntaxAnalysis();
                    CheckForMoreTokens();
                    primitiveToken = GetCurrentToken();
                    if (primitiveToken.Type != ExpressionTokenType.RightSquareBrace)
                    {
                        throw new SyntaxException(null, SyntaxErrorCode.MissedCloseSquareBracket, "Expected ']' was not found", primitiveToken.Line, primitiveToken.Column);
                    }
                    MoveToNextToken();
                    AddTokenToResult(ExpressionTokenType.Element, Variant.Empty, primitiveToken.Line, primitiveToken.Column);
                }
            }
        }
    }
}