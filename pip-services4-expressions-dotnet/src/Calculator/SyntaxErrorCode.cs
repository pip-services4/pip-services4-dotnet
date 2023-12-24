using System;

namespace PipServices4.Expressions.Calculator
{
    /// <summary>
    /// General syntax errors.
    /// </summary>
    public static class SyntaxErrorCode
    {
        /// <summary>
        /// The unknown
        /// </summary>
        public static string Unknown = "UNKNOWN";

        /// <summary>
        /// The internal error
        /// </summary>
        public static string Internal = "INTERNAL";

        /// <summary>
        /// The unexpected end.
        /// </summary>
        public static string UnexpectedEnd = "UNEXPECTED_END";

        /// <summary>
        /// The error near
        /// </summary>
        public static string ErrorNear = "ERROR_NEAR";

        /// <summary>
        /// The error at
        /// </summary>
        public static string ErrorAt = "ERROR_AT";

        /// <summary>
        /// The unknown symbol
        /// </summary>
        public static string UnknownSymbol = "UNKNOWN_SYMBOL";

        /// <summary>
        /// The missed close parenthesis
        /// </summary>
        public static string MissedCloseParenthesis = "MISSED_CLOSE_PARENTHESIS";

        /// <summary>
        /// The missed close square bracket
        /// </summary>
        public static string MissedCloseSquareBracket = "MISSED_CLOSE_SQUARE_BRACKET";
    }
}
