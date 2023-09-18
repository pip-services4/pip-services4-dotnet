using PipServices4.Components.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Logic.State
{
	/// <summary>
	/// Interface for state storages that are used to store and retrieve transaction states.
	/// </summary>
	public interface IStateStore
	{
		/// <summary>
		/// Loads state from the store using its key.
		/// If value is missing in the store it returns null.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context">(optional) a context to trace execution through call chain.</param>
		/// <param name="key">a unique state key.</param>
		/// <returns>the state value or <code>null</code> if value wasn't found.</returns>
		Task<T> LoadAsync<T>(IContext context, string key);

		/// <summary>
		/// Loads an array of states from the store using their keys.
		/// </summary>
		/// <param name="context">(optional) a context to trace execution through call chain.</param>
		/// <param name="keys">unique state keys.</param>
		/// <returns>list with state values and their corresponding keys.</returns>
		Task<List<StateValue<T>>> LoadBulkAsync<T>(IContext context, List<string> keys);

		/// <summary>
		/// Saves state into the store.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context">(optional) a context to trace execution through call chain.</param>
		/// <param name="key">a unique state key.</param>
		/// <param name="value">a state value.</param>
		/// <returns></returns>
		Task<T> SaveAsync<T>(IContext context, string key, T value);

		/// <summary>
		/// Deletes a state from the store by its key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context">(optional) a context to trace execution through call chain.</param>
		/// <param name="key">a unique value key.</param>
		/// <returns></returns>
		Task<T> DeleteAsync<T>(IContext context, string key);
	}
}