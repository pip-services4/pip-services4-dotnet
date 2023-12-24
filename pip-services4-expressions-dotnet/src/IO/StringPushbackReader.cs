using System;
using System.Text;

namespace PipServices4.Expressions.IO
{
    /// <summary>
    /// Wraps string to provide unlimited pushback that allows tokenizers
    /// to look ahead through stream to perform lexical analysis.
    /// </summary>
    public class StringPushbackReader : IPushbackReader
    {
        public const char Eof = '\xffff';

        private string _content;
        private int _position;
        private int _pushbackCharsCount;
        private char _pushbackSingleChar;
        private StringBuilder _pushbackChars = new StringBuilder();

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="content">A text content to be read.</param>
        public StringPushbackReader(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            _content = content;
            _position = 0;
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
                if (_position < _content.Length)
                {
                    _position++;
                    return _content[_position - 1];
                }

                return Eof;
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
                return _position < _content.Length ? _content[_position] : Eof;
            }
        }

        /// <summary>
        /// Puts the specified character to the top of the stream.
        /// </summary>
        /// <param name="value">A character to be pushed back.</param>
        public void Pushback(char value)
        {
            // Skip EOF
            if (value == Eof)
            {
                return;
            }

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

