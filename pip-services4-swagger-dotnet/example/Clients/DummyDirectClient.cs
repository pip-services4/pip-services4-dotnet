using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Data.Query;
using PipServices4.Rpc.Clients;
using System.Threading.Tasks;

namespace PipServices4.Swagger.Clients
{
    public class DummyDirectClient : DirectClient<IDummyController>, IDummyClient
    {
        public DummyDirectClient()
        {
            _dependencyResolver.Put("controller", new Descriptor("pip-services4-dummies", "controller", "*", "*", "*"));
        }

        public async Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            filter = filter ?? new FilterParams();
            paging = paging ?? new PagingParams();

            using (var timing = Instrument(context, "dummy.get_page_by_filter"))
            {
                return await _controller.GetPageByFilterAsync(context, filter, paging);
            }
        }

        public async Task<Dummy> GetOneByIdAsync(IContext context, string id)
        {
            using (var timing = Instrument(context, "dummy.get_one_by_id"))
            {
                return await _controller.GetOneByIdAsync(context, id);
            }
        }

        public async Task<Dummy> CreateAsync(IContext context, Dummy entity)
        {
            using (var timing = Instrument(context, "dummy.create"))
            {
                return await _controller.CreateAsync(context, entity);
            }
        }

        public async Task<Dummy> UpdateAsync(IContext context, Dummy entity)
        {
            using (var timing = Instrument(context, "dummy.update"))
            {
                return await _controller.UpdateAsync(context, entity);
            }
        }

        public async Task<Dummy> DeleteByIdAsync(IContext context, string id)
        {
            using (var timing = Instrument(context, "dummy.delete_by_id"))
            {
                return await _controller.DeleteByIdAsync(context, id);
            }
        }

        public async Task RaiseExceptionAsync(IContext context)
        {
            await _controller.RaiseExceptionAsync(context);
        }
    }
}
