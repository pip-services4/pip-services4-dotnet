using PipServices4.Components.Refer;
using System.Collections.Generic;

namespace PipServices4.Container.Refer
{
	/// <summary>
	/// Chainable decorator for <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_refer_1_1_i_references.html">IReferences</a> that allows to inject additional capabilities
	/// such as automatic component creation, automatic registration and opening.
	/// </summary>
	/// See <see cref="IReferences"/>
	public class ReferencesDecorator : IReferences
	{
		/// <summary>
		/// Creates a new instance of the decorator.
		/// </summary>
		/// <param name="baseReferences">the next references or decorator in the chain.</param>
		/// <param name="parentReferences">the decorator at the top of the chain.</param>
		public ReferencesDecorator(IReferences baseReferences = null, IReferences parentReferences = null)
		{
			BaseReferences = baseReferences ?? parentReferences;
			ParentReferences = parentReferences ?? baseReferences;
		}

		/** The next references or decorator in the chain. */
		public IReferences BaseReferences { get; set; }
		/** The decorator at the top of the chain. */
		public IReferences ParentReferences { get; set; }

		/// <summary>
		/// Puts a new reference into this reference map.
		/// </summary>
		/// <param name="locator">a locator to find the reference by.</param>
		/// <param name="component">a component reference to be added.</param>
		public virtual void Put(object locator, object component)
		{
			BaseReferences.Put(locator, component);
		}

		/// <summary>
		/// Removes a previously added reference that matches specified locator. If many
		/// references match the locator, it removes only the first one.When all
		/// references shall be removed, use removeAll() method instead.
		/// </summary>
		/// <param name="locator">a locator to remove reference</param>
		/// <returns>the removed component reference.</returns>
		public virtual object Remove(object locator)
		{
			return BaseReferences.Remove(locator);
		}

		/// <summary>
		/// Removes all component references that match the specified locator.
		/// </summary>
		/// <param name="locator">the locator to remove references by.</param>
		/// <returns>a list, containing all removed references.</returns>
		public virtual List<object> RemoveAll(object locator)
		{
			return BaseReferences.RemoveAll(locator);
		}

		/// <summary>
		/// Gets locators for all registered component references in this reference map.
		/// </summary>
		/// <returns>a list with component locators.</returns>
		public virtual List<object> GetAllLocators()
		{
			return BaseReferences.GetAllLocators();
		}

		/// <summary>
		/// Gets all component references registered in this reference map.
		/// </summary>
		/// <returns>a list with component references.</returns>
		public virtual List<object> GetAll()
		{
			return BaseReferences.GetAll();
		}

		/// <summary>
		/// Gets an optional component reference that matches specified locator.
		/// </summary>
		/// <param name="locator">the locator to find references by.</param>
		/// <returns>a matching component reference or null if nothing was found.</returns>
		public virtual object GetOneOptional(object locator)
		{
			var components = Find<object>(locator, false);
			return components.Count > 0 ? components[0] : null;
		}

		/// <summary>
		/// Gets an optional component reference that matches specified locator.
		/// </summary>
		/// <typeparam name="T">the class type</typeparam>
		/// <param name="locator">the locator to find references by.</param>
		/// <returns>a matching component reference or null if nothing was found.</returns>
		public virtual T GetOneOptional<T>(object locator)
		{
			var components = Find<T>(locator, false);
			return components.Count > 0 ? components[0] : default(T);
		}

		/// <summary>
		/// Gets a required component reference that matches specified locator.
		/// </summary>
		/// <param name="locator">the locator to find a reference by.</param>
		/// <returns>a matching component reference.</returns>
		public virtual object GetOneRequired(object locator)
		{
			var components = Find<object>(locator, true);
			return components.Count > 0 ? components[0] : null;
		}

		/// <summary>
		/// Gets a required component reference that matches specified locator.
		/// </summary>
		/// <typeparam name="T">the class type</typeparam>
		/// <param name="locator">the locator to find a reference by.</param>
		/// <returns>a matching component reference.</returns>
		public virtual T GetOneRequired<T>(object locator)
		{
			var components = Find<T>(locator, true);
			return components.Count > 0 ? components[0] : default(T);
		}

		/// <summary>
		/// Gets all component references that match specified locator.
		/// </summary>
		/// <param name="locator">the locator to find references by.</param>
		/// <returns>a list with matching component references or empty list if nothing was found.</returns>
		public virtual List<object> GetOptional(object locator)
		{
			return Find<object>(locator, false);
		}

		/// <summary>
		/// Gets all component references that match specified locator.
		/// </summary>
		/// <typeparam name="T">the class type</typeparam>
		/// <param name="locator">the locator to find references by.</param>
		/// <returns>a list with matching component references or empty list if nothing was found.</returns>
		public virtual List<T> GetOptional<T>(object locator)
		{
			return Find<T>(locator, false);
		}

		/// <summary>
		/// Gets all component references that match specified locator. At least one
		/// component reference must be present.If it doesn't the method throws an error.
		/// </summary>
		/// <param name="locator">the locator to find references by.</param>
		/// <returns>a list with matching component references.</returns>
		public virtual List<object> GetRequired(object locator)
		{
			return Find<object>(locator, true);
		}

		/// <summary>
		/// Gets all component references that match specified locator. At least one
		/// component reference must be present.If it doesn't the method throws an error.
		/// </summary>
		/// <typeparam name="T">the class type</typeparam>
		/// <param name="locator">the locator to find references by.</param>
		/// <returns>a list with matching component references.</returns>
		public virtual List<T> GetRequired<T>(object locator)
		{
			return Find<T>(locator, true);
		}

		/// <summary>
		/// Gets all component references that match specified locator.
		/// </summary>
		/// <param name="locator">the locator to find a reference by.</param>
		/// <param name="required">forces to raise an exception if no reference is found.</param>
		/// <returns>a list with matching component references.</returns>
		public virtual List<object> Find(object locator, bool required)
		{
			return Find<object>(locator, required);
		}

		/// <summary>
		/// Gets all component references that match specified locator.
		/// </summary>
		/// <typeparam name="T">the class type</typeparam>
		/// <param name="locator">the locator to find a reference by.</param>
		/// <param name="required">forces to raise an exception if no reference is found.</param>
		/// <returns>a list with matching component references.</returns>
		public virtual List<T> Find<T>(object locator, bool required)
		{
			return BaseReferences.Find<T>(locator, required);
		}

	}
}
