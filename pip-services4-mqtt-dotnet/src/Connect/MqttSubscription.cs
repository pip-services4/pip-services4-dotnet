using System;
using MQTTnet.Protocol;

namespace PipServices4.Mqtt.Connect
{
    public class MqttSubscription
    {
        public string Topic { get; set; }
        public MqttQualityOfServiceLevel Qos { get; set; }
        public bool Filter { get; set; }
        public IMqttMessageListener Listener { get; set; }
    }
}
