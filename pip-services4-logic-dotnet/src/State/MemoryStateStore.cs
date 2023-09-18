using PipServices4.Components.Config;
using PipServices4.Components.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Logic.State
{
	/// <summary>
	/// State store that keeps states in the process memory.
	/// 
	/// Remember: This implementation is not suitable for synchronization of distributed processes.
	/// <see cref="IStateStore"/>
	/// </summary>
	public class MemoryStateStore : IStateStore, IReconfigurable
	{
		private readonly Dictionary<string, StateEntry> _states = new Dictionary<string, StateEntry>();
		private long _timeout = 0;

		/// <summary>
		/// Creates a new instance of the state store.
		/// </summary>
		public MemoryStateStore()
		{
			//
		}

		/// <summary>
		/// Configures the component by passing configuration parameters.
		/// </summary>
		/// <param name="config">Configuration parameters to be set.</param>
		public void Configure(ConfigParams config)
		{
			_timeout = config.GetAsLongWithDefault("options.timeout", _timeout);
		}

		/// <summary>
		/// Clears the component state.
		/// </summary>
		public void Cleanup(IContext context)
		{
			if (_timeout == 0) return;

			long cutOffTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _timeout;

			// Cleanup obsolete entries
			foreach (var pair in _states)
			{
				var entry = pair.Value;
				// Remove obsolete entry
				if (entry.LastUpdateTime < cutOffTime)
				{
					_states.Remove(pair.Key);
				}
			}
		}

		/// <summary>
		/// Loads a stored value from the store using its key.
		/// If the value is missing in the store, it returns null.
		/// </summary>
		/// <typeparam name="T">The type of the state value.</typeparam>
		/// <param name="context">(optional) A context to trace execution through the call chain.</param>
		/// <param name="key">A unique state key.</param>
		/// <returns>The state value or <code>null</code> if the value wasn't found.</returns>
		public Task<T> LoadAsync<T>(IContext context, string key)
		{
			if (key == null)
			{
				throw new Exception("Key cannot be null");
			}

			// Cleanup the stored states
			Cleanup(context);

			// Get entry from the store
			if (_states.TryGetValue(key, out var entry))
			{
				return Task.FromResult((T)entry.Value);
			}

			return Task.FromResult<T>(default);
		}

		/// <summary>
		/// Loads an array of states from the store using their keys.
		/// </summary>
		/// <typeparam name="T">The type of the state value.</typeparam>
		/// <param name="context">(optional) A context to trace execution through the call chain.</param>
		/// <param name="keys">Unique state keys.</param>
		/// <returns>An array with state values.</returns>
		public async Task<List<StateValue<T>>> LoadBulkAsync<T>(IContext context, List<string> keys)
		{
			// Cleanup the stored states
			Cleanup(context);

			var result = new List<StateValue<T>>();

			foreach (var key in keys)
			{
				var value = await LoadAsync<T>(context, key);
				result.Add(new StateValue<T> { Key = key, Value = value });
			}

			return result;
		}

		/// <summary>
		/// Saves state into the store.
		/// </summary>
		/// <typeparam name="T">The type of the state value.</typeparam>
		/// <param name="context">(optional) A context to trace execution through the call chain.</param>
		/// <param name="key">A unique state key.</param>
		/// <param name="value">A state value to store.</param>
		/// <returns>The value that was stored in the cache.</returns>
		public Task<T> SaveAsync<T>(IContext context, string key, T value)
		{
			if (key == null)
			{
				throw new Exception("Key cannot be null");
			}

			// Cleanup the stored states
			Cleanup(context);

			// Get the entry
			if (_states.TryGetValue(key, out var entry))
			{
				entry.Value = value;
			}
			// Or create a new entry
			else
			{
				entry = new StateEntry(key, value);
				_states[key] = entry;
			}

			return Task.FromResult(value);
		}

		/// <summary>
		/// Deletes a state from the store by its key.
		/// </summary>
		/// <typeparam name="T">The type of the state value.</typeparam>
		/// <param name="context">(optional) A context to trace execution through the call chain.</param>
		/// <param name="key">A unique state key.</param>
		/// <returns>The deleted state value.</returns>
		public Task<T> DeleteAsync<T>(IContext context, string key)
		{
			if (key == null)
			{
				throw new Exception("Key cannot be null");
			}

			// Cleanup the stored states
			Cleanup(context);

			// Get the entry
			if (_states.TryGetValue(key, out var entry))
			{
				_states.Remove(key);
				return Task.FromResult((T)entry.Value);
			}

			return Task.FromResult<T>(default);
		}
	}
}
