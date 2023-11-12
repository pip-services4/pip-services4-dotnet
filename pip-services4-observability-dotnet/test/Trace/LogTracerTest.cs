using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Observability.Log;
using PipServices4.Observability.Trace;
using System;
using Xunit;

namespace PipServices4.Observability.test.Trace
{
	public sealed class LogTracerTest
	{
		LogTracer _tracer;

		public LogTracerTest()
		{
			_tracer = new LogTracer();
			_tracer.SetReferences(References.FromTuples(
				new Descriptor("pip-services", "logger", "null", "default", "1.0"), new NullLogger()
			));
		}

		[Fact]
		public void TestSimpleTracing()
		{
			_tracer.Trace(Context.FromTraceId("123"), "mycomponent", "mymethod", 123456);
			_tracer.Failure(Context.FromTraceId("123"), "mycomponent", "mymethod", new Exception("Test error"), 123456);
		}

		[Fact]
		public void TestTraceTiming()
		{
			var timing = _tracer.BeginTrace(Context.FromTraceId("123"), "mycomponent", "mymethod");
			timing.EndTrace();

			timing = _tracer.BeginTrace(Context.FromTraceId("123"), "mycomponent", "mymethod");
			timing.EndFailure(new Exception("Test error"));
		}
	}
}
