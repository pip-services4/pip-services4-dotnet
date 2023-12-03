using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Grpc.Clients;
using System.Threading.Tasks;

namespace PipServices4.Grpc.Clients
{
    public sealed class DummyCommandableGrpcClient : CommandableGrpcClient, IDummyClient
    {
        public DummyCommandableGrpcClient()
            : base()
        { }

        public DummyCommandableGrpcClient(string name) 
            : base(name)
        { }

        public Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            filter = filter ?? new FilterParams();
            paging = paging ?? new PagingParams();

            var requestEntity = new
            {
                context,
                filter,
                paging
            };

            return CallCommandAsync<DataPage<Dummy>>("get_dummies", context, requestEntity);
        }

        public Task<Dummy> GetOneByIdAsync(IContext context, string dummy_id)
        {
            var requestEntity = new
            {
                context,
                dummy_id
            };

            return CallCommandAsync<Dummy>("get_dummy_by_id", context, requestEntity);
        }

        public Task<Dummy> CreateAsync(IContext context, Dummy dummy)
        {
            var requestEntity = new
            {
                context,
                dummy
            };

            return CallCommandAsync<Dummy>("create_dummy", context, requestEntity);
        }

        public Task<Dummy> UpdateAsync(IContext context, Dummy dummy)
        {
            var requestEntity = new
            {
                context,
                dummy
            };

            return CallCommandAsync<Dummy>("update_dummy", context, requestEntity);
        }

        public Task<Dummy> DeleteByIdAsync(IContext context, string dummy_id)
        {
            var requestEntity = new
            {
                context,
                dummy_id
            };

            return CallCommandAsync<Dummy>("delete_dummy", context, requestEntity);
        }

    }
}
