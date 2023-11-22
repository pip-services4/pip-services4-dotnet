using MongoDB.Driver;
using PipServices4.Commons.Data;
using PipServices4.Components.Context;
using PipServices4.Data.Query;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Mongodb.Test.Fixtures
{
    public interface IDummyPersistence
    {
        Task<Dummy> ModifyAsync(IContext context, string id, AnyValueMap updateMap);
        Task<Dummy> DeleteAsync(IContext context, string id);
        Task<Dummy> GetByIdAsync(IContext context, string id);
        Task<object> GetByIdAsync(IContext context, string id, ProjectionParams projection);
        Task<DataPage<Dummy>> GetAsync(IContext context, FilterParams filter, PagingParams paging, SortParams sort);
        Task<DataPage<object>> GetAsync(IContext context, FilterParams filter, PagingParams paging, SortParams sort, ProjectionParams projection);
        Task ClearAsync();

        Task<Dummy> CreateAsync(IContext context, Dummy item);
        Task DeleteByFilterAsync(IContext context, FilterDefinition<Dummy> filterDefinition);
        Task<Dummy> DeleteByIdAsync(IContext context, string id);
        Task DeleteByIdsAsync(IContext context, string[] ids);
        Task<List<Dummy>> GetListByFilterAsync(IContext context, FilterDefinition<Dummy> filterDefinition, SortDefinition<Dummy> sortDefinition = null);
        Task<List<Dummy>> GetListByIdsAsync(IContext context, string[] ids);
        Task<Dummy> GetOneByIdAsync(IContext context, string id);
        Task<object> GetOneByIdAsync(IContext context, string id, ProjectionParams projection);
        Task<Dummy> GetOneRandomAsync(IContext context, FilterDefinition<Dummy> filterDefinition);
        Task<DataPage<object>> GetPageByFilterAndProjectionAsync(IContext context, FilterDefinition<Dummy> filterDefinition, PagingParams paging = null, SortDefinition<Dummy> sortDefinition = null, ProjectionParams projection = null);
        Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterDefinition<Dummy> filterDefinition, PagingParams paging = null, SortDefinition<Dummy> sortDefinition = null);
        Task<Dummy> ModifyAsync(IContext context, FilterDefinition<Dummy> filterDefinition, UpdateDefinition<Dummy> updateDefinition);
        Task<Dummy> ModifyByIdAsync(IContext context, string id, UpdateDefinition<Dummy> updateDefinition);
        Task<Dummy> SetAsync(IContext context, Dummy item);
        Task<Dummy> UpdateAsync(IContext context, Dummy item);
    }
}

