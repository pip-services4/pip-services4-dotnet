using System;

namespace PipServices4.Expressions.Tokenizers.Utilities
{
    /// <summary>
    /// Validates characters that are processed by Tokenizers.
    /// </summary>
    public sealed class CharValidator
    {
        public const char Eof = '\xffff';

        /// <summary>
        /// Default contructor to prevent creation of a class instance.
        /// </summary>
        private CharValidator()
        {
        }

        public static bool IsEof(char value)
        {
            return value == Eof;
        }

        public static bool IsEol(char value)
        {
            return value == '\n' || value == '\r';
        }

        public static bool IsDigit(char value)
        {
            return value >= '0' && value <= '9';
        }
    }
}
