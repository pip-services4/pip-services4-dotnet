using System;
using System.Runtime.Serialization;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// Exception thrown when TokenizerState was called in incorrect state or for unsupported character.
    /// </summary>
    [DataContract]
    public class IncorrectStateException : TokenizerException
    {
        public IncorrectStateException(string correlationId = null, string code = null,
            string message = null, Exception innerException = null)
            : base(correlationId, code, message, innerException) { }
    }
}
