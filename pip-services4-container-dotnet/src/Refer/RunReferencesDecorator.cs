using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Container.Refer
{
	/// <summary>
	/// References decorator that automatically opens to newly added components
	/// that implement <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_run_1_1_i_openable.html">IOpenable</a> interface and closes removed components
	/// that implement <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_run_1_1_i_closable.html">IClosable</a> interface.
	/// </summary>
	public class RunReferencesDecorator : ReferencesDecorator, IOpenable
	{
		private bool _opened = false;

		/// <summary>
		/// Creates a new instance of the decorator.
		/// </summary>
		/// <param name="baseReferences">the next references or decorator in the chain.</param>
		/// <param name="parentReferences">the decorator at the top of the chain.</param>
		public RunReferencesDecorator(IReferences baseReferences = null, IReferences parentReferences = null)
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
		/// Opens the component.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		public async Task OpenAsync(IContext context)
		{
			if (!_opened)
			{
				var components = base.GetAll();
				await Opener.OpenAsync(context, components);
				_opened = true;
			}
		}

		/// <summary>
		/// Closes component and frees used resources.
		/// </summary>
		/// <param name="context">(optional) execution context to trace execution through call chain.</param>
		public async Task CloseAsync(IContext context)
		{
			if (_opened)
			{
				var components = base.GetAll();
				await Closer.CloseAsync(context, components);
				_opened = false;
			}
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
				Opener.OpenOneAsync(null, component).Wait();
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
				Closer.CloseOneAsync(null, component).Wait();

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
				Closer.CloseAsync(null, components).Wait();

			return components;
		}

	}
}
