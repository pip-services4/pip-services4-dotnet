using PipServices4.Components.Refer;
using PipServices4.Mqtt.Build;
using PipServices4.Mqtt.Queues;
using System;
using Xunit;

namespace PipServices4.Mqtt.Test.Build
{
    public class MqttMessageQueueFactoryTest : IDisposable
	{

		public void Dispose()
		{ }

		[Fact]
		public void TestFactoryCreateMessageQueue()
		{
			var factory = new MqttMessageQueueFactory();
			var descriptor = new Descriptor("pip-services", "message-queue", "mqtt", "test", "1.0");

			var canResult = factory.CanCreate(descriptor);
			Assert.NotNull(canResult);

			var queue = factory.Create(descriptor) as MqttMessageQueue;
			Assert.NotNull(queue);
			Assert.Equal("test", queue.Name);
		}

	}
}
