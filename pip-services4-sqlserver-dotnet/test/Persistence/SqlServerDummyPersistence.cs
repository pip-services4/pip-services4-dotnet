using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Sqlserver.Persistence;
using PipServices4.SqlServer.Test.Fixtures;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.SqlServer.Test.Persistence
{
    public class SqlServerDummyPersistence : IdentifiableSqlServerPersistence<Dummy, string>, IDummyPersistence
    {
        public SqlServerDummyPersistence()
            : base("dummies")
        {
        }

        protected override void DefineSchema()
        {
            ClearSchema();
            EnsureSchema($"CREATE TABLE [{_tableName}] ([id] VARCHAR(32) PRIMARY KEY, [key] VARCHAR(50), [content] VARCHAR(MAX), create_time_utc DATETIME, sub_dummy NVARCHAR(MAX))");
            EnsureIndex($"{_tableName}_key", new Dictionary<string, bool> { { "key", true } }, new IndexOptions { Unique = true });
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
                filterCondition += string.Format("{0}='{1}'", QuoteIdentifier("key"), key);
            return filterCondition;
        }
    }
}