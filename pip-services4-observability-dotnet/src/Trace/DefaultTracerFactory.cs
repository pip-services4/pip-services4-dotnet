using PipServices4.Components.Build;
using PipServices4.Components.Refer;

namespace PipServices4.Observability.Trace
{
	/// <summary>
	/// Creates <see cref="ITracer"/> components by their descriptors.
	/// 
	/// See <see cref="Factory"/>, <see cref="NullTracer"/>, <see cref="ConsoleTracer"/>, <see cref="CompositeTracer"/>
	/// </summary>
	public class DefaultTracerFactory : Factory
	{
		private static readonly Descriptor NullTracerDescriptor = new Descriptor("pip-services", "tracer", "null", "*", "1.0");
		private static readonly Descriptor LogTracerDescriptor = new Descriptor("pip-services", "tracer", "log", "*", "1.0");
		private static readonly Descriptor CompositeTracerDescriptor = new Descriptor("pip-services", "tracer", "composite", "*", "1.0");

		public DefaultTracerFactory() : base()
		{
			RegisterAsType(NullTracerDescriptor, typeof(NullTracer));
			RegisterAsType(LogTracerDescriptor, typeof(LogTracer));
			RegisterAsType(CompositeTracerDescriptor, typeof(CompositeTracer));
		}

	}
}
