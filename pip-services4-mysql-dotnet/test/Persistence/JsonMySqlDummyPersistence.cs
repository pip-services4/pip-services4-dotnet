using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Mysql.Persistence;
using PipServices4.MySql.Test.Fixtures;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.MySql.Test.Persistence
{
    public class JsonMySqlDummyPersistence: IdentifiableJsonMySqlPersistence<Dummy, string>, IDummyPersistence
    {
        public JsonMySqlDummyPersistence()
            : base("dummies_json")
        {
        }

        protected override void DefineSchema()
        {
            ClearSchema();
            EnsureTable();
            EnsureSchema($"ALTER TABLE `{_tableName}` ADD `data_key` VARCHAR(50) AS (JSON_UNQUOTE(`data`->\"$.key\"))");
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
                filterCondition += "data->\"$.key\"='" + key + "'";
            
            return filterCondition;
        }
    }
}
