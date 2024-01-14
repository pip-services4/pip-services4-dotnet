﻿using PipServices4.Config.Config;
using PipServices4.Config.Connect;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Azure.Queues
{
    public class StorageMessageQueueTest
    {
        StorageMessageQueue Queue { get; set; }
        MessageQueueFixture Fixture { get; set; }

        public StorageMessageQueueTest()
        {
            var config = YamlConfigReader.ReadConfig(null, "..\\..\\..\\..\\config\\test_connections.yaml", null);
            var connection = ConnectionParams.FromString(config.GetAsString("storage_queue"));
            Queue = new StorageMessageQueue("TestQueue", connection) {Interval = 50};
            //Queue.SetReferences(new MockReferences());
            Queue.OpenAsync(null).Wait();

            Fixture = new MessageQueueFixture(Queue);
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStorageSendReceiveMessageAsync()
        {
            await Queue.ClearAsync(null);
            await Fixture.TestSendReceiveMessageAsync();
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStorageReceiveSendMessageAsync()
        {
            await Queue.ClearAsync(null);
            await Fixture.TestReceiveSendMessageAsync();
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStorageReceiveAndCompleteAsync()
        {
            await Queue.ClearAsync(null);
            await Fixture.TestReceiveAndCompleteMessageAsync();
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStorageReceiveAndRenewLockAndCompleteAsync()
        {
            await Queue.ClearAsync(null);
            await Fixture.TestReceiveAndRenewLockAndCompleteMessageAsync();
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStorageReceiveAndAbandonAsync()
        {
            await Queue.ClearAsync(null);
            await Fixture.TestReceiveAndAbandonMessageAsync();
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStorageSendPeekMessageAsync()
        {
            await Queue.ClearAsync(null);
            await Fixture.TestSendPeekMessageAsync();
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStoragePeekNoMessageAsync()
        {
            await Queue.ClearAsync(null);
            await Fixture.TestPeekNoMessageAsync();
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStorageOnMessageAsync()
        {
            await Queue.ClearAsync(null);
            await Fixture.TestOnMessageAsync();
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStorageMoveToDeadMessageAsync()
        {
            await Fixture.TestMoveToDeadMessageAsync();
        }

        [Fact(Skip = "Not valid credentials")]
        //[Fact]
        public async Task TestStorageMessageCountAsync()
        {
            await Fixture.TestMessageCountAsync();
        }

        //[TestMethod]
        //public async Task TestStorageNullMessageAsync()
        //{
        //    var envelope = await Queue.ReceiveAsync(TimeSpan.FromMilliseconds(10000000));
        //    await Queue.CompleteAsync(envelope);
        //    Assert.IsNotNull(envelope);
        //}
    }
}
