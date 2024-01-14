using System;

namespace PipServices4.Messaging.Queues
{
    public class LockedMessage
    {
        public MessageEnvelope Message { get; set; }
        public DateTime ExpirationTimeUtc { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}
