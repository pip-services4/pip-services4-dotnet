using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Datadog.Log;

namespace PipServices4.Datadog.Build
{
    /// <summary>
    /// Creates DataDog components by their descriptors.
    /// </summary>
    public class DefaultDataDogFactory : Factory
    {
	    public static Descriptor Descriptor = new Descriptor("pip-services", "factory", "datadog", "default", "1.0");
	    public static Descriptor DataDogLoggerDescriptor = new Descriptor("pip-services", "logger", "datadog", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultDataDogFactory()
        {
            RegisterAsType(DataDogLoggerDescriptor, typeof(DataDogLogger));
        }
    }
}
