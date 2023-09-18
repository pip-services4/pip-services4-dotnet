using PipServices4.Components.Refer;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using Xunit;

namespace PipServices4.Observability.test.Count
{
	//[TestClass]
	public sealed class LogCountersTest
	{
		private readonly LogCounters _counters = new LogCounters();
		private readonly CountersFixture _fixture;

		public LogCountersTest()
		{
			var log = new ConsoleLogger();
			var refs = References.FromTuples(
				new Descriptor("pip-services", "logger", "null", "default", "1.0"), log
			);

			_counters.SetReferences(refs);

			_fixture = new CountersFixture(_counters);
		}

		[Fact]
		public void TestSimpleCounters()
		{
			_fixture.TestSimpleCounters();
		}

		[Fact]
		public void TestMeasureElapsedTime()
		{
			_fixture.TestMeasureElapsedTime();
		}
	}
}
