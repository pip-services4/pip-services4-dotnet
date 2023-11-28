using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PipServices4.Datadog.Clients
{
	[DataContract]
	public class DataDogMetricPoint
	{
		[DataMember(Name = "time")]
		public DateTime? Time { get; set; }
		[DataMember(Name = "value")]
		public double? Value { get; set; }
	}
}
