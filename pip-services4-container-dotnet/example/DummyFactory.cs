using PipServices4.Components.Build;
using PipServices4.Components.Refer;

namespace PipServices4.Container
{
	public class DummyFactory : Factory
	{
		public static Descriptor Descriptor = new Descriptor("pip-services4-dummies", "factory", "default", "default", "1.0");
		public static Descriptor ControllerDescriptor = new Descriptor("pip-services4-dummies", "controller", "*", "*", "1.0");

		public DummyFactory()
		{
			RegisterAsType(ControllerDescriptor, typeof(DummyController));
		}
	}
}
