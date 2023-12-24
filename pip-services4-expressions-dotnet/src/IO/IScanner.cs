using System;
using System.Collections.Generic;
using System.Text;

namespace PipServices4.Expressions.IO
{
    /// <summary>
    /// Defines scanner that can read and unread characters and count lines.
    /// This scanner is used by tokenizers to process input streams.
    /// </summary>
    public interface IScanner
    {
        /// <summary>
        /// Reads character from the top of the stream.
        /// </summary>
        /// <returns>A read character or <code>-1</code> if stream processed to the end.</returns>
        char Read();

        /// <summary>
        /// Gets the current line number
        /// </summary>
        /// <returns>The current line number in the stream</returns>
        int Line();

        /// <summary>
        /// Gets the column in the current line
        /// </summary>
        /// <returns>The column in the current line in the stream</returns>
        int Column();

        /// <summary>
        /// Returns the character from the top of the stream without moving the stream pointer.
        /// </summary>
        /// <returns>A character from the top of the stream or <code>-1</code> if stream is empty.</returns>
        char Peek();

        /// <summary>
        /// Gets the next character line number
        /// </summary>
        /// <returns>The next character line number in the stream</returns>
        int PeekLine();

        /// <summary>
        /// Gets the next character column number
        /// </summary>
        /// <returns>The next character column number in the stream</returns>
        int PeekColumn();

        /// <summary>
        /// Puts the one character back into the stream stream.
        /// </summary>
        void Unread();

        /// <summary>
        /// Pushes the specified number of characters to the top of the stream.
        /// </summary>
        /// <param name="count">A number of characcted to be pushed back.</param>
        void UnreadMany(int count);

        /// <summary>
        /// Resets scanner to the initial position
        /// </summary>
        void Reset();
    }
}
