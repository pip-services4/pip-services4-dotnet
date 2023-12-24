using System;
using System.Collections.Generic;
using System.Text;

namespace PipServices4.Expressions.Mustache.Parsers
{
    /// <summary>
    /// Defines a mustache token holder.
    /// </summary>
    public class MustacheToken
    {
        private MustacheTokenType _type;
        private string _value;
        private IList<MustacheToken> _tokens = new List<MustacheToken>();
        private int _line;
        private int _column;

        /// <summary>
        /// Creates an instance of a mustache token.
        /// </summary>
        /// <param name="type">a token type.</param>
        /// <param name="value">a token value.</param>
        /// <param name="line">a line number where the token is.</param>
        /// <param name="column">a column numer where the token is.</param>
        public MustacheToken(MustacheTokenType type, string value, int line, int column)
        {
            _type = type;
            _value = value;
            _line = line;
            _column = column;
        }

        /// <summary>
        /// Gets the token type.
        /// </summary>
        public MustacheTokenType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the token value or variable name.
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets a list of subtokens is this token a section.
        /// </summary>
        public IList<MustacheToken> Tokens
        {
            get { return _tokens; }
        }

        /// <summary>
        /// The line number where the token is.
        /// </summary>
        public int Line
        {
            get { return _line; }
        }

        /// <summary>
        /// The column number where the token is.
        /// </summary>
        public int Column
        {
            get { return _column; }
        }
    }
}
