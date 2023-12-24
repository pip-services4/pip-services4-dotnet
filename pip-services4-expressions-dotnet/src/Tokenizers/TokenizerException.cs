using System;
using System.Runtime.Serialization;

using PipServices4.Commons.Errors;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// Exception that can be thrown by Tokenizer.
    /// </summary>
    [DataContract]
    public class TokenizerException : BadRequestException
    {
        public TokenizerException(string correlationId = null, string code = null,
            string message = null, Exception innerException = null)
            : base(correlationId, code, message, innerException) { }
    }

}

