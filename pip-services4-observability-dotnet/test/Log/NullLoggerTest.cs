using PipServices4.Observability.Log;
using Xunit;

namespace PipServices4.Observability.test.Log
{
	//[TestClass]
	public sealed class NullLoggerTest
	{
		private ILogger Log { get; set; }
		private LoggerFixture Fixture { get; set; }

		public NullLoggerTest()
		{
			Log = new NullLogger();
			Fixture = new LoggerFixture(Log);
		}

		[Fact]
		public void TestLogLevel()
		{
			Fixture.TestLogLevel();
		}

		[Fact]
		public void TestSimpleLogging()
		{
			Fixture.TestSimpleLogging();
		}

		[Fact]
		public void TestErrorLogging()
		{
			Fixture.TestErrorLogging();
		}
	}
}