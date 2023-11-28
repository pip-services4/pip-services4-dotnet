using System.Runtime.Serialization;

namespace PipServices4.Postgres.Test.Fixtures
{
	[DataContract]
	public class SubDummy
	{
		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "array_of_double")]
		public double[] ArrayOfDouble { get; set; }
	}
}
