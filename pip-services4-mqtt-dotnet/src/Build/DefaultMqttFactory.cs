using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Mqtt.Queues;

namespace PipServices4.Mqtt.Build
{
    /// <summary>
    /// Creates MqttMessageQueue components by their descriptors.
    /// </summary>
    public class DefaultMqttFactory: Factory
    {
        private static Descriptor MqttMessageQueueFactoryDescriptor = new Descriptor("pip-services", "queue-factory", "mqtt", "*", "1.0");
        private static Descriptor MqttMessageQueueDescriptor = new Descriptor("pip-services", "message-queue", "mqtt", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultMqttFactory()
        {
            RegisterAsType(MqttMessageQueueFactoryDescriptor, typeof(MqttMessageQueueFactory));
            Register(MqttMessageQueueDescriptor, (locator) => {
                Descriptor descriptor = locator as Descriptor;
                var name = descriptor != null ? descriptor.Name : null;
                return new MqttMessageQueue(name);
            });
        }
    }
}
