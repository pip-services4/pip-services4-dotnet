using PipServices3.Rpc.Clients;
using PipServices4.Components.Refer;
using PipServices4.Rpc.Test.Sample;
using System;
using Xunit;

namespace PipServices4.Rpc.Test.Clients
{
    public class DummyDirectClientTest: IDisposable
    {
        private readonly DummyService _service;
        private readonly DummyDirectClient _client;
        private readonly DummyClientFixture _fixture;

        public DummyDirectClientTest()
        {
            _service = new DummyService();
            _client = new DummyDirectClient();

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "service", "default", "default", "1.0"), _service
            );
            _client.SetReferences(references);

            _fixture = new DummyClientFixture(_client);

            var clientTask = _client.OpenAsync(null);
            clientTask.Wait();
        }

        [Fact]
        public void TestCrudOperations()
        {
            var task = _fixture.TestCrudOperations();
            task.Wait();
        }

        public void Dispose()
        {
            var task = _client.CloseAsync(null);
            task.Wait();
        }
    }
}
