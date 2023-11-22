//using PipServices3.Commons.Data;
//using PipServices3.Data;
//using PipServices3.MySql.Fixtures;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace PipServices4.MySql.Test.Fixtures
//{
//    public interface IDummy2Persistence : IGetter<Dummy2, Integer>, IWriter<Dummy2, Integer>, IPartialUpdater<Dummy2, Integer>
//    {
//        Task<DataPage<Dummy2>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging);
//        Task<long> GetCountByFilterAsync(IContext context, FilterParams filter);
//        Task<List<Dummy2>> GetListByIdsAsync(IContext context, Integer[] ids);
//        Task<Dummy2> SetAsync(IContext context, Dummy2 item);
//        Task DeleteByIdsAsync(IContext context, Integer[] ids);
//    }
//}

