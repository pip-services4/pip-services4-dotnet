using PipServices4.Commons.Errors;
using System;

namespace PipServices4.Observability.Trace
{
	/// <summary>
	/// Data object to store captured operation traces.
	/// This object is used by <see cref="CachedTracer"/>.
	/// </summary>
	public class OperationTrace
	{
		/// <summary>
		/// The time when operation was executed 
		/// </summary>
		public DateTime Time;

		/// <summary>
		/// The source (context name)
		/// </summary>
		public string Source;

		/// <summary>
		/// The name of component
		/// </summary>
		public string Component;

		/// <summary>
		/// The name of the executed operation
		/// </summary>
		public string Operation;

		/// <summary>
		/// The transaction id to trace execution through call chain.
		/// </summary>
		public string CorrelationId;

		/// <summary>
		/// The duration of the operation in milliseconds
		/// </summary>
		public long Duration;

		/// <summary>
		/// The description of the captured error
		/// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_errors_1_1_error_description.html">ErrorDescription</a>, 
		/// <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_errors_1_1_application_exception.html">ApplicationException </a>
		/// </summary>
		public ErrorDescription Error;
	}
}
