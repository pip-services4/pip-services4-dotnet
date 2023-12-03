namespace PipServices4.Kafka.Connect
{
    public interface IKafkaMessageListener
    {
        void OnMessage(KafkaMessage msg);
    }
}
