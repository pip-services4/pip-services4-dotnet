using System;
using System.Collections.Generic;
using System.Text;

namespace PipServices4.Expressions.IO
{
    public class StringScanner : IScanner
    {
        public const char Eof = '\xffff';

        private string _content;
        private int _position;
        private int _line;
        private int _column;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="content">A text content to be read.</param>
        public StringScanner(string content)
        {
            if (content == null)
                throw new Exception("Content cannot be null");

            _content = content;
            _position = -1;
            _line = 1;
            _column = 0;
        }

        /// <summary>
        /// Returns character from a specified position in the stream
        /// </summary>
        /// <param name="position">a position to read character</param>
        /// <returns>a character from the specified position or EOF ('\xffff')</returns>
        private char CharAt(int position)
        {
            if (position < 0 || position >= _content.Length)
                return StringScanner.Eof;

            return _content[position];
        }

        /// <summary>
        /// Checks if the current character represents a new line 
        /// </summary>
        /// <param name="charBefore">the character before the current one</param>
        /// <param name="charAt">the current character</param>
        /// <param name="charAfter">the character after the current one</param>
        /// <returns><code>true</code> if the current character is a new line, or <code>false</code> otherwise.</returns>
        private bool IsLine(int charBefore, int charAt, int charAfter)
        {
            if (charAt != 10 && charAt != 13)
                return false;

            if (charAt == 13 && (charBefore == 10 || charAfter == 10))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the current character represents a column 
        /// </summary>
        /// <param name="charAt">the current character</param>
        /// <returns><code>true</code> if the current character is a column, or <code>false</code> otherwise.</returns>
        private bool IsColumn(int charAt)
        {
            if (charAt == 10 || charAt == 13)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the current line number
        /// </summary>
        /// <returns>The current line number in the stream</returns>
        public int Line()
        {
            return _line;
        }

        /// <summary>
        /// Gets the column in the current line
        /// </summary>
        /// <returns>The column in the current line in the stream</returns>
        public int Column()
        {
            return _column;
        }

        /// <summary>
        /// Reads character from the top of the stream.
        /// A read character or<code>-1</code> if stream processed to the end.
        /// </summary>
        /// <returns></returns>
        public char Read()
        {
            // Skip if we are at the end
            if ((_position + 1) > _content.Length)
                return StringScanner.Eof;

            // Update the current position
            _position++;

            if (_position >= _content.Length)
                return StringScanner.Eof;

            // Update line and columns
            int charBefore = CharAt(this._position - 1);
            int charAt = CharAt(this._position);
            int charAfter = CharAt(this._position + 1);

            if (IsLine(charBefore, charAt, charAfter))
            {
                _line++;
                _column = 0;
            }

            if (IsColumn(charAt))
                _column++;


            return (char)charAt;
        }

        /// <summary>
        /// Returns the character from the top of the stream without moving the stream pointer.
        /// </summary>
        /// <returns>A character from the top of the stream or <code>-1</code> if stream is empty.</returns>
        public char Peek()
        {
            return CharAt(_position + 1);
        }

        /// <summary>
        /// Gets the next character line number
        /// </summary>
        /// <returns>The next character line number in the stream</returns>
        public int PeekLine()
        {
            int charBefore = CharAt(_position);
            int charAt = CharAt(_position + 1);
            int charAfter = CharAt(_position + 2);

            return IsLine(charBefore, charAt, charAfter) ? _line + 1 : _line;
        }

        /// <summary>
        /// Gets the next character column number
        /// </summary>
        /// <returns>The next character column number in the stream</returns>
        public int PeekColumn()
        {
            int charBefore = CharAt(_position);
            int charAt = CharAt(_position + 1);
            int charAfter = CharAt(_position + 2);

            if (IsLine(charBefore, charAt, charAfter))
                return 0;

            return IsColumn(charAt) ? _column + 1 : _column;
        }

        /// <summary>
        /// Puts the one character back into the stream stream.
        /// </summary>
        public void Unread()
        {
            // Skip if we are at the beginning
            if (_position < -1)
                return;

            // Update the current position
            _position--;

            // Update line and columns (optimization)
            if (_column > 0)
            {
                _column--;
                return;
            }

            // Update line and columns (full version)
            _line = 1;
            _column = 0;

            int charBefore = StringScanner.Eof;
            int charAt = StringScanner.Eof;
            int charAfter = CharAt(0);

            for (int position = 0; position <= _position; position++)
            {
                charBefore = charAt;
                charAt = charAfter;
                charAfter = CharAt(position + 1);

                if (IsLine(charBefore, charAt, charAfter))
                {
                    _line++;
                    _column = 0;
                }
                if (IsColumn(charAt))
                    _column++;
            }
        }

        /// <summary>
        /// Pushes the specified number of characters to the top of the stream.
        /// </summary>
        /// <param name="count">A number of characcted to be pushed back.</param>
        public void UnreadMany(int count)
        {
            while (count > 0)
            {
                this.Unread();
                count--;
            }
        }

        /// <summary>
        /// Resets scanner to the initial position
        /// </summary>
        public void Reset()
        {
            _position = -1;
            _line = 1;
            _column = 0;
        }
    }
}
