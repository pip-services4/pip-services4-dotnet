using System;
using System.Runtime.Serialization;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// Exception thrown when incorrect character is detected input stream. 
    /// </summary>
    [DataContract]
    public class InvalidCharacterException : TokenizerException
    {
        public InvalidCharacterException(string correlationId = null, string code = null,
            string message = null, Exception innerException = null)
            : base(correlationId, code, message, innerException) { }
    }
}
