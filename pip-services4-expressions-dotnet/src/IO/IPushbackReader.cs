namespace PipServices4.Expressions.IO
{
    /// <summary>
    /// Defines reader with ability to push back characters.
    /// This reader is used by tokenizers to process input streams.
    /// </summary>
    public interface IPushbackReader
    {
        /// <summary>
        /// Reads character from the top of the stream.
        /// </summary>
        /// <returns>A read character or <code>-1</code> if stream processed to the end.</returns>
        char Read();

        /// <summary>
        /// Returns the character from the top of the stream without moving the stream pointer.
        /// </summary>
        /// <returns>A character from the top of the stream or <code>-1</code> if stream is empty.</returns>
        char Peek();

        /// <summary>
        /// Puts the specified character to the top of the stream.
        /// </summary>
        /// <param name="value">A character to be pushed back.</param>
        void Pushback(char value);

        /// <summary>
        /// Pushes the specified string to the top of the stream.
        /// </summary>
        /// <param name="value">A string to be pushed back.</param>
        void PushbackString(string value);
    }
}

