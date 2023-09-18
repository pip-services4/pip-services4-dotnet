namespace PipServices4.Components.Context
{
	/// <summary>
	/// Interface to specify execution context.
	/// <see cref="Context"/>
	/// </summary>
	public interface IContext
	{
		/// <summary>
		/// Gets a map element specified by its key.
		/// </summary>
		/// <param name="key">a key of the element to get.</param>
		/// <returns>the value of the map element.</returns>
		object Get(string key);
	}
}
