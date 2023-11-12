using PipServices4.Commons.Errors;
using PipServices4.Components.Context;
using System;
using System.Runtime.Serialization;

namespace PipServices4.Components.Refer
{
    /// <summary>
    /// Error when required component dependency cannot be found.
    /// </summary>
#if CORE_NET
    [DataContract]
#else
    [Serializable]
#endif
    public class ReferenceException : InternalException
    {
        /// <summary>
        /// Creates an error instance and assigns its values.
        /// </summary>
        public ReferenceException()
            : this(null, null)
        {
        }

        /// <summary>
        /// Creates an error instance and assigns its values.
        /// </summary>
        /// <param name="locator">the locator to find reference to dependent component.</param>
        public ReferenceException(object locator)
            : base(null, "REF_ERROR", "Failed to obtain reference to " + locator)
        {
            WithDetails("locator", locator);
        }

        /// <summary>
        /// Creates an error instance and assigns its values.
        /// </summary>
        /// <param name="traceId">(optional) a unique transaction id to trace execution
        /// through call chain.</param>
        /// <param name="locator">the locator to find reference to dependent component.</param>
        public ReferenceException(string traceId, object locator)
            : base(traceId, "REF_ERROR", "Failed to obtain reference to " + locator)
        {
            WithDetails("locator", locator);
        }

        /// <summary>
        /// Creates an error instance and assigns its values.
        /// </summary>
        /// <param name="traceId">(optional) a unique transaction id to trace execution
        /// through call chain.</param>
        /// <param name="message">(optional) a human-readable description of the error.</param>
        public ReferenceException(string traceId, string message)
            : base(traceId, "REF_ERROR", message)
        { }

        public ReferenceException(string traceId, string code, string message)
            : base(traceId, code, message)
        { }

#if !CORE_NET
        protected ReferenceException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
#endif

    }
}
