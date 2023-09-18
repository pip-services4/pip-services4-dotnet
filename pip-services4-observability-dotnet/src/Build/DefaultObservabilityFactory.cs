using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Observability.Log;
using PipServices4.Observability.Trace;

namespace PipServices4.Observability.Count
{
	/// <summary>
	/// Creates observability components by their descriptors.
	/// <see cref="Factory"/>
	/// <see cref="LogCounters"/>
	/// <see cref="CompositeCounters"/>
	/// <see cref="ConsoleLogger"/>
	/// <see cref="CompositeLogger"/>
	/// </summary>
	public class DefaultObservabilityFactory : Factory
	{
		private static readonly Descriptor NullCountersDescriptor = new Descriptor("pip-services", "counters", "null", "*", "1.0");
		private static readonly Descriptor LogCountersDescriptor = new Descriptor("pip-services", "counters", "log", "*", "1.0");
		private static readonly Descriptor CompositeCountersDescriptor = new Descriptor("pip-services", "counters", "composite", "*", "1.0");
		private static readonly Descriptor NullLoggerDescriptor = new Descriptor("pip-services", "logger", "null", "*", "1.0");
		private static readonly Descriptor ConsoleLoggerDescriptor = new Descriptor("pip-services", "logger", "console", "*", "1.0");
		private static readonly Descriptor CompositeLoggerDescriptor = new Descriptor("pip-services", "logger", "composite", "*", "1.0");
		private static readonly Descriptor NullTracerDescriptor = new Descriptor("pip-services", "tracer", "null", "*", "1.0");
		private static readonly Descriptor LogTracerDescriptor = new Descriptor("pip-services", "tracer", "log", "*", "1.0");
		private static readonly Descriptor CompositeTracerDescriptor = new Descriptor("pip-services", "tracer", "composite", "*", "1.0");

		/**
         * Create a new instance of the factory.
         */
		public DefaultObservabilityFactory()
		{
			RegisterAsType(NullCountersDescriptor, typeof(NullCounters));
			RegisterAsType(LogCountersDescriptor, typeof(LogCounters));
			RegisterAsType(CompositeCountersDescriptor, typeof(CompositeCounters));
			RegisterAsType(NullLoggerDescriptor, typeof(NullLogger));
			RegisterAsType(ConsoleLoggerDescriptor, typeof(ConsoleLogger));
			RegisterAsType(CompositeLoggerDescriptor, typeof(CompositeLogger));
			RegisterAsType(NullTracerDescriptor, typeof(NullTracer));
			RegisterAsType(LogTracerDescriptor, typeof(LogTracer));
			RegisterAsType(CompositeTracerDescriptor, typeof(CompositeTracer));
		}
	}
}