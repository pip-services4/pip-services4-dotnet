using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Data.Query;
using PipServices4.Rpc.Clients;
using PipServices4.Rpc.Test.Sample;
using System.Threading.Tasks;

namespace PipServices3.Rpc.Clients
{
    public class DummyDirectClient : DirectClient<IDummyService>, IDummyClient
    {
        public DummyDirectClient()
        {
            _dependencyResolver.Put("service", new Descriptor("pip-services4-dummies", "service", "*", "*", "*"));
        }

        public async Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            filter = filter ?? new FilterParams();
            paging = paging ?? new PagingParams();

            using (var timing = Instrument(context, "dummy.get_page_by_filter"))
            {
                return await _service.GetPageByFilterAsync(context, filter, paging);
            }
        }

        public async Task<Dummy> GetOneByIdAsync(IContext context, string id)
        {
            using (var timing = Instrument(context, "dummy.get_one_by_id"))
            {
                return await _service.GetOneByIdAsync(context, id);
            }
        }

        public async Task<Dummy> CreateAsync(IContext context, Dummy entity)
        {
            using (var timing = Instrument(context, "dummy.create"))
            {
                return await _service.CreateAsync(context, entity);
            }
        }

        public async Task<Dummy> UpdateAsync(IContext context, Dummy entity)
        {
            using (var timing = Instrument(context, "dummy.update"))
            {
                return await _service.UpdateAsync(context, entity);
            }
        }

        public async Task<Dummy> DeleteByIdAsync(IContext context, string id)
        {
            using (var timing = Instrument(context, "dummy.delete_by_id"))
            {
                return await _service.DeleteByIdAsync(context, id);
            }
        }

        public async Task RaiseExceptionAsync(IContext context)
        {
            await _service.RaiseExceptionAsync(context);
        }

        public async Task<string> CheckCorrelationId(IContext context)
        {
            using (var timing = Instrument(context, "dummy.check_correlation_id"))
            {
                return await _service.CheckCorrelationId(context);
            }
        }
    }
}
