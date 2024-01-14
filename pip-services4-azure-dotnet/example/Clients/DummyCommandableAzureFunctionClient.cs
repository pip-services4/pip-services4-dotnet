using PipServices4.Components.Context;
using PipServices4.Data.Query;
using System.Threading.Tasks;

namespace PipServices4.Azure.Clients
{
    public class DummyCommandableAzureFunctionClient : CommandableAzureFunctionClient, IDummyClient
    {
        public DummyCommandableAzureFunctionClient() : base("dummies")
        {

        }

        public async Task<DataPage<Dummy>> GetDummiesAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            return await CallAsync<DataPage<Dummy>>("dummies.get_dummies", context, new { filter, paging });
        }

        public async Task<Dummy> GetDummyByIdAsync(IContext context, string dummyId)
        {
            var response = await this.CallAsync<Dummy>("dummies.get_dummy_by_id", context, new { dummy_id = dummyId });

            if (response == null)
                return null;

            return response;
        }

        public async Task<Dummy> CreateDummyAsync(IContext context, Dummy dummy)
        {
            return await CallAsync<Dummy>("dummies.create_dummy", context, new { dummy });
        }

        public async Task<Dummy> UpdateDummyAsync(IContext context, Dummy dummy)
        {
            return await this.CallAsync<Dummy>("dummies.update_dummy", context, new { dummy });
        }

        public async Task<Dummy> DeleteDummyAsync(IContext context, string dummyId)
        {
            return await CallAsync<Dummy>("dummies.delete_dummy", context, new { dummy_id = dummyId });
        }
    }
}
