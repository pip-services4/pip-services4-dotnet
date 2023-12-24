using System;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// A token represents a logical chunk of a string. For example, a typical tokenizer would break
    /// the string "1.23 &lt;= 12.3" into three tokens: the number 1.23, a less-than-or-equal symbol,
    /// and the number 12.3. A token is a receptacle, and relies on a tokenizer to decide precisely how
    /// to divide a string into tokens.
    /// </summary>
    public class Token
    {
        private TokenType _type;
        private string _value;
        private int _line;
        private int _column;

        /// <summary>
        /// Constructs this token with type and value.
        /// </summary>
        /// <param name="type">The type of this token.</param>
        /// <param name="value">The token string value.</param>
        /// <param name="line`">The line number where the token is.</param>
        /// <param name="column">The column number where the token is.</param>
        public Token(TokenType type, string value, int line, int column)
        {
            _type = type;
            _value = value;
            _line = line;
            _column = column;
        }

        /// <summary>
        /// The token type.
        /// </summary>
        public TokenType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// The token value.
        /// </summary>
        public string Value
        {
            get { return _value; }
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

        public override bool Equals(object obj)
        {
            if (obj is Token)
            {
                Token token = (Token)obj;
                return token._type == _type && token._value.Equals(_value);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

