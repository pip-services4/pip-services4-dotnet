using PipServices4.Observability.Log;
using Xunit;

namespace PipServices4.Observability.test.Log
{
	//[TestClass]
	public sealed class DiagnosticsLoggerTest
	{
		private ILogger Log { get; set; }
		private LoggerFixture Fixture { get; set; }

		public DiagnosticsLoggerTest()
		{
			Log = new DiagnosticsLogger();
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