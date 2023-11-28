using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Persistence.Read;
using PipServices4.Persistence.Write;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Postgres.Test.Fixtures
{
    public interface IDummyPersistence2 : IGetter<Dummy2, long>, IWriter<Dummy2, long>, IPartialUpdater<Dummy2, long>
    {
        Task<DataPage<Dummy2>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging);
        Task<long> GetCountByFilterAsync(IContext context, FilterParams filter);
        Task<List<Dummy2>> GetListByIdsAsync(IContext context, long[] ids);
        Task<Dummy2> SetAsync(IContext context, Dummy2 item);
        Task DeleteByIdsAsync(IContext context, long[] ids);
    }
}

