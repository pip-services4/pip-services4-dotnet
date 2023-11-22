using PipServices4.Components.Context;
using PipServices4.Data.Query;
using System.Threading.Tasks;

namespace PipServices4.Http.Clients
{
    public sealed class DummyCommandableHttpClient : CommandableHttpClient, IDummyClient
    {
        public DummyCommandableHttpClient() 
            : base("dummy")
        { }

        public Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            filter = filter ?? new FilterParams();
            paging = paging ?? new PagingParams();

            var requestEntity = new
            {
                trace_id = ContextResolver.GetTraceId(context),
                filter,
                paging
            };

            return CallCommandAsync<DataPage<Dummy>>("get_dummies", context, requestEntity);
        }

        public Task<Dummy> GetOneByIdAsync(IContext context, string dummy_id)
        {
            var requestEntity = new
            {
                trace_id = ContextResolver.GetTraceId(context),
                dummy_id
            };

            return CallCommandAsync<Dummy>("get_dummy_by_id", context, requestEntity);
        }

        public Task<Dummy> CreateAsync(IContext context, Dummy dummy)
        {
            var requestEntity = new
            {
                trace_id = ContextResolver.GetTraceId(context),
                dummy
            };

            return CallCommandAsync<Dummy>("create_dummy", context, requestEntity);
        }

        public Task<Dummy> UpdateAsync(IContext context, Dummy dummy)
        {
            var requestEntity = new
            {
                trace_id = ContextResolver.GetTraceId(context),
                dummy
            };

            return CallCommandAsync<Dummy>("update_dummy", context, requestEntity);
        }

        public Task<Dummy> DeleteByIdAsync(IContext context, string dummy_id)
        {
            var requestEntity = new
            {
                trace_id = ContextResolver.GetTraceId(context),
                dummy_id
            };

            return CallCommandAsync<Dummy>("delete_dummy", context, requestEntity);
        }

        public Task RaiseExceptionAsync(IContext context)
        {
            return CallCommandAsync<object>("raise_exception", context, null);
        }

        public Task<string> CheckTraceId(IContext context)
        {
            return CallCommandAsync<string>("check_trace_id", context, new { trace_id = ContextResolver.GetTraceId(context) });
        }
    }
}
