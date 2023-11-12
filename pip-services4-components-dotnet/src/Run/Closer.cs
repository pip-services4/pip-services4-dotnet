using PipServices4.Components.Context;
using System.Collections;
using System.Threading.Tasks;

namespace PipServices4.Components.Run
{
    /// <summary>
    /// Helper class that closes previously opened components.
    /// </summary>
    /// See <see cref="IClosable"/>
    public class Closer
    {
        /// <summary>
        /// Closes specific component.
        /// To be closed components must implement IClosable interface. If they don't
        /// the call to this method has no effect.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="component">a list of components to be closed</param>
        /// See <see cref="IClosable"/>
        public static async Task CloseOneAsync(IContext context, object component)
        {
            var closable = component as IClosable;
            if (closable != null)
                await closable.CloseAsync(context);
        }

        /// <summary>
        /// Closes multiple components.
        /// To be closed components must implement IClosable interface. If they
        /// don't the call to this method has no effect.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="components">a list of components to be closed</param>
        /// See <see cref="CloseOneAsync(string, object)"/>, <see cref="IClosable"/>
        public static async Task CloseAsync(IContext context, IEnumerable components)
        {
            if (components == null) return;

            foreach (var component in components)
                await CloseOneAsync(context, component);
        }
    }
}
