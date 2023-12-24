using System;
using System.IO;
using System.Text;

namespace PipServices4.Expressions.IO
{
    /// <summary>
    /// Implements a reader of CSV stream
    /// </summary>
    public sealed class CsvReader
    {
        private char[] _fieldSeparators;
        private char[] _quoteSymbols;
        private string _endOfLine;
        private TextReader _reader;
        private string _buffer;
        private int _bufferPosition;
        private char _currentChar;
        private char _nextChar;

        /// <summary>
        /// Constructs this object with text reader.
        /// </summary>
        /// <param name="reader">A text reader to read the CSV data.</param>
        /// <param name="fieldSeparators">Separators for fields in CSV stream.</param>
        /// <param name="quoteSymbols">Characters to quote strings.</param>
        public CsvReader(TextReader reader, char[] fieldSeparators, char[] quoteSymbols, string endOfLine)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            _reader = reader;

            _fieldSeparators = fieldSeparators;
            _quoteSymbols = quoteSymbols;
            _endOfLine = endOfLine;

            ReadNextChar();
        }

        /// <summary>
        /// Constructs this object with string buffer.
        /// </summary>
        /// <param name="buffer">A string buffer that contains CSV data.</param>
        /// <param name="fieldSeparators">Separator for fields in CSV stream.</param>
        /// <param name="quoteSymbols">Caracter to quote strings.</param>
        public CsvReader(TextReader reader, char[] fieldSeparators, char[] quoteSymbols)
            : this(reader, fieldSeparators, quoteSymbols, "\r\n")
        {
        }

        /// <summary>
        /// Constructs this object with string buffer.
        /// </summary>
        /// <param name="buffer">A string buffer that contains CSV data.</param>
        /// <param name="fieldSeparators">Separator for fields in CSV stream.</param>
        /// <param name="quoteSymbols">Character to quote strings.</param>
        public CsvReader(string buffer, char[] fieldSeparators, char[] quoteSymbols)
            : this(buffer, fieldSeparators, quoteSymbols, "\r\n")
        {
        }

        /// <summary>
        /// Constructs this object with string buffer.
        /// </summary>
        /// <param name="buffer">A string buffer that contains CSV data.</param>
        /// <param name="fieldSeparators">Separators for fields in CSV stream.</param>
        /// <param name="quoteSymbols">Characters to quote strings.</param>
        public CsvReader(string buffer, char[] fieldSeparators, char[] quoteSymbols, string endOfLine)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            _buffer = buffer;

            _fieldSeparators = fieldSeparators;
            _quoteSymbols = quoteSymbols;
            _endOfLine = endOfLine;

            ReadNextChar();
        }

        /// <summary>
        /// Separators for fields in CSV stream.
        /// Default is comma (,).
        /// </summary>
        public char[] FieldSeparators
        {
            get { return _fieldSeparators; }
        }

        /// <summary>
        /// Characters to quote strings in CSV streams.
        /// Default is double quote (").
        /// </summary>
        public char[] QuoteSymbols
        {
            get { return _quoteSymbols; }
        }

        /// <summary>
        /// Separator for rows in CSV stream.
        /// Default is "\r\n".
        /// </summary>
        public string EndOfLine
        {
            get { return _endOfLine; }
        }

        /// <summary>
        /// Flag that shows end of CSV stream.
        /// </summary>
        public bool Eof
        {
            get { return IsCurrentCharEof(); }
        }

        /// <summary>
        /// Flag that shows enf of line in CSV stream;
        /// </summary>
        public bool Eol
        {
            get { return IsCurrentCharEof() || IsCurrentCharEol(); }
        }

        /// <summary>
        /// Reads the next field from the CSV stream and sets EOL and OEF flags.
        /// It returns <code>String.Empty</code> for null string or in a case of EOF.
        /// </summary>
        /// <returns></returns>
        public string ReadField()
        {
            // Skip field separator from the previos field.
            if (IsCurrentCharFieldSeparator())
            {
                ReadNextChar();
            }
            // Skip EOL characters
            else
            {
                while (IsCurrentCharEol())
                {
                    ReadNextChar();
                }
            }
            // Check for end of file.
            if (IsCurrentCharEof())
            {
                return null;
            }

            // Process field characters in one or few strings.
            StringBuilder result = new StringBuilder();
            bool stringQuoted = false;
            bool wasQuoted = false;
            char quoteSymbol = '\xffff';

            while (!IsCurrentCharEof())
            {
                if (stringQuoted)
                {
                    if (CurrentChar == quoteSymbol)
                    {
                        if (NextChar == quoteSymbol)
                        {
                            ReadNextChar();
                            result.Append(CurrentChar);
                            ReadNextChar();
                        }
                        else
                        {
                            stringQuoted = false;
                            quoteSymbol = '\xffff';
                            ReadNextChar();
                        }
                    }
                    else
                    {
                        result.Append(CurrentChar);
                        ReadNextChar();
                    }
                }
                else
                {
                    if (IsCurrentCharEol() || IsCurrentCharFieldSeparator())
                    {
                        break;
                    }
                    else if (IsCurrentCharQuoteSymbol())
                    {
                        stringQuoted = true;
                        wasQuoted = true;
                        quoteSymbol = CurrentChar;
                        ReadNextChar();
                    }
                    else
                    {
                        result.Append(CurrentChar);
                        ReadNextChar();
                    }
                }
            }
            return wasQuoted || result.Length > 0 ? result.ToString() : null;
        }

        public void SkipLine()
        {
            while (!IsCurrentCharEol() && !IsCurrentCharEof())
            {
                ReadNextChar();
            }
        }

        private void ReadNextChar()
        {
            if (_reader != null)
            {
                _currentChar = (char)_reader.Read();
            }
            else
            {
                _currentChar = _bufferPosition < _buffer.Length
                    ? _buffer[_bufferPosition++] : '\xffff';
            }
            _nextChar = '\xffff';
        }

        private char CurrentChar
        {
            get { return _currentChar; }
        }

        private char NextChar
        {
            get
            {
                if (_nextChar == '\xffff')
                {
                    if (_reader != null)
                    {
                        _nextChar = (char)_reader.Peek();
                    }
                    else
                    {
                        _nextChar = _bufferPosition < _buffer.Length
                            ? _buffer[_bufferPosition] : '\xffff';
                    }
                }
                return _nextChar;
            }
        }

        private bool IsCurrentCharEof()
        {
            return _currentChar == '\xffff';
        }

        private bool IsCurrentCharEol()
        {
            return _currentChar == '\n' || _currentChar == '\r';
        }

        private bool IsCurrentCharFieldSeparator()
        {
            return IsSymbolIncluded(_currentChar, _fieldSeparators);
        }

        private bool IsCurrentCharQuoteSymbol()
        {
            return IsSymbolIncluded(_currentChar, _quoteSymbols);
        }

        private static bool IsSymbolIncluded(char checkSymbol, char[] symbols)
        {
            if (symbols.Length == 1)
            {
                return symbols[0] == checkSymbol;
            }
            else if (symbols.Length == 2)
            {
                return symbols[0] == checkSymbol
                    || symbols[1] == checkSymbol;
            }
            else if (symbols.Length == 0)
            {
                return false;
            }
            else
            {
                foreach (char symbol in symbols)
                {
                    if (symbol == checkSymbol)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

    }
}
