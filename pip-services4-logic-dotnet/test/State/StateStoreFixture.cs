using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Logic.State
{
	public class StateStoreFixture
	{
		const string KEY1 = "key1";
		const string KEY2 = "key2";

		const string VALUE1 = "value1";
		const string VALUE2 = "value2";

		private readonly IStateStore _state;

		public StateStoreFixture(IStateStore state)
		{
			_state = state;
		}

		public async Task TestDeleteAsync()
		{
			await _state.SaveAsync(null, KEY1, VALUE1);
			await _state.SaveAsync(null, KEY2, VALUE2);

			var val = await _state.LoadAsync<string>(null, KEY1);

			Assert.NotNull(val);
			Assert.Equal(VALUE1, val);

			var values = await _state.LoadBulkAsync<string>(null, new List<string> { KEY2 });
			Assert.Single(values);
			Assert.Equal(KEY2, values[0].Key);
			Assert.Equal(VALUE2, values[0].Value);
		}

		public async Task TestSaveAndLoadAsync()
		{
			await _state.SaveAsync(null, KEY1, VALUE1);

			await _state.DeleteAsync<string>(null, KEY1);

			var val = await _state.LoadAsync<string>(null, KEY1);
			Assert.Null(val);
		}
	}
}
