using PipServices4.Components.Refer;
using PipServices4.Messaging.Build;
using PipServices4.Messaging.Queues;
using PipServices4.Mqtt.Queues;

namespace PipServices4.Mqtt.Build
{
    public class MqttMessageQueueFactory : MessageQueueFactory
    {
        private static readonly Descriptor MqttMessageQueueDescriptor = new Descriptor("pip-services", "message-queue", "mqtt", "*", "1.0");

        public MqttMessageQueueFactory()
        {
            Register(MqttMessageQueueDescriptor, (locator) => {
                Descriptor descriptor = locator as Descriptor;
                var name = descriptor != null ? descriptor.Name : null;
                return CreateQueue(name);
            });
        }

        public override IMessageQueue CreateQueue(string name)
        {
            var queue = new MqttMessageQueue(name);
            if (_config != null)
            {
                queue.Configure(_config);
            }
            if (_references != null)
            {
                queue.SetReferences(_references);
            }
            return queue;
        }
    }
}
