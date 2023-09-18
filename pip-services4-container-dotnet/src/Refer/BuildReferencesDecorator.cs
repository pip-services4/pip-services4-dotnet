using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using System;
using System.Collections.Generic;

namespace PipServices4.Container.Refer
{
	/// <summary>
	/// References decorator that automatically creates missing components using
	/// available component factories upon component retrival.
	/// </summary>
	public class BuildReferencesDecorator : ReferencesDecorator
	{
		/// <summary>
		/// Creates a new instance of the decorator.
		/// </summary>
		/// <param name="baseReferences">the next references or decorator in the chain.</param>
		/// <param name="parentReferences">the decorator at the top of the chain.</param>
		public BuildReferencesDecorator(IReferences baseReferences = null, IReferences parentReferences = null)
			: base(baseReferences, parentReferences)
		{ }

		/// <summary>
		/// Finds a factory capable creating component by given descriptor from the
		/// components registered in the references.
		/// </summary>
		/// <param name="locator">a locator of component to be created.</param>
		/// <returns>found factory or null if factory was not found.</returns>
		public IFactory FindFactory(object locator)
		{
			foreach (var component in GetAll())
			{
				var factory = component as IFactory;
				if (factory != null)
				{
					if (factory.CanCreate(locator) != null)
						return factory;
				}
			}

			return null;
		}

		/// <summary>
		/// Creates a component identified by given locator.
		/// </summary>
		/// <param name="locator">a locator to identify component to be created.</param>
		/// <param name="factory">a factory that shall create the component.</param>
		/// <returns>the created component.</returns>
		public object Create(object locator, IFactory factory)
		{
			// Find factory
			if (factory == null) return null;

			try
			{
				// Create component
				return factory.Create(locator);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Clarifies a component locator by merging two descriptors into one to replace
		/// missing fields.That allows to get a more complete descriptor that includes all possible fields.
		/// </summary>
		/// <param name="locator">a component locator to clarify.</param>
		/// <param name="factory">a factory that shall create the component.</param>
		/// <returns>clarified component descriptor (locator)</returns>
		public object ClarifyLocator(object locator, IFactory factory)
		{
			if (factory == null) return locator;
			if (!(locator is Descriptor)) return locator;

			object anotherLocator = factory.CanCreate(locator);
			if (anotherLocator == null) return locator;
			if (!(anotherLocator is Descriptor)) return locator;

			Descriptor descriptor = (Descriptor)locator;
			Descriptor anotherDescriptor = (Descriptor)anotherLocator;

			return new Descriptor(
				descriptor.Group != null ? descriptor.Group : anotherDescriptor.Group,
				descriptor.Type != null ? descriptor.Type : anotherDescriptor.Type,
				descriptor.Kind != null ? descriptor.Kind : anotherDescriptor.Kind,
				descriptor.Name != null ? descriptor.Name : anotherDescriptor.Name,
				descriptor.Version != null ? descriptor.Version : anotherDescriptor.Version
			);
		}

		/// <summary>
		/// Gets all component references that match specified locator.
		/// </summary>
		/// <typeparam name="T">the class type</typeparam>
		/// <param name="locator">the locator to find a reference by.</param>
		/// <param name="required">forces to raise an exception if no reference is found.</param>
		/// <returns>a list with matching component references.</returns>
		public override List<T> Find<T>(object locator, bool required)
		{
			var components = base.Find<T>(locator, false);

			// Try to create component
			if (required && components.Count == 0)
			{
				var factory = FindFactory(locator);
				var component = Create(locator, factory);
				if (component is T)
				{
					locator = ClarifyLocator(locator, factory);
					ParentReferences.Put(locator, component);
					components.Add((T)component);
				}
			}

			// Throw exception is no required components found
			if (required && components.Count == 0)
				throw new ReferenceException(locator);

			return components;
		}
	}
}
