using PipServices4.Components.Refer;
using PipServices4.Nats.Build;
using PipServices4.Nats.Queues;
using System;
using Xunit;

namespace PipServices4.Nats.Test.Build
{
    public class NatsMessageQueueFactoryTest : IDisposable
	{

		public void Dispose()
		{ }

		[Fact]
		public void TestFactoryCreateMessageQueue()
		{
			var factory = new NatsMessageQueueFactory();
			var descriptor = new Descriptor("pip-services", "message-queue", "nats", "test", "1.0");

			var canResult = factory.CanCreate(descriptor);
			Assert.NotNull(canResult);

			var queue = factory.Create(descriptor) as NatsMessageQueue;
			Assert.NotNull(queue);
			Assert.Equal("test", queue.Name);
		}

	}
}
