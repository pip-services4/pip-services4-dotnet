using System;

using PipServices4.Expressions.Tokenizers.Generic;

namespace PipServices4.Expressions.Csv
{
    /// <summary>
    /// Implements a word state to tokenize CSV stream.
    /// </summary>
    public class CsvWordState : GenericWordState
    {
        /// <summary>
        /// Constructs this object with specified parameters.
        /// </summary>
        /// <param name="fieldSeparators">Separators for fields in CSV stream.</param>
        /// <param name="quoteSymbol">Delimiters character to quote strings.</param>
        public CsvWordState(char[] fieldSeparators, char[] quoteSymbols)
        {
            ClearWordChars();
            SetWordChars('\x0000', '\xffff', true);

            SetWordChars(CsvConstant.CR, CsvConstant.CR, false);
            SetWordChars(CsvConstant.LF, CsvConstant.LF, false);

            foreach (char fieldSeparator in fieldSeparators)
            {
                SetWordChars(fieldSeparator, fieldSeparator, false);
            }

            foreach (char quoteSymbol in quoteSymbols)
            {
                SetWordChars(quoteSymbol, quoteSymbol, false);
            }
        }

    }
}
