using System;
using System.Runtime.Serialization;
using PipServices4.Commons.Errors;

namespace PipServices4.Expressions.Calculator
{
    /// <summary>
    /// Exception that can be thrown by Expression Parser.
    /// </summary>
    [DataContract]
    public class SyntaxException : BadRequestException
    {
        public SyntaxException(string correlationId = null, string code = null, string message = null, 
            int line = 0, int column = 0, Exception innerException = null) :

                base(correlationId, code, 
                    line != 0 || column != 0 ? message + " at line " + line + " and column " + column : message, 
                    innerException)
        {

        }
    }
}
