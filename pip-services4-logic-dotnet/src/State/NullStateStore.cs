using PipServices4.Components.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Logic.State
{
	/// <summary>
	/// Dummy state store implementation that doesn't do anything.
	/// 
	/// It can be used in testing or in situations when state management is not required
	/// but shall be disabled.
	/// 
	/// <see cref="ICache"/>
	/// </summary>
	public class NullStateStore : IStateStore
	{
		/// <summary>
		/// Loads state from the store using its key.
		/// If the value is missing in the store, it returns null.
		/// </summary>
		/// <typeparam name="T">The type of the state value.</typeparam>
		/// <param name="context">(optional) A context to trace execution through the call chain.</param>
		/// <param name="key">A unique state key.</param>
		/// <returns>The state value or <code>null</code> if the value wasn't found.</returns>
		public Task<T> LoadAsync<T>(IContext context, string key)
		{
			return Task.FromResult<T>(default);
		}

		/// <summary>
		/// Loads an array of states from the store using their keys.
		/// </summary>
		/// <typeparam name="T">The type of the state value.</typeparam>
		/// <param name="context">(optional) A context to trace execution through the call chain.</param>
		/// <param name="keys">Unique state keys.</param>
		/// <returns>An empty list of state values and their corresponding keys.</returns>
		public Task<List<StateValue<T>>> LoadBulkAsync<T>(IContext context, List<string> keys)
		{
			return Task.FromResult(new List<StateValue<T>>());
		}

		/// <summary>
		/// Saves state into the store.
		/// </summary>
		/// <typeparam name="T">The type of the state value.</typeparam>
		/// <param name="context">(optional) A context to trace execution through the call chain.</param>
		/// <param name="key">A unique state key.</param>
		/// <param name="value">A state value.</param>
		/// <returns>The state that was stored in the store.</returns>
		public Task<T> SaveAsync<T>(IContext context, string key, T value)
		{
			return Task.FromResult<T>(value);
		}

		/// <summary>
		/// Deletes a state from the store by its key.
		/// </summary>
		/// <typeparam name="T">The type of the state value.</typeparam>
		/// <param name="context">(optional) A context to trace execution through the call chain.</param>
		/// <param name="key">A unique value key.</param>
		public Task<T> DeleteAsync<T>(IContext context, string key)
		{
			return Task.FromResult<T>(default);
		}
	}
}
