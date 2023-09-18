using PipServices4.Components.Build;
using PipServices4.Components.Refer;

namespace PipServices4.Components.Context
{
	/// <summary>
	/// Creates information components by their descriptors.
	/// </summary>
	/// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_config_1_1_config_params.html">Factory</a>, 
	/// <see cref="ContextInfo"/>
	public class DefaultInfoFactory : Factory
    {
        public static readonly Descriptor Descriptor = new Descriptor("pip-services", "factory", "info", "default", "1.0");
        public static Descriptor ContextInfoDescriptor = new Descriptor("pip-services", "context-info", "default", "*", "1.0");
        public static Descriptor ContainerInfoDescriptor = new Descriptor("pip-services", "container-info", "default", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultInfoFactory()
        {
            RegisterAsType(ContextInfoDescriptor, typeof(ContextInfo));
            RegisterAsType(ContainerInfoDescriptor, typeof(ContextInfo));
        }
    }
}
