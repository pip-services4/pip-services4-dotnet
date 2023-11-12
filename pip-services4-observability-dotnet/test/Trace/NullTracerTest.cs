using PipServices4.Components.Context;
using PipServices4.Observability.Trace;
using System;
using Xunit;

namespace PipServices4.Observability.test.Trace
{
	public class NullTracerTest
	{
		NullTracer _tracer;

		public NullTracerTest()
		{
			_tracer = new NullTracer();
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
