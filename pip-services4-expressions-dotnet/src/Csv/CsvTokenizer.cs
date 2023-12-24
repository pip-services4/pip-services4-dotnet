using System;
using PipServices4.Expressions.Tokenizers;

namespace PipServices4.Expressions.Csv
{
    /// <summary>
    /// Implements a tokenizer class for CSV files.
    /// </summary>
    public class CsvTokenizer : AbstractTokenizer
    {
        private char[] _fieldSeparators = new char[] { ',' };
        private char[] _quoteSymbols = new char[] { '"' };
        private string _endOfLine = new string(new char[] { CsvConstant.CR, CsvConstant.LF });

        /// <summary>Separator for fields in CSV stream.</summary>
        public char[] FieldSeparators
        {
            get { return _fieldSeparators; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                foreach (char fieldSeparator in value)
                {
                    if (fieldSeparator == CsvConstant.CR
                        || fieldSeparator == CsvConstant.LF
                        || fieldSeparator == CsvConstant.Nil)
                    {
                        throw new ArgumentException("Invalid field separator.");
                    }
                    foreach (char quoteSymbol in QuoteSymbols)
                    {
                        if (fieldSeparator == quoteSymbol)
                        {
                            throw new ArgumentException("Invalid field separator.");
                        }
                    }
                }

                _fieldSeparators = value;
                WordState = new CsvWordState(value, QuoteSymbols);
                AssignStates();
            }
        }

        /// <summary>Separator for rows in CSV stream.</summary>
        public string EndOfLine
        {
            get { return _endOfLine; }
            set
            {
                _endOfLine = value;
            }
        }

        /// <summary>Character to quote strings.</summary>
        public char[] QuoteSymbols
        {
            get { return _quoteSymbols; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                foreach (char quoteSymbol in value)
                {
                    if (quoteSymbol == CsvConstant.CR
                        || quoteSymbol == CsvConstant.LF
                        || quoteSymbol == CsvConstant.Nil)
                    {
                        throw new ArgumentException("Invalid quote symbol.");
                    }
                    foreach (char fieldSeparator in FieldSeparators)
                    {
                        if (quoteSymbol == fieldSeparator)
                        {
                            throw new ArgumentException("Invalid quote symbol.");
                        }
                    }
                }

                _quoteSymbols = value;
                WordState = new CsvWordState(FieldSeparators, value);
                AssignStates();
            }
        }

        /// <summary>
        /// Assigns tokenizer states to correct characters.
        /// </summary>
        private void AssignStates()
        {
            ClearCharacterStates();
            SetCharacterState('\x0000', '\xffff', WordState);
            SetCharacterState(CsvConstant.CR, CsvConstant.CR, SymbolState);
            SetCharacterState(CsvConstant.LF, CsvConstant.LF, SymbolState);
            foreach (char fieldSeparator in FieldSeparators)
            {
                SetCharacterState(fieldSeparator, fieldSeparator, SymbolState);
            }
            foreach (char quoteSymbol in QuoteSymbols)
            {
                SetCharacterState(quoteSymbol, quoteSymbol, QuoteState);
            }
        }

        /// <summary>
        /// Constructs this object with default parameters.
        /// </summary>
        public CsvTokenizer()
        {
            NumberState = null;
            WhitespaceState = null;
            CommentState = null;
            WordState = new CsvWordState(FieldSeparators, QuoteSymbols);
            SymbolState = new CsvSymbolState();
            QuoteState = new CsvQuoteState();
            AssignStates();
        }
    }
}
