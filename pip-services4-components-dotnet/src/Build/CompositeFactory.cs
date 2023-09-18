using System;
using System.Collections.Generic;

namespace PipServices4.Components.Build
{
    /// <summary>
    /// Aggregates multiple factories into a single factory component.
    /// When a new component is requested, it iterates through
    /// factories to locate the one able to create the requested component.
    /// 
    /// This component is used to conveniently keep all supported factories in a single place.
    /// </summary>
    /// <example>
    /// <code>
    /// var factory = new CompositeFactory();
    /// factory.Add(new DefaultLoggerFactory());
    /// factory.Add(new DefaultCountersFactory());
    /// 
    /// var loggerLocator = new Descriptor("*", "logger", "*", "*", "1.0");
    /// factory.CanCreate(loggerLocator); 		// Result: Descriptor("pip-service", "logger", "null", "default", "1.0")
    /// factory.Create(loggerLocator); 			// Result: created NullLogger
    /// </code>
    /// </example>
    public class CompositeFactory : IFactory
    {
        private readonly List<IFactory> _factories = new List<IFactory>();

        /// <summary>
        /// Creates a new instance of the factory.
        /// </summary>
        public CompositeFactory() { }

        /// <summary>
        /// Creates a new instance of the factory.
        /// </summary>
        /// <param name="factories">a list of factories to embed into this factory.</param>
        public CompositeFactory(params IFactory[] factories)
        {
            if (factories != null)
            {
                _factories.AddRange(factories);
            }
        }

        /// <summary>
        /// Adds a factory into the list of embedded factories.
        /// </summary>
        /// <param name="factory">a factory to be added.</param>
        public void Add(IFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factories.Add(factory);
        }

        /// <summary>
        /// Removes a factory from the list of embedded factories.
        /// </summary>
        /// <param name="factory">the factory to remove.</param>
        public void Remove(IFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factories.Remove(factory);
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
            if (locator == null)
                throw new ArgumentNullException(nameof(locator));

            var factory = _factories.FindLast(x => x.CanCreate(locator) != null);

            return factory != null ? factory.CanCreate(locator) : null;
        }

        /// <summary>
        /// Creates a component identified by given locator.
        /// </summary>
        /// <param name="locator">a locator to identify component to be created.</param>
        /// <returns>the created component.</returns>
        public object Create(object locator)
        {
            if (locator == null)
                throw new ArgumentNullException(nameof(locator));

            var factory = _factories.FindLast(x => x.CanCreate(locator) != null);

            if (factory == null)
                throw new CreateException(null, locator);

            return factory.Create(locator);
        }
    }
}