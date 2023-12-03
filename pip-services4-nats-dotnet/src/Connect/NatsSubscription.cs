using System;
using NATS.Client;

namespace PipServices4.Nats.Connect
{
    public class NatsSubscription
    {
        public string Subject { get; set; }
        public string Queue { get; set; }
        public INatsMessageListener Listener { get; set; }
        public IAsyncSubscription Handler { get; set; }
    }
}
