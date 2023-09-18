using PipServices4.Commons.Data;
using PipServices4.Components.Config;
using PipServices4.Components.Exec;
using System.Collections;
using System.Collections.Generic;

namespace PipServices4.Components.Context
{
	/// <summary>
	/// Basic implementation of an execution context.
	/// <see cref="IContext"/>
	/// <see cref="AnyValueMap"/>
	/// </summary>
	public class Context : IContext
	{
		private readonly AnyValueMap _values;

		public object Get(string key)
		{
			return _values.Get(key);
		}

		/// <summary>
		/// Creates a new instance of the map and assigns its value.
		/// </summary>
		/// <param name="values">(optional) values to initialize this map.</param>
		public Context(IDictionary values = null)
		{
			_values = new AnyValueMap(values);
		}

		public Context(IDictionary<string, object> values)
		{
			_values = new AnyValueMap(values);
		}

		public Context(AnyValueMap values)
		{
			_values = values;
		}

		/// <summary>
		/// Creates a new Context object filled with key-value pairs from specified object.
		/// </summary>
		/// <param name="values">an object with key-value pairs used to initialize a new Context.</param>
		/// <returns>a new Context object.</returns>
		public static Context FromValue(IDictionary<string, object> values) 
		{
			return new Context(values);
		}

		/// <summary>
		/// Creates a new Context object filled with provided key-value pairs called tuples.
		/// Tuples parameters contain a sequence of key1, value1, key2, value2, ... pairs.
		/// </summary>
		/// <param name="tuples">the tuples to fill a new Parameters object.</param>
		/// <returns>a new Parameters object.</returns>
		public static Context FromTuples(params object[] tuples)
		{
			var map = AnyValueMap.FromTuples(tuples);
			return new Context(map);
		}

		/// <summary>
		/// Creates new Context from ConfigMap object.
		/// </summary>
		/// <param name="config">a ConfigParams that contain parameters.</param>
		/// <returns>a new Context object.</returns>
		public static Context FromConfig(ConfigParams config)
		{
			if (config == null)
				return new Context();

			var values = new AnyValueMap();
			foreach (var pair in config) {
				values[pair.Key] = pair.Value;
			}

			return new Context(values);
		}

		/// <summary>
		/// Creates new Context from trace id.
		/// </summary>
		/// <param name="traceId">a transaction id to trace execution through call chain.</param>
		/// <returns>a new Context object.</returns>
		public static Context FromTraceId(string traceId)
		{
			var map = Parameters.FromTuples("trace_id", traceId);
			return new Context(map);
		}
	}
}
