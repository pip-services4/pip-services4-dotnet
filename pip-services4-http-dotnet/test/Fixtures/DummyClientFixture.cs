﻿using PipServices4.Components.Context;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Http.Test.Fixtures
{
    public sealed class DummyClientFixture
    {
        private readonly Dummy _dummy1 = new Dummy("1", "Key 1", "Content 1");
        private readonly Dummy _dummy2 = new Dummy("2", "Key 2", "Content 2");

        private readonly IDummyClient _client;

        public DummyClientFixture(IDummyClient client)
        {
            Assert.NotNull(client);
            _client = client;
        }

        public async Task TestCrudOperations()
        {
            // Create one dummy
            var dummy1 = await _client.CreateAsync(Context.FromTraceId("1"), _dummy1);

            Assert.NotNull(dummy1);
            Assert.NotNull(dummy1.Id);
            Assert.Equal(_dummy1.Key, dummy1.Key);
            Assert.Equal(_dummy1.Content, dummy1.Content);

            // Create another dummy
            var dummy2 = await _client.CreateAsync(Context.FromTraceId("2"), _dummy2);

            Assert.NotNull(dummy2);
            Assert.NotNull(dummy2.Id);
            Assert.Equal(_dummy2.Key, dummy2.Key);
            Assert.Equal(_dummy2.Content, dummy2.Content);

            // Get all dummies
            var dummies = await _client.GetPageByFilterAsync(Context.FromTraceId("3"), null, null);
            Assert.NotNull(dummies);
            Assert.True(dummies.Data.Count >= 2);

            // Update the dummy
            dummy1.Content = "Updated Content 1";
            var dummy = await _client.UpdateAsync(Context.FromTraceId("4"), dummy1);

            Assert.NotNull(dummy);
            Assert.Equal(dummy1.Id, dummy.Id);
            Assert.Equal(dummy1.Key, dummy.Key);
            Assert.Equal("Updated Content 1", dummy.Content);

            // Delete the dummy
            await _client.DeleteByIdAsync(Context.FromTraceId("5"), dummy1.Id);

            // Try to get deleted dummy
            dummy = await _client.GetOneByIdAsync(Context.FromTraceId("6"), dummy1.Id);
            Assert.Null(dummy);

            // Check trace id
            var result = await _client.CheckTraceId(Context.FromTraceId("test_trace_id"));
            Assert.Equal("test_trace_id", result);
        }
    }
}
