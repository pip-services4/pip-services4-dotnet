using PipServices4.Components.Build;
using PipServices4.Components.Refer;

namespace PipServices4.Container.Test
{
	/// <summary>
	/// Creates test components by their descriptors.
	/// </summary>
	/// <see cref="Factory"/>, <see cref="Shutdown"/>
	public class DefaultTestFactory : Factory
	{
		private static readonly Descriptor ShutdownDescriptor = new Descriptor("pip-services", "shutdown", "*", "*", "1.0");

		/// <summary>
		/// Create a new instance of the factory.
		/// </summary>
		public DefaultTestFactory() : base()
		{
			RegisterAsType(DefaultTestFactory.ShutdownDescriptor, typeof(Shutdown));
		}
	}
}
