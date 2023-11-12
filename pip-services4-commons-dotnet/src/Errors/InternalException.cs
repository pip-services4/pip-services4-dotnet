using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PipServices4.Commons.Errors
{
    /// <summary>
    /// Class of errors related to internal system errors, programming mistakes, etc.
    /// </summary>
#if CORE_NET
    [DataContract]
#else
    [Serializable]
#endif
    public class InternalException : ApplicationException
    {
        /// <summary>
        /// Creates an error instance with error message.
        /// </summary>
        /// <param name="message">(optional) a human-readable description of the error.</param>
        [JsonConstructor]
        public InternalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an error instance and assigns its values.
        /// </summary>
        public InternalException()
        {
        }

        /// <summary>
        /// Creates an error instance and assigns its values.
        /// </summary>
        /// <param name="innerException">an error object</param>
        /// See <see cref="ErrorCategory.Internal"/>
        public InternalException(Exception innerException) 
            : base(ErrorCategory.Internal, null, null, null)
        {
            Status = 500;
            WithCause(innerException);
        }

        /// <summary>
        /// Creates an error instance and assigns its values.
        /// </summary>
        /// <param name="traceId">(optional) a unique transaction id to trace execution through call chain.</param>
        /// <param name="code">(optional) a unique error code. Default: "UNKNOWN"</param>
        /// <param name="message">(optional) a human-readable description of the error.</param>
        /// <param name="innerException">an error object</param>
        public InternalException(string traceId = null, string code = null, string message = null, Exception innerException = null) 
            : base(ErrorCategory.Internal, traceId, code, message)
        {
            Status = 500;
            WithCause(innerException);
        }

#if !CORE_NET
        protected InternalException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
#endif

    }
}