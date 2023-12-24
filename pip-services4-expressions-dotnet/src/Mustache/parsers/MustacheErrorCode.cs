using System;
using System.Collections.Generic;
using System.Text;

namespace PipServices4.Expressions.Mustache.Parsers
{
    /// <summary>
    /// General syntax errors.
    /// </summary>
    public static class MustacheErrorCode
    {
        /// <summary>
        /// The unknown
        /// </summary>
        public const string Unknown = "UNKNOWN";

        /// <summary>
        /// The internal error
        /// </summary>
        public const string Internal = "INTERNAL";

        /// <summary>
        /// The unexpected end.
        /// </summary>
        public const string UnexpectedEnd = "UNEXPECTED_END";

        /// <summary>
        /// The error near
        /// </summary>
        public const string ErrorNear = "ERROR_NEAR";

        /// <summary>
        /// The error at
        /// </summary>
        public const string ErrorAt = "ERROR_AT";

        /// <summary>
        /// The unexpected symbol
        /// </summary>
        public const string UnexpectedSymbol = "UNEXPECTED_SYMBOL";

        /// <summary>
        /// The mismatched brackets
        /// </summary>
        public const string MismatchedBrackets = "MISTMATCHED_BRACKETS";

        /// <summary>
        /// The missing variable
        /// </summary>
        public const string MissingVariable = "MISSING_VARIABLE";

        /// <summary>
        /// Not closed section
        /// </summary>
        public const string NotClosedSection = "NOT_CLOSED_SECTION";

        /// <summary>
        /// Unexpected section end
        /// </summary>
        public const string UnexpectedSectionEnd = "UNEXPECTED_SECTION_END";
    }
}
