using PipServices4.Expressions.Variants;

namespace PipServices4.Expressions.Calculator.Parsers
{
    /// <summary>
    /// Defines an expression token holder.
    /// </summary>
    public class ExpressionToken
    {
        private ExpressionTokenType _type;
        private Variant _value;
        private int _line;
        private int _column;

        /// <summary>
        /// Creates an instance of this token and initializes it with specified values.
        /// </summary>
        /// <param name="type">The type of this token.</param>
        /// <param name="value">The value of this token.</param>
        /// <param name="line">The value of this token.</param>
        /// <param name="column">The value of this token.</param>
        public ExpressionToken(ExpressionTokenType type, Variant value, int line, int column)
        {
            _type = type;
            _value = value;
            _line = line;
            _column = column;
        }

        /// <summary>
        /// Creates an instance of this class with specified type and Null value.
        /// </summary>
        /// <param name="type">The type of this token.</param>
        public ExpressionToken(ExpressionTokenType type)
        {
            _type = type;
            _value = Variant.Empty;
        }

        /// <summary>
        /// The type of this token.
        /// </summary>
        public ExpressionTokenType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// The value of this token.
        /// </summary>
        public Variant Value
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
    }
}
