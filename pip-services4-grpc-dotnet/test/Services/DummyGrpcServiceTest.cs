using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Data.Keys;
using PipServices4.Data.Query;
using PipServices4.Grpc;
using PipServices4.Grpc.Clients;
using PipServices4.Grpc.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Grpc.Services
{
    [Collection("Sequential")]
    public class DummyGrpcServiceTest : IDisposable
    {
        private readonly DummyGrpcClient client;
        private readonly DummyGrpcService service;
        private readonly IContext context;

        public DummyGrpcServiceTest()
        {
            context = Context.FromTraceId(IdGenerator.NextLong());

            var config = ConfigParams.FromTuples(
                "connection.protocol", "http",
                "connection.host", "localhost",
                "connection.port", 3000
            );

            client = new DummyGrpcClient();
            client.Configure(config);

            service = new DummyGrpcService();
            service.Configure(config);

            var references = References.FromTuples(
                new Descriptor("pip-services4-dummies", "controller", "default", "default", "1.0"), new DummyController()
            );

            service.SetReferences(references);
            service.OpenAsync(null).Wait();

            client.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            client.CloseAsync(null).Wait();
            service.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task CRUD_Operations()
        {
            await It_Should_Be_Opened();
            await It_Should_Create_Dummy();
            await It_Should_Create_Dummy2();
            await It_Should_Update_Dummy2();
            await It_Should_Get_Dummy();
            await It_Should_Get_Dummies();
            await It_Should_Delete_Dummy();
        }

        private async Task It_Should_Delete_Dummy()
        {
            var existingDummy = new Dummy("1", "Key 1", "Content 1");

            var resultDummy = await client.DeleteByIdAsync(context, existingDummy.Id);

            Assert.NotNull(resultDummy);
            Assert.NotNull(resultDummy.Id);
            Assert.Equal(existingDummy.Key, resultDummy.Key);
            Assert.Equal(existingDummy.Content, resultDummy.Content);

            var result = await client.GetOneByIdAsync(context, existingDummy.Id);

            Assert.Null(result);
        }

        private async Task It_Should_Be_Opened()
        {
            Assert.True(service.IsOpen());
            Assert.True(client.IsOpen());
        }

        private async Task It_Should_Create_Dummy()
        {
            var newDummy = new Dummy("1", "Key 1", "Content 1");

            var resultDummy = await client.CreateAsync(context, newDummy);

            Assert.NotNull(resultDummy);
            Assert.NotNull(resultDummy.Id);
            Assert.Equal(newDummy.Key, resultDummy.Key);
            Assert.Equal(newDummy.Content, resultDummy.Content);
        }

        private async Task It_Should_Create_Dummy2()
        {
            var newDummy = new Dummy("2", "Key 2", "Content 2");

            var resultDummy = await client.CreateAsync(context, newDummy);

            Assert.NotNull(resultDummy);
            Assert.NotNull(resultDummy.Id);
            Assert.Equal(newDummy.Key, resultDummy.Key);
            Assert.Equal(newDummy.Content, resultDummy.Content);
        }
        
        private async Task It_Should_Update_Dummy2()
        {
            var dummy = new Dummy("2", "Key 2", "Content 3");

            var resultDummy = await client.UpdateAsync(context, dummy);

            Assert.NotNull(resultDummy);
            Assert.NotNull(resultDummy.Id);
            Assert.Equal(dummy.Key, resultDummy.Key);
            Assert.Equal(dummy.Content, resultDummy.Content);
        }

        private async Task It_Should_Get_Dummy()
        {
            var existingDummy = new Dummy("1", "Key 1", "Content 1");

            var resultDummy = await client.GetOneByIdAsync(context, existingDummy.Id);

            Assert.NotNull(resultDummy);
            Assert.NotNull(resultDummy.Id);
            Assert.Equal(existingDummy.Key, resultDummy.Key);
            Assert.Equal(existingDummy.Content, resultDummy.Content);
        }

        private async Task It_Should_Get_Dummies()
        {
            var existingDummy = new Dummy("1", "Key 1", "Content 1");
            
            var resultDummies = await client.GetPageByFilterAsync(context, FilterParams.FromTuples("key", existingDummy.Key), new PagingParams(0, 100));

            Assert.NotNull(resultDummies);
            Assert.NotNull(resultDummies.Data);
            Assert.Single(resultDummies.Data);
            
            resultDummies = await client.GetPageByFilterAsync(context, new FilterParams(), new PagingParams(0, 100));

            Assert.NotNull(resultDummies);
            Assert.NotNull(resultDummies.Data);
            Assert.Equal(2, resultDummies.Data.Count());
        }
    }
}