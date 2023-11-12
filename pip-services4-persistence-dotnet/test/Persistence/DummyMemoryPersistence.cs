using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using PipServices4.Commons.Data;
using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Persistence.Persistence;
using PipServices4.Persistence.Test.Sample;

namespace PipServices4.Persistence.Test.Persistence
{

    public class DummyMemoryPersistence : IdentifiableMemoryPersistence<Dummy, string>, IDummyPersistence
    {
        public DummyMemoryPersistence() : base()
        {
            _maxPageSize = 1000;
        }

        private List<Func<Dummy, bool>> ComposeFilter(FilterParams filter)
        {
            filter ??= new FilterParams();

            var id = filter.GetAsNullableString("id");
            var key = filter.GetAsNullableString("key");
            var ids = filter.GetAsNullableString("ids");
            var idsList = ids != null ? new List<string>(ids.Split(',')) : null;

            return new List<Func<Dummy, bool>>()
            {
                (item) =>
                {
                    if (id != null && item.Id != id)
                        return false;
                    if (key != null && item.Key != key)
                        return false;
                    if (idsList != null && idsList.IndexOf(item.Id) < 0)
                        return false;
                    return true;
                }
            };
        }

        public async Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            return await base.GetPageByFilterAsync(context, this.ComposeFilter(filter), paging);
        }


        public Task<Dummy> ModifyAsync(IContext context, string id, AnyValueMap updateMap)
        {
            throw new NotImplementedException();
        }

        public Task<Dummy> DeleteAsync(IContext context, string id)
        {
            throw new NotImplementedException();
        }

        public Task<Dummy> GetByIdAsync(IContext context, string id)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetByIdAsync(IContext context, string id, ProjectionParams projection)
        {
            throw new NotImplementedException();
        }

        public Task<DataPage<Dummy>> GetAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            throw new NotImplementedException();
        }

        public Task<DataPage<object>> GetAsync(IContext context, FilterParams filter, PagingParams paging, ProjectionParams projection)
        {
            throw new NotImplementedException();
        }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public Task<object> GetOneByIdAsync(IContext context, string id, ProjectionParams projection)
        {
            throw new NotImplementedException();
        }
    }
}
