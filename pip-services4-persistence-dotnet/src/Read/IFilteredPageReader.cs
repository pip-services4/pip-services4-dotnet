using PipServices4.Components.Context;
using PipServices4.Data.Query;
using System.Threading.Tasks;

namespace PipServices4.Persistence.Read
{
    /// <summary>
    /// Interface for data processing components that can retrieve a page of data items by a filter.
    /// </summary>
    /// <typeparam name="T">the class type</typeparam>
    public interface IFilteredPageReader<T>
         where T : class
    {
        /// <summary>
        /// Gets a page of data items using filter parameters.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="filter">(optional) filter parameters</param>
        /// <param name="paging">(optional) paging parameters</param>
        /// <param name="sort">(optional) sort parameters</param>
        /// <returns>list of filtered items.</returns>
        Task<DataPage<T>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging, SortParams sort);
    }
}
