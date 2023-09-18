namespace PipServices4.Components.Build
{
    /// <summary>
    /// Interface for component factories.
    /// 
    /// Factories use locators to identify components to be created.
    /// 
    /// The locators are similar to those used to locate components in references.
    /// They can be of any type like strings or integers.However Pip.Services toolkit
    /// most often uses Descriptor objects as component locators.
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Checks if this factory is able to create component by given locator.
        /// This method searches for all registered components and returns
        /// a locator for component it is able to create that matches the given locator.
        /// If the factory is not able to create a requested component is returns null.
        /// </summary>
        /// <param name="locater">a locator to identify component to be created.</param>
        /// <returns>a locator for a component that the factory is able to create.</returns>
        object CanCreate(object locater);

        /// <summary>
        /// Creates a component identified by given locator.
        /// </summary>
        /// <param name="locator">a locator to identify component to be created.</param>
        /// <returns>the created component.</returns>
        object Create(object locater);
    }
}