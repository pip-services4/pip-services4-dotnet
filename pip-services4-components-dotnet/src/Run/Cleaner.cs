using PipServices4.Components.Context;
using System.Collections;
using System.Threading.Tasks;

namespace PipServices4.Components.Run
{
    /// <summary>
    /// Helper class that cleans stored object state.
    /// </summary>
    /// See <see cref="ICleanable"/>
    public class Cleaner
    {
        /// <summary>
        /// Clears state of specific component.
        /// To be cleaned state components must implement ICleanable interface. If they
        /// don't the call to this method has no effect.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="component">a component to be cleaned</param>
        /// See <see cref="ICleanable"/>
        public static async Task ClearOneAsync(IContext context, object component)
        {
            var cleanable = component as ICleanable;
            if (cleanable != null)
                await cleanable.ClearAsync(context);
        }

        /// <summary>
        /// Clears state of multiple components.
        /// To be cleaned state components must implement ICleanable interface. If they
        /// don't the call to this method has no effect.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="components">a list of components to be cleaned</param>
        /// See <see cref="ClearOneAsync(string, object)"/>, <see cref="ICleanable"/>
        public static async Task ClearAsync(IContext context, IEnumerable components)
        {
            if (components == null) return;

            foreach (var component in components)
                await ClearOneAsync(context, component);
        }
    }
}
