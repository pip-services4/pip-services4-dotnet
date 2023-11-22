//using System.Collections.Generic;
//using System.Threading.Tasks;
//using PipServices3.Commons.Data;
//using PipServices3.MySql.Fixtures;

//namespace PipServices4.MySql.Test.Persistence
//{
//	public class MySql2DummyPersistence : IdentifiableMySqlPersistence<Dummy2, Integer>, IDummy2Persistence
//    {
//        public MySql2DummyPersistence()
//            : base("dummies2")
//        {
//        }

//        protected override void DefineSchema()
//        {
//            ClearSchema();
//            EnsureSchema($"CREATE TABLE `{_tableName}` (`id` INTEGER PRIMARY KEY, `key` VARCHAR(50), `content` TEXT, `create_time_utc` DATETIME, `sub_dummy` TEXT)");
//            EnsureIndex($"{_tableName}_key", new Dictionary<string, bool> { { "key", true } }, new IndexOptions { Unique = true });
//        }

//        public async Task<DataPage<Dummy2>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging)
//		{
//			return await base.GetPageByFilterAsync(context, ComposeFilter(filter), paging, null, null);
//		}

//		public async Task<long> GetCountByFilterAsync(IContext context, FilterParams filter)
//        {
//            return await base.GetCountByFilterAsync(context, ComposeFilter(filter));
//        }

//        private string ComposeFilter(FilterParams filter)
//        {
//            filter ??= new FilterParams();
//            var key = filter.GetAsNullableString("key");

//            var filterCondition = "";
//            if (key != null)
//                filterCondition += string.Format("{0}='{1}'", QuoteIdentifier("key"), key);
//            return filterCondition;
//        }
//    }
//}