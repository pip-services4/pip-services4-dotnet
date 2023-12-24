using System;
using System.IO;
using System.Text;

namespace PipServices4.Expressions.IO
{
    /// <summary>
    /// Wraps TextReader to provide unlimited pushback that allows tokenizers
    /// to look ahead through stream to perform lexical analysis.
    /// </summary>
    public class TextPushbackReader: IPushbackReader
    {
        public const char Eof = '\xffff';

        private TextReader _reader;
        private int _pushbackCharsCount;
        private char _pushbackSingleChar;
        private StringBuilder _pushbackChars = new StringBuilder();

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="reader">A text reader to be wrapped to add pushback functionality.</param>
        public TextPushbackReader(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            _reader = reader;
        }

        /// <summary>
        /// Reads character from the top of the stream.
        /// </summary>
        /// <returns>A read character or <code>-1</code> if stream processed to the end.</returns>
        public char Read()
        {
            if (_pushbackCharsCount == 1)
            {
                _pushbackCharsCount--;
                return _pushbackSingleChar;
            }
            else if (_pushbackCharsCount > 1)
            {
                char result = _pushbackChars[0];
                _pushbackChars.Remove(0, 1);
                _pushbackCharsCount--;

                if (_pushbackCharsCount == 1)
                {
                    _pushbackSingleChar = _pushbackChars[0];
                    _pushbackChars.Remove(0, 1);
                }

                return result;
            }
            else
            {
                int value = _reader.Read();
                return value != -1 ? Convert.ToChar(value) : Eof;
            }
        }

        /// <summary>
        /// Returns the character from the top of the stream without moving the stream pointer.
        /// </summary>
        /// <returns>A character from the top of the stream or <code>-1</code> if stream is empty.</returns>
        public char Peek()
        {
            if (_pushbackCharsCount == 1)
            {
                return _pushbackSingleChar;
            }
            else if (_pushbackCharsCount > 1)
            {
                return _pushbackChars[0];
            }
            else
            {
                return (char)_reader.Peek();
            }
        }

        /// <summary>
        /// Puts the specified character to the top of the stream.
        /// </summary>
        /// <param name="value">A character to be pushed back.</param>
        public void Pushback(char value)
        {
            if (_pushbackCharsCount == 0)
            {
                _pushbackSingleChar = value;
            }
            else if (_pushbackCharsCount == 1)
            {
                _pushbackChars.Insert(0, new char[] { _pushbackSingleChar });
                _pushbackChars.Insert(0, new char[] { value });
            }
            else
            {
                _pushbackChars.Insert(0, new char[] { value });
            }
            _pushbackCharsCount++;
        }

        /// <summary>
        /// Pushes the specified string to the top of the stream.
        /// </summary>
        /// <param name="value">A string to be pushed back.</param>
        public void PushbackString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length == 1)
                {
                    Pushback(value[0]);
                }
                else
                {
                    if (_pushbackCharsCount == 1)
                    {
                        _pushbackChars.Insert(0, new char[] { _pushbackSingleChar });
                    }
                    _pushbackChars.Insert(0, value);
                    _pushbackCharsCount += value.Length;
                }
            }
        }

    }
}

