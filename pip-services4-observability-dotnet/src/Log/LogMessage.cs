using PipServices4.Commons.Errors;
using PipServices4.Components.Context;
using System;
using System.Runtime.Serialization;

namespace PipServices4.Observability.Log
{
	/// <summary>
	/// Data object to store captured log messages. This object is used by CachedLogger.
	/// </summary>
	[DataContract]
	public class LogMessage
	{
		/** The time then message was generated */
		[DataMember(Name = "time")]
		public DateTime Time { get; set; }

		/** The source (context name) */
		[DataMember(Name = "source")]
		public string Source { get; set; }

		/** This log level */
		[DataMember(Name = "level")]
		public string Level { get; set; }

		/** The transaction id to trace execution through call chain. */
		[DataMember(Name = "trace_id")]
		public string TraceId { get; set; }

		/** The description of the captured error. */
		[DataMember(Name = "error")]
		public ErrorDescription Error { get; set; }

		/** The human-readable message */
		[DataMember(Name = "message")]
		public string Message { get; set; }
	}
}
