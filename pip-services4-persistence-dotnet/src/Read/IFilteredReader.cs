using System.Collections.Generic;
using PipServices4.Commons.Data;
using System.Threading.Tasks;
using PipServices4.Data.Query;
using PipServices4.Components.Context;

namespace PipServices4.Persistence.Read
{
    /// <summary>
    /// Interface for data processing components that can retrieve a list of data items by filter.
    /// </summary>
    /// <typeparam name="T">the class type</typeparam>
    public interface IFilteredReader<T>
        where T : class 
    {
        /// <summary>
        /// Gets a list of data items using filter parameters.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="filter">(optional) filter parameters</param>
        /// <param name="sort">(optional) sort parameters</param>
        /// <returns>list of filtered items.</returns>
        Task<List<T>> GetListByFilterAsync(IContext context, FilterParams filter, SortParams sort);
    }
}
