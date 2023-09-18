using PipServices4.Commons.Reflect;
using System;
using System.Collections.Generic;

namespace PipServices4.Components.Build
{
    /// <summary>
    /// Basic component factory that creates components using registered types and factory functions.
    /// </summary>
    /// <example>
    /// <code>
    /// Factory factory = new Factory();
    /// 
    /// factory.RegisterAsType(
    /// new Descriptor("mygroup", "mycomponent1", "default", "*", "1.0"),
    /// MyComponent1 );
    /// 
    /// factory.Register(
    /// new Descriptor("mygroup", "mycomponent2", "default", "*", "1.0"),
    /// (locator) => {return new MyComponent2();
    /// });
    /// 
    /// factory.Create(new Descriptor("mygroup", "mycomponent1", "default", "name1", "1.0"))
    /// factory.Create(new Descriptor("mygroup", "mycomponent2", "default", "name2", "1.0"))
    /// </code>
    /// </example>
    public class Factory: IFactory
    {
        private class Registration
        {
            public Registration(Object locator, Func<object, object> factory)
            {
                Locator = locator;
                Factory = factory;
            }

            public object Locator { get; private set; }
            public Func<object, object> Factory { get; private set; }
        }

        private List<Registration> _registrations = new List<Registration>();

        /// <summary>
        /// Registers a component using a factory method.
        /// </summary>
        /// <param name="locator">a locator to identify component to be created.</param>
        /// <param name="factory">a factory function that receives a locator and returns a created component.</param>
        public void Register(object locator, Func<object, object> factory)
        {
            if (locator == null)
                throw new NullReferenceException("Locator cannot be null");
            if (factory == null)
                throw new NullReferenceException("Factory cannot be null");

            _registrations.Add(new Registration(locator, factory));
        }

        /// <summary>
        /// Registers a component using its type (a constructor function).
        /// </summary>
        /// <param name="locator">a locator to identify component to be created.</param>
        /// <param name="type">a component type.</param>
        public void RegisterAsType(object locator, Type type)
        {
            if (locator == null)
                throw new NullReferenceException("Locator cannot be null");
            if (type == null)
                throw new NullReferenceException("Type cannot be null");

            Func<object, object> factory = (_) =>
            {
                try
                {
                    return TypeReflector.CreateInstanceByType(type);
                }
                catch
                {
                    return null;
                }
            };
            _registrations.Add(new Registration(locator, factory));
        }

        /// <summary>
        /// Checks if this factory is able to create component by given locator.
        /// 
        /// This method searches for all registered components and returns a locator for
        /// component it is able to create that matches the given locator.If the factory
        /// is not able to create a requested component is returns null.
        /// </summary>
        /// <param name="locator">a locator to identify component to be created.</param>
        /// <returns>a locator for a component that the factory is able to create.</returns>
        public object CanCreate(object locator)
        {
            foreach (Registration registration in _registrations)
            {
                object thisLocator = registration.Locator;
                if (thisLocator.Equals(locator))
                    return thisLocator;
            }
            return null;
        }

        /// <summary>
        /// Creates a component identified by given locator.
        /// </summary>
        /// <param name="locator">a locator to identify component to be created.</param>
        /// <returns>the created component.</returns>
        public object Create(object locator)
        {
		    foreach (Registration registration in _registrations) {
                if (registration.Locator.Equals(locator))
                {
                    try
                    {
                        return registration.Factory(locator);
                    }
                    catch (Exception ex)
                    {
                        if (ex is CreateException)
						    throw;

                        throw (CreateException)new CreateException(
                            null,
                            "Failed to create object for " + locator
                        ).WithCause(ex);
                    }
                }
            }
		    return null;
        }
	
    }
}
