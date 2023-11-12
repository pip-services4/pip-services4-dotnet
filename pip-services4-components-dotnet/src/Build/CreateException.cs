using PipServices4.Commons.Errors;
using PipServices4.Components.Context;
using System;
using System.Runtime.Serialization;

namespace PipServices4.Components.Build
{
    /// <summary>
    /// Error raised when factory is not able to create requested component.
    /// </summary>
    /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_errors_1_1_application_exception.html">ApplicationException</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_errors_1_1_internal_exception.html">InternalException</a>
#if CORE_NET
    [DataContract]
#else
    [Serializable]
#endif
    public class CreateException : InternalException
    {
        /// <summary>
        /// Creates an error instance.
        /// </summary>
        public CreateException()
            : this(null, null)
        { }

        /// <summary>
        /// Creates an error instance and assigns its values.
        /// </summary>
        /// <param name="traceId">(optional) a unique transaction id to trace execution through call chain.</param>
        /// <param name="locator">locator of the component that cannot be created.</param>
        public CreateException(string traceId, object locator) 
            : base(traceId, "CANNOT_CREATE", "Requested component " + locator + " cannot be created")
        {
            WithDetails("locator", locator);
        }

        /// <summary>
        /// Creates an error instance and assigns its values.
        /// </summary>
        /// <param name="context">(optional) a unique transaction id to trace execution through call chain.</param>
        /// <param name="message">human-readable error.</param>
        public CreateException(string traceId, string message) 
            : base(traceId, "CANNOT_CREATE", message)
        { }

#if !CORE_NET
        protected CreateException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
#endif

    }
}