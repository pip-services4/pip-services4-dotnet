using PipServices4.Components.Refer;
using PipServices4.Kafka.Queues;
using PipServices4.Messaging.Build;
using PipServices4.Messaging.Queues;

namespace PipServices4.Kafka.Build
{
    public class KafkaMessageQueueFactory : MessageQueueFactory
    {
        private static readonly Descriptor KafkaMessageQueueDescriptor = new Descriptor("pip-services", "message-queue", "kafka", "*", "1.0");

        public KafkaMessageQueueFactory()
        {
            Register(KafkaMessageQueueDescriptor, (locator) => {
                Descriptor descriptor = locator as Descriptor;
                var name = descriptor != null ? descriptor.Name : null;
                return CreateQueue(name);
            });
        }

        public override IMessageQueue CreateQueue(string name)
        {
            var queue = new KafkaMessageQueue(name);
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
