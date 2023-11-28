using System;
using MQTTnet;

namespace PipServices4.Mqtt.Connect
{
    public interface IMqttMessageListener
    {
        void OnMessage(MqttApplicationMessage message);
    }
}
