using PipServices4.Components.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Persistence.Write
{
    /// <summary>
    /// Interface for data processing components that save data items.
    /// </summary>
    /// <typeparam name="T">the class type</typeparam>
    public interface ISaver<in T>
    {
        /// <summary>
        /// Saves given data items.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="items">a list of items to save.</param>
        Task SaveAsync(IContext context, IEnumerable<T> items);
    }
}
