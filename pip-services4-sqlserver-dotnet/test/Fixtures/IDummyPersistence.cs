using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Persistence.Read;
using PipServices4.Persistence.Write;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.SqlServer.Test.Fixtures
{
    public interface IDummyPersistence : IGetter<Dummy, string>, IWriter<Dummy, string>, IPartialUpdater<Dummy, string>
    {
        Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging);
        Task<long> GetCountByFilterAsync(IContext context, FilterParams filter);
        Task<List<Dummy>> GetListByIdsAsync(IContext context, string[] ids);
        Task<Dummy> SetAsync(IContext context, Dummy item);
        Task DeleteByIdsAsync(IContext context, string[] ids);
    }
}

