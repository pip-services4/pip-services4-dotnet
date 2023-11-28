using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Messaging.Queues;

namespace PipServices4.Messaging.Build
{
    /// <summary>
    /// Creates MemoryMessageQueue components by their descriptors.
    /// Name of created message queue is taken from its descriptor.
    /// </summary>
    /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_build_1_1_factory.html">Factory</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-messaging-dotnet/class_pip_services_1_1_messaging_1_1_queues_1_1_memory_message_queue.html">MemoryMessageQueue</a>
    public class DefaultMessagingFactory : Factory
    {
        private static Descriptor MemoryMessageQueueDescriptor = new Descriptor("pip-services", "message-queue", "memory", "*", "1.0");
        private static Descriptor MemoryMessageQueueFactoryDescriptor = new Descriptor("pip-services", "queue-factory", "memory", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultMessagingFactory()
        {
            Register(MemoryMessageQueueDescriptor, (locator) =>
            {
                Descriptor descriptor = locator as Descriptor;
                var name = descriptor != null ? descriptor.Name : null;

                return new MemoryMessageQueue(name);
            });
            RegisterAsType(MemoryMessageQueueFactoryDescriptor, typeof(MemoryMessageQueueFactory));
        }
    }
}
