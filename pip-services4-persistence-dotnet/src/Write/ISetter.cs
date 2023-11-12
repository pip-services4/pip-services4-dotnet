using PipServices4.Components.Context;
using System.Threading.Tasks;

namespace PipServices4.Persistence.Write
{
    /// <summary>
    /// Interface for data processing components that can set (create or update) data items.
    /// </summary>
    /// <typeparam name="T">the class type</typeparam>
    public interface ISetter<T>
    {
        /// <summary>
        /// Sets a data item. If the data item exists it updates it, otherwise it create a new data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">a item to be set.</param>
        /// <returns>updated item.</returns>
        Task<T> SetAsync(IContext context, T item);
    }
}
