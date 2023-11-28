using PipServices4.Components.Context;
using PipServices4.Observability.Log;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices3.DataDog.Fixtures
{
    public class LoggerFixture
	{
		private CachedLogger _logger;

		public LoggerFixture(CachedLogger logger)
		{
			_logger = logger;
		}

		public void TestLogLevel()
		{
			Assert.True(_logger.Level >= LogLevel.None);
			Assert.True(_logger.Level <= LogLevel.Trace);
		}

		public async Task TestSimpleLoggingAsync()
		{
			var context = Context.FromTraceId("987");
			_logger.Level = LogLevel.Trace;
			_logger.Fatal(context, null, "Fatal error message");
			_logger.Error(context, null, "Error message");
			_logger.Warn(context, "Warning message");
			_logger.Info(context, "Information message");
			_logger.Debug(context, "Debug message");
			_logger.Trace(context, "Trace message");
			_logger.Dump();

			await Task.Delay(1000);
		}

		public async Task TestErrorLoggingAsync()
		{
			try
			{
				// Raise an exception
				throw new ApplicationException();
			}
			catch (Exception ex)
			{
                var context = Context.FromTraceId("987");
                _logger.Fatal(context, ex, "Fatal error");
				_logger.Error(context, ex, "Recoverable error");

				Assert.NotNull(ex);
			}

			_logger.Dump();

			await Task.Delay(1000);
		}
	}
}
