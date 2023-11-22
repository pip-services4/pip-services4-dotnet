
using MongoDB.Driver;
using PipServices4.Commons.Data;
using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Mongodb.Persistence;
using PipServices4.Mongodb.Test.Fixtures;
using System;
using System.Threading.Tasks;

namespace PipServices4.Mongodb.Test.Persistence
{
    public class PartitionMongoDbDummyPersistence : PartitionMongoDbPersistence<Dummy, string>, IDummyPersistence
    {
        public PartitionMongoDbDummyPersistence()
            : base("dummies_ex", PartitionHelper.PartitionKey)
        {
        }

        public async Task ClearAsync()
        {
            await ClearAsync(null);
        }

        public async Task<Dummy> DeleteAsync(IContext context, string id)
        {
            return await DeleteByIdAsync(context, id);
        }

        public async Task<DataPage<Dummy>> GetAsync(IContext context, FilterParams filter, PagingParams paging, SortParams sort)
        {
            return await GetPageByFilterAsync(context, ComposeFilter(filter), paging, ComposeSort(sort));
        }

        public async Task<DataPage<object>> GetAsync(IContext context, FilterParams filter, PagingParams paging, SortParams sort, ProjectionParams projection)
        {
            return await GetPageByFilterAndProjectionAsync(context, ComposeFilter(filter), paging, ComposeSort(sort), projection);
        }

        public async Task<Dummy> GetByIdAsync(IContext context, string id)
        {
            return await GetOneByIdAsync(context, id);
        }

        public async Task<object> GetByIdAsync(IContext context, string id, ProjectionParams projection)
        {
            return await GetOneByIdAsync(context, id, projection);
        }

        public async Task<Dummy> ModifyAsync(IContext context, string id, AnyValueMap updateMap)
        {
            return await ModifyByIdAsync(context, id, ComposeUpdate(updateMap));
        }

        protected override FilterDefinition<Dummy> ComposeFilter(FilterParams filterParams)
        {
            filterParams = filterParams ?? new FilterParams();

            var builder = Builders<Dummy>.Filter;
            var filter = builder.Empty;

            foreach (var filterKey in filterParams.Keys)
            {
                if (filterKey.Equals("ids"))
                {
                    filter &= builder.In(s => s.Id, ToArrayOfType<string>(filterParams.GetAsNullableString("ids")));
                    continue;
                }

                var filterParam = filterParams[filterKey];

                filter &= IsArray(filterParam) ? builder.In(filterKey, ToArrayOfType<string>(filterParam)) :
                    builder.Eq(filterKey, filterParam);
            }

            return filter;
        }

        protected static TT[] ToArrayOfType<TT>(string value)
        {
            if (value == null)
            {
                return null;
            }

            var items = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) as TT[];
            return (items != null && items.Length > 0) ? items : null;
        }

        protected static bool IsArray(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return value.Split(',').Length > 1;
        }

        protected override string GetPartitionKey(string id)
        {
            return PartitionHelper.GetValue(id);
        }
    }
}