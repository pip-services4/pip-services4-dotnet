using PipServices4.Components.Context;
using System.Threading.Tasks;

namespace PipServices4.Persistence.Write
{
    /// <summary>
    /// Interface for data processing components that can create, update and delete data items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public interface IWriter<T, in K>
    {
        /// <summary>
        /// Creates a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">an item to be created.</param>
        /// <returns>created item.</returns>
        Task<T> CreateAsync(IContext context, T item);

        /// <summary>
        /// Updates a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">an item to be updated.</param>
        /// <returns>updated item.</returns>
        Task<T> UpdateAsync(IContext context, T item);

        /// <summary>
        /// Deleted a data item by it's unique id.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="id">an id of the item to be deleted</param>
        /// <returns>deleted item by unique id.</returns>
        Task<T> DeleteByIdAsync(IContext context, K id);
    }
}
