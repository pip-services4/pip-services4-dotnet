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
    public class MemoryMessageQueueFactory : MessageQueueFactory
    {
        private static readonly Descriptor MemoryQueueDescriptor = new Descriptor("pip-services", "message-queue", "memory", "*", "*");

        public MemoryMessageQueueFactory()
        {
            Register(MemoryQueueDescriptor, (locator) =>
            {
                Descriptor descriptor = locator as Descriptor;
                var name = descriptor != null ? descriptor.Name : null;

                return CreateQueue(name);
            });
        }

        /// <summary>
        /// Creates a message queue component and assigns its name.
        /// </summary>
        /// <param name="name">name of the created message queue</param>
        /// <returns>A created message queue</returns>
        public override IMessageQueue CreateQueue(string name)
        {
            return new MemoryMessageQueue(name);
        }

    }
}
