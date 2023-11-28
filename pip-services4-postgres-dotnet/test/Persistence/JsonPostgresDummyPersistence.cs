using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Postgres.Persistence;
using PipServices4.Postgres.Test.Fixtures;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Postgres.Test.Persistence
{
    public class JsonPostgresDummyPersistence: IdentifiableJsonPostgresPersistence<Dummy, string>, IDummyPersistence
    {
        public JsonPostgresDummyPersistence()
            : base("dummies_json")
        {
        }

		protected override void DefineSchema()
		{
            ClearSchema();
            EnsureTable("VARCHAR(32)", "JSONB");
            EnsureIndex($"{_tableName}_key", new Dictionary<string, bool> { { "(data->>'key')", true } }, new IndexOptions { Unique = true });
        }

        public async Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging)
		{
			return await base.GetPageByFilterAsync(context, ComposeFilter(filter), paging, null, null);
		}

		public async Task<long> GetCountByFilterAsync(IContext context, FilterParams filter)
        {
            return await base.GetCountByFilterAsync(context, ComposeFilter(filter));
        }

        private static string ComposeFilter(FilterParams filter)
        {
            filter ??= new FilterParams();
            var key = filter.GetAsNullableString("key");

            var filterCondition = "";
            if (key != null)
                filterCondition += "data->>'key'='" + key + "'";
            return filterCondition;
        }
    }
}
