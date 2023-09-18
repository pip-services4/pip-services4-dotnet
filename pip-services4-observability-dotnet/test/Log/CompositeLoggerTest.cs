using PipServices4.Components.Refer;
using PipServices4.Observability.Log;
using Xunit;

namespace PipServices4.Observability.test.Log
{
	//[TestClass]
	public sealed class CompositeLoggerTest
	{
		private CompositeLogger Log { get; set; }
		private LoggerFixture Fixture { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeLoggerTest"/> class.
		/// </summary>
		public CompositeLoggerTest()
		{
			Log = new CompositeLogger();

			var refs = References.FromTuples(
				new Descriptor("pip-services", "logger", "null", "default", "1.0"), new NullLogger(),
				new Descriptor("pip-services", "logger", "console", "default", "1.0"), new ConsoleLogger(),
				DefaultLoggerFactory.DiagnosticsLoggerDescriptor, new DiagnosticsLogger()
			);
			Log.SetReferences(refs);

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