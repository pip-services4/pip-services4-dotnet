using PipServices4.Commons.Errors;
using PipServices4.Components.Context;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PipServices4.Data.Validate
{
    /// <summary>
    /// Errors in schema validation.
    /// Validation errors are usually generated based in ValidationResult.
    /// If using strict mode, warnings will also raise validation exceptions.
    /// </summary>
    /// See <see cref="BadRequestException"/>, <see cref="ValidationResult"/>
#if CORE_NET
    [DataContract]
#else
    [Serializable]
#endif
    public class ValidationException : BadRequestException
    {
        private static long SerialVersionUid { get; } = -1459801864235223845L;

        /// <summary>
        /// Creates a new instance of validation exception and assigns its values.
        /// </summary>
        /// <param name="traceId">(optional) a unique transaction id to trace execution
        /// through call chain.</param>
        /// <param name="results">(optional) a list of validation results</param>
        /// See <see cref="ValidationResult"/>
        public ValidationException(string traceId, IList<ValidationResult> results) :
            this(traceId, ComposeMessage(results))
        {
            WithDetails("results", results);
        }

        /// <summary>
        /// Creates a new instance of validation exception and assigns its values.
        /// </summary>
        /// <param name="traceId">(optional) a unique transaction id to trace execution
        /// through call chain.</param>
        /// <param name="message">(optional) a human-readable description of the error.</param>
        /// See <see cref="ValidationResult"/>
        public ValidationException(string traceId, string message) :
            base(traceId, "INVALID_DATA", message)
        {
        }

#if !CORE_NET
        protected ValidationException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
#endif

        /// <summary>
        /// Composes human readable error message based on validation results.
        /// </summary>
        /// <param name="results">a list of validation results.</param>
        /// <returns>a composed error message.</returns>
        /// See <see cref="ValidationResult"/>
        public static string ComposeMessage(IList<ValidationResult> results)
        {
            var builder = new StringBuilder();
            builder.Append("Validation failed");

            if (results != null && results.Count > 0)
            {
                var first = true;
                foreach (var result in results)
                {
                    if (result.Type == ValidationResultType.Information)
                        continue;

                    builder.Append(!first ? ": " : ", ");
                    builder.Append(result.Message);
                    first = false;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates a new ValidationException based on errors in validation results.
        /// If validation results have no errors, than null is returned.
        /// </summary>
        /// <param name="traceId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="results">list of validation results that may contain errors</param>
        /// <param name="strict">true to treat warnings as errors.</param>
        /// <returns>a newly created ValidationException or null if no errors in found.</returns>
        /// See <see cref="ValidationResult"/>
        public static ValidationException FromResults(string traceId, List<ValidationResult> results, bool strict)
        {
            var hasErrors = false;

            for(int index =0; index < results.Count; index++)
            {
                var result = results[index];

                if (result.Type == ValidationResultType.Error)
                    hasErrors = true;

                if (strict && result.Type == ValidationResultType.Warning)
                    hasErrors = true;
            }

            return hasErrors ? new ValidationException(traceId, results) : null;
        }

        /// <summary>
        /// Throws ValidationException based on errors in validation results. If
        /// validation results have no errors, than no exception is thrown.
        /// </summary>
        /// <param name="traceId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="results">list of validation results that may contain errors</param>
        /// <param name="strict">true to treat warnings as errors.</param>
        /// See <see cref="ValidationResult"/>, <see cref="ValidationException"/>
        public static void ThrowExceptionIfNeeded(string traceId, IList<ValidationResult> results, bool strict)
        {
            var hasErrors = false;
            foreach (var result in results)
            {
                if (result.Type == ValidationResultType.Error)
                    hasErrors = true;

                if (strict && result.Type == ValidationResultType.Warning)
                    hasErrors = true;
            }

            if (hasErrors)
                throw new ValidationException(traceId, results);
        }

        /// <summary>
        /// Throws ValidationException based on errors in validation results. If
        /// validation results have no errors, than no exception is thrown.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.
        /// <param name="results">list of validation results that may contain errors</param>
        /// <param name="strict">true to treat warnings as errors.</param>
        /// See <see cref="ValidationResult"/>, <see cref="ValidationException"/>
        public static void ThrowExceptionIfNeeded(IContext context, IList<ValidationResult> results, bool strict)
        {
            var hasErrors = false;
            foreach (var result in results)
            {
                if (result.Type == ValidationResultType.Error)
                    hasErrors = true;

                if (strict && result.Type == ValidationResultType.Warning)
                    hasErrors = true;
            }

            if (hasErrors)
                throw new ValidationException(context != null ? ContextResolver.GetTraceId(context) : null, results);
        }
    }
}