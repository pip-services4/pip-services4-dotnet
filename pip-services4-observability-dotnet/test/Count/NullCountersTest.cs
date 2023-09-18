using PipServices4.Observability.Count;
using Xunit;

namespace PipServices4.Observability.test.Count
{
	//[TestClass]
	public sealed class NullCountersTest
	{
		private readonly NullCounters _counters = new NullCounters();

		public NullCountersTest()
		{
			_counters = new NullCounters();
		}

		[Fact]
		public void TestSimpleCounters()
		{
			_counters.Last("Test.LastValue", 123);
			_counters.Increment("Test.Increment", 3);
			_counters.Stats("Test.Statistics", 123);
		}

		[Fact]
		public void TestMeasureElapsedTime()
		{
			var timer = _counters.BeginTiming("Test.Elapsed");
			timer.EndTiming();
		}
	}
}
