using System.Threading.Tasks;
using PipServices4.Commons.Data;
using PipServices4.Components.Context;

namespace PipServices4.Persistence.Write
{
    /// <summary>
    /// Interface for data processing components to update data items partially.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public interface IPartialUpdater<T, in K>
    {
        /// <summary>
        /// Updates only few selected fields in a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="id">an id of data item to be updated.</param>
        /// <param name="data">a map with fields to be updated.</param>
        /// <returns>updated item.</returns>
        Task<T> UpdatePartially(IContext context, K id, AnyValueMap data);
    }
}
