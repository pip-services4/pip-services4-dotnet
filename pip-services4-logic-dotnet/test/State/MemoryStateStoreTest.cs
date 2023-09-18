using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Logic.State
{
	public class MemoryStateStoreTest
	{
		private MemoryStateStore _cache;
		private StateStoreFixture _fixture;

		public MemoryStateStoreTest()
		{
			_cache = new MemoryStateStore();
			_fixture = new StateStoreFixture(_cache);
		}

		[Fact]
		public async Task TestSaveAndLoadAsync()
		{
			await _fixture.TestSaveAndLoadAsync();
		}

		[Fact]
		public async Task TestDeleteAsync()
		{
			await _fixture.TestDeleteAsync();
		}
	}
}
