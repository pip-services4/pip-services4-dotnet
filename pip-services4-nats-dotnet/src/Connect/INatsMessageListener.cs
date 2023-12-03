using System;
using NATS.Client;

namespace PipServices4.Nats.Connect
{
    public interface INatsMessageListener
    {
        void OnMessage(object sender, MsgHandlerEventArgs e);
    }
}
