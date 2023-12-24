using System;
using System.IO;
using System.Text;

namespace PipServices4.Expressions.IO
{
    /// <summary>
    /// Implements a writer of CSV stream
    /// </summary>
    public sealed class CsvWriter
    {
        private TextWriter _writer;
        private bool _lastWasField;
        private char _fieldSeparator;
        private char _quoteSymbol;

        /// <summary>
        /// Constructs this object with text writer.
        /// </summary>
        /// <param name="writer">A text writer to writer the CSV data.</param>
        /// <param name="fieldSeparator">A separator for fields in CSV stream.</param>
        /// <param name="quoteSymbol">A character to quote strings.</param>
        public CsvWriter(TextWriter writer, char fieldSeparator, char quoteSymbol)
        {
            _writer = writer;
            _fieldSeparator = fieldSeparator;
            _quoteSymbol = quoteSymbol;
        }

        /// <summary>
        /// Separator for fields in CSV stream.
        /// Default is comma (,).
        /// </summary>
        public char FieldSeparator
        {
            get { return _fieldSeparator; }
        }

        /// <summary>
        /// Character to quote strings in CSV streams.
        /// Default is double quote (").
        /// </summary>
        public char QuoteChar
        {
            get { return _quoteSymbol; }
        }

        /// <summary>
        /// Writes a line terminator to CSV stream.
        /// </summary>
        public void WriteLine()
        {
            _writer.WriteLine();
            _lastWasField = false;
        }

        /// <summary>
        /// Writer a field value to CSV stream.
        /// </summary>
        /// <param name="fieldValue">A field value to be written.</param>
        public void WriteField(string fieldValue)
        {
            if (_lastWasField)
            {
                _writer.Write(_fieldSeparator);
            }
            _lastWasField = true;
            if (fieldValue != null)
            {
                if (fieldValue.IndexOf(_fieldSeparator) >= 0
                    || fieldValue.IndexOf(_quoteSymbol) >= 0
                    || fieldValue.IndexOf('\n') >= 0
                    || fieldValue.IndexOf('\r') >= 0
                    || fieldValue == "")
                {
                    fieldValue = EncodeString(fieldValue, _quoteSymbol);
                }
                _writer.Write(fieldValue);
            }
        }

        /// <summary>
        /// Encodes a string value.
        /// </summary>
        /// <param name="value">A string value to be encoded.</param>
        /// <param name="quoteSymbol">A string quote character.</param>
        /// <returns>An encoded string.</returns>
        public string EncodeString(string value, char quoteSymbol)
        {
            if (value != null)
            {
                StringBuilder result = new StringBuilder();
                string quoteString = Char.ToString(quoteSymbol);
                result.Append(quoteSymbol);
                result.Append(value.Replace(quoteString, quoteString + quoteString));
                result.Append(quoteSymbol);
                return result.ToString();
            }
            else
            {
                return null;
            }
        }

    }
}
