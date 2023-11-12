using PipServices4.Components.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Persistence.Read
{
    /// <summary>
    /// Interface for data processing components that load data items.
    /// </summary>
    /// <typeparam name="T">the class type</typeparam>
    public interface ILoader<T>
    {
        /// <summary>
        /// Loads data items.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns>a list of data items.</returns>
        Task<List<T>> LoadAsync(IContext context);
    }
}
