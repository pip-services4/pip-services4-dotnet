using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Data.Keys;
using PipServices4.Grpc;
using PipServices4.Grpc.Clients;
using PipServices4.Grpc.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Grpc.Services
{
    [Collection("Sequential")]
    public sealed class DummyGrpcEndpointTest : IDisposable
    {
        private static readonly ConfigParams grpcConfig = ConfigParams.FromTuples(
            "connection.protocol", "http",
            "connection.host", "localhost",
            "connection.port", 3005
        );

        private readonly DummyController _ctrl;
        private readonly DummyCommandableGrpcService _service1;
        private readonly DummyCommandableGrpcService _service2;
        private readonly DummyCommandableGrpcClient _client1;
        private readonly DummyCommandableGrpcClient _client2;

        private readonly GrpcEndpoint _grpcEndpoint;

        public DummyGrpcEndpointTest()
        {
            _ctrl = new DummyController();
            _service1 = new DummyCommandableGrpcService("dummy1");
            _service2 = new DummyCommandableGrpcService("dummy2");

            _client1 = new DummyCommandableGrpcClient("dummy1");
            _client2 = new DummyCommandableGrpcClient("dummy2");

            _grpcEndpoint = new GrpcEndpoint();

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "controller", "default", "default", "1.0"), _ctrl,
                new Descriptor("pip-services4", "endpoint", "grpc", "default", "1.0"), _grpcEndpoint
            );

            _client1.Configure(grpcConfig);
            _client2.Configure(grpcConfig);

            _grpcEndpoint.Configure(grpcConfig);

            _service1.SetReferences(references);
            _service2.SetReferences(references);

            _grpcEndpoint.OpenAsync(null).Wait();

            _client1.OpenAsync(null).Wait();
            _client2.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            _service1.CloseAsync(null).Wait();
            _service2.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task It_Should_Perform_CRUD_Operations()
        {
            It_Should_Be_OpenedAsync();

            await It_Should_Create_DummyAsync(_client1);
            await It_Should_Create_DummyAsync(_client2);

            await It_Should_Get_DummyAsync(_client1);
            await It_Should_Get_DummyAsync(_client2);
        }

        private void It_Should_Be_OpenedAsync()
        {
            Assert.True(_grpcEndpoint.IsOpen());
        }

        private async Task It_Should_Create_DummyAsync(IDummyClient client)
        {
            var newDummy = new Dummy("1", "Key 1", "Content 1");

            var resultDummy = await client.CreateAsync(Context.FromTraceId(IdGenerator.NextLong()), newDummy);

            Assert.NotNull(resultDummy);
            Assert.Equal(newDummy.Key, resultDummy.Key);
            Assert.Equal(newDummy.Content, resultDummy.Content);
        }

        private async Task It_Should_Get_DummyAsync(IDummyClient client)
        {
            var existingDummy = new Dummy("1", "Key 1", "Content 1");

            var resultDummy = await client.GetOneByIdAsync(Context.FromTraceId(IdGenerator.NextLong()), existingDummy.Id);

            Assert.NotNull(resultDummy);
            Assert.Equal(existingDummy.Key, resultDummy.Key);
            Assert.Equal(existingDummy.Content, resultDummy.Content);
        }
    }
}
