using PipServices4.Logic.Cache;
using Xunit;

namespace PipServices4.Logic.test.Cache
{
	//[TestClass]
	public sealed class NullCacheTest
	{
		private NullCache _cache;

		public NullCacheTest()
		{
			_cache = new NullCache();
		}

		[Fact]
		public void Retrieve_ReturnsNull()
		{
			var val = _cache.RetrieveAsync<object>(null, "key1").Result;
			Assert.Null(val);
		}

		[Fact]
		public void Store_ReturnsSameValue()
		{
			var key = "key1";
			var val = "value1";

			var storedVal = _cache.StoreAsync(null, key, val, 0).Result;
			Assert.Equal(val, storedVal);
		}
	}
}
