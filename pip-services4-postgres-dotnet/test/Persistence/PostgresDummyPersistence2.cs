using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Postgres.Persistence;
using PipServices4.Postgres.Test.Fixtures;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Postgres.Test.Persistence
{
    public class PostgresDummyPersistence2 : IdentifiablePostgresPersistence<Dummy2, long>, IDummyPersistence2
    {
        public PostgresDummyPersistence2()
            : base("dummies2")
        {
            _autoGenerateId = false;
        }

		protected override void DefineSchema()
		{
            ClearSchema();
            EnsureSchema($"CREATE TABLE {_tableName} (id NUMERIC PRIMARY KEY, key TEXT, content TEXT, create_time_utc TIMESTAMP with time zone, sub_dummy JSONB)");
            EnsureIndex($"{_tableName}_key", new Dictionary<string, bool> { { "key", true } }, new IndexOptions { Unique = true });
        }

		//protected override Dummy OnConvertToPublic(AnyValueMap map)
		//{
		//          Dummy dummy = new Dummy();

		//          ObjectWriter.SetProperties(dummy, map);

		//          if (map.TryGetValue(nameof(dummy.SubDummy), out object subDummyJson))
		//          {
		//              var subDummyMap = new AnyValueMap(JsonConverter.ToMap(subDummyJson.ToString()));
		//              dummy.SubDummy = ConvertSubDummyToPublic(subDummyMap);
		//          }

		//          return dummy;
		//}

		//      private SubDummy ConvertSubDummyToPublic(AnyValueMap map)
		//      {
		//          SubDummy subDummy = new SubDummy();
		//          subDummy.Type = map.GetAsNullableString(nameof(subDummy.Type));

		//          var arrayOfDouble = map.GetAsObject(nameof(subDummy.ArrayOfDouble)) as IEnumerable<object>;
		//          subDummy.ArrayOfDouble = arrayOfDouble.Select(x => System.Convert.ToDouble(x)).ToArray();
		//          return subDummy;
		//      }

		public async Task<DataPage<Dummy2>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging)
        {
            filter ??= new FilterParams();
            var key = filter.GetAsNullableString("key");
            
            var filterCondition = "";
            if (key != null)
                filterCondition += "key='" + key + "'";

            return await base.GetPageByFilterAsync(context, filterCondition, paging, null, null);
        }

        public async Task<long> GetCountByFilterAsync(IContext context, FilterParams filter)
        {
            filter ??= new FilterParams();
            var key = filter.GetAsNullableString("key");

            var filterCondition = "";
            if (key != null)
                filterCondition += "key='" + key + "'";

            return await base.GetCountByFilterAsync(context, filterCondition);
        }

    }
}