using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Container.Refer
{
	/// <summary>
	/// References decorator that automatically sets references to newly added components
	/// that implement <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_refer_1_1_i_referenceable.html">IReferenceable</a> interface and unsets references from removed components
	/// that implement <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_refer_1_1_i_unreferenceable.html">IUnreferenceable</a> interface.
	/// </summary>
	public class LinkReferencesDecorator : ReferencesDecorator, IOpenable
	{
		private bool _opened = false;

		/// <summary>
		/// Creates a new instance of the decorator.
		/// </summary>
		/// <param name="baseReferences">the next references or decorator in the chain.</param>
		/// <param name="parentReferences">the decorator at the top of the chain.</param>
		public LinkReferencesDecorator(IReferences baseReferences = null, IReferences parentReferences = null)
			: base(baseReferences, parentReferences)
		{ }

		/// <summary>
		/// Checks if the component is opened.
		/// </summary>
		/// <returns>true if the component has been opened and false otherwise.</returns>
		public bool IsOpen()
		{
			return _opened;
		}

		/// <summary>
		/// true if the component has been opened and false otherwise.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		public async Task OpenAsync(string correlationId)
		{
			if (!_opened)
			{
				_opened = true;
				var components = base.GetAll();
				Referencer.SetReferences(this.ParentReferences, components);
			}

			await Task.Delay(0);
		}

		/// <summary>
		/// Closes component and frees used resources.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		public async Task CloseAsync(string correlationId)
		{
			if (_opened)
			{
				_opened = false;
				var components = base.GetAll();
				Referencer.UnsetReferences(components);
			}

			await Task.Delay(0);
		}

		/// <summary>
		/// Puts a new reference into this reference map.
		/// </summary>
		/// <param name="locator">a locator to find the reference by.</param>
		/// <param name="component">a component reference to be added.</param>
		public override void Put(object locator, object component)
		{
			base.Put(locator, component);

			if (_opened)
				Referencer.SetReferencesForOne(ParentReferences, component);
		}

		/// <summary>
		/// Removes a previously added reference that matches specified locator. If many
		/// references match the locator, it removes only the first one.When all
		/// references shall be removed, use removeAll() method instead.
		/// </summary>
		/// <param name="locator">a locator to remove reference</param>
		/// <returns>the removed component reference.</returns>
		public override object Remove(object locator)
		{
			var component = base.Remove(locator);

			if (_opened)
				Referencer.UnsetReferencesForOne(component);

			return component;
		}

		/// <summary>
		/// Removes all component references that match the specified locator.
		/// </summary>
		/// <param name="locator">the locator to remove references by.</param>
		/// <returns>a list, containing all removed references.</returns>
		public override List<object> RemoveAll(object locator)
		{
			var components = base.RemoveAll(locator);

			if (_opened)
				Referencer.UnsetReferences(components);

			return components;
		}

	}
}
