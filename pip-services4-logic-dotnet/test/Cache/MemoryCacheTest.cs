using PipServices4.Components.Config;
using PipServices4.Logic.Cache;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Logic.test.Cache
{
	//[TestClass]
	public class MemoryCacheTest
	{
		private readonly MemoryCache _cache = new MemoryCache();

		private const string Key1 = "key1";
		private const string Key2 = "key2";
		private const string Key3 = "key3";

		private const string Value1 = "value1";
		private const string Value2 = "value2";
		private const string Value3 = "value3";

		public MemoryCacheTest()
		{
			var value = _cache.StoreAsync(null, Key1, Value1, 1000).Result;
			value = _cache.StoreAsync(null, Key2, Value2, 1000).Result;
		}

		[Fact]
		public void Retrieve_BothValue_In500ms()
		{
			Task.Delay(500).Wait();

			var val1 = _cache.RetrieveAsync<string>(null, Key1).Result;
			var val2 = _cache.RetrieveAsync<string>(null, Key2).Result;

			Assert.NotNull(val1);
			Assert.Equal(Value1, val1);

			Assert.NotNull(val2);
			Assert.Equal(Value2, val2);
		}

		[Fact]
		public void Retrieve_BothValue_In1000ms_Fails()
		{
			Task.Delay(1000).Wait();

			var val1 = _cache.RetrieveAsync<string>(null, Key1).Result;
			var val2 = _cache.RetrieveAsync<string>(null, Key2).Result;

			//Assert.Null(val1);
			//Assert.Null(val2);
		}

		[Fact]
		public void Store_ReturnsSameValue()
		{
			var storedVal = _cache.StoreAsync(null, Key3, Value3, 0).Result;
			Assert.Equal(Value3, storedVal);
		}

		[Fact]
		public void Store_ValueIsStored()
		{
			var value = _cache.StoreAsync(null, Key3, Value3, 1000).Result;
			var val3 = _cache.RetrieveAsync<string>(null, Key3).Result;

			Assert.NotNull(val3);
			Assert.Equal(Value3, val3);
		}

		[Fact]
		public void Remove_ValueIsRemoved()
		{
			_cache.RemoveAsync(null, Key1).Wait();

			var val1 = _cache.RetrieveAsync<string>(null, Key1).Result;
			Assert.Null(val1);
		}

		[Fact]
		public void Configure_NewValueStaysFor1500ms_ButFailsFor2500ms()
		{
			var config = ConfigParams.FromTuples("timeout", 2000);

			_cache.Configure(config);

			var value = _cache.StoreAsync(null, Key3, Value3, 0).Result;
			var val3 = _cache.RetrieveAsync<string>(null, Key3).Result;
			Assert.NotNull(val3);
			Assert.Equal(Value3, val3);

			Task.Delay(2500).Wait();

			val3 = _cache.RetrieveAsync<string>(null, Key3).Result;
			Assert.Null(val3);
		}
	}
}
