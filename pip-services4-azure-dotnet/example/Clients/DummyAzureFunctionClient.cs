using PipServices4.Components.Context;
using PipServices4.Data.Query;
using System.Threading.Tasks;

namespace PipServices4.Azure.Clients
{
    public class DummyAzureFunctionClient : AzureFunctionClient, IDummyClient
    {
        public async Task<DataPage<Dummy>> GetDummiesAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            var response = await CallAsync<DataPage<Dummy>>("dummies.get_dummies", context, new { filter, paging });

            return response;
        }

        public async Task<Dummy> CreateDummyAsync(IContext context, Dummy dummy)
        {
            var response = await CallAsync<Dummy>("dummies.create_dummy", context, new { dummy });

            return response;
        }

        public async Task<Dummy> GetDummyByIdAsync(IContext context, string dummyId)
        {
            var response = await CallAsync<Dummy>("dummies.get_dummy_by_id", context, new { dummy_id = dummyId });

            if (response == null)
                return null;

            return response;
        }

        public async Task<Dummy> UpdateDummyAsync(IContext context, Dummy dummy)
        {
            var response = await CallAsync<Dummy>("dummies.update_dummy", context, new { dummy = dummy });

            return response as Dummy;
        }

        public async Task<Dummy> DeleteDummyAsync(IContext context, string dummyId)
        {
            var response = await CallAsync<Dummy>("dummies.delete_dummy", context, new { dummy_id = dummyId });

            return response;
        }
    }
}
