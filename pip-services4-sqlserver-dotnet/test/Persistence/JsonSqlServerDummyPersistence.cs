using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Sqlserver.Persistence;
using PipServices4.SqlServer.Test.Fixtures;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.SqlServer.Test.Persistence
{
    public class JsonSqlServerDummyPersistence: IdentifiableJsonSqlServerPersistence<Dummy, string>, IDummyPersistence
    {
        public JsonSqlServerDummyPersistence()
            : base("dummies_json")
        {
        }


        protected override void DefineSchema()
        {
            ClearSchema();
            EnsureTable();
            EnsureSchema("ALTER TABLE [dummies_json] ADD [data_key] AS JSON_VALUE([data],'$.key')");
            EnsureIndex($"{_tableName}_key", new Dictionary<string, bool> { { "data_key", true } }, new IndexOptions { Unique = true });
        }

        public async Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            return await base.GetPageByFilterAsync(context, ComposeFilter(filter), paging, null, null);
        }

        public async Task<long> GetCountByFilterAsync(IContext context, FilterParams filter)
        {
            return await base.GetCountByFilterAsync(context, ComposeFilter(filter));
        }

        private string ComposeFilter(FilterParams filter)
        {
            filter ??= new FilterParams();
            var key = filter.GetAsNullableString("key");

            var filterCondition = "";
            if (key != null)
                filterCondition += "JSON_VALUE([data],'$.key')='" + key + "'";
            
            return filterCondition;
        }
    }
}
