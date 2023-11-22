using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipServices4.Commons.Data;
using PipServices4.Commons.Reflect;
using PipServices4.Components.Context;
using PipServices4.Data.Data;
using PipServices4.Data.Keys;
using PipServices4.Persistence.Read;
using PipServices4.Persistence.Write;

namespace PipServices4.Persistence.Persistence
{
    /// <summary>
    /// Abstract persistence component that stores data in memory
    /// and implements a number of CRUD operations over data items with unique ids.
    /// The data items must implement IIdentifiable interface.
    /// 
    /// In basic scenarios child classes shall only override <c>GetPageByFilter()</c>,
    /// <c>GetListByFilter()</c> or <c>DeleteByFilter()</c> operations with specific filter function.
    /// All other operations can be used out of the box.
    /// 
    /// In complex scenarios child classes can implement additional operations by
    /// accessing cached items via <c>this._items</c> property and calling <c>Save()</c> method on updates.
    /// 
    /// ### References ###
    /// 
    /// - *:logger:*:*:1.0         (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <example>
    /// <code>
    /// class MyMemoryPersistence: IdentifiableMemoryPersistence<MyData, string> 
    /// {
    ///     public MyFilePersistence(string path)
    ///     {
    ///         base(MyData.class, new JsonPersister(path));
    ///         
    ///     private List<Func<MyData, bool>> ComposeFilter(FilterParams filter)
    ///     {
    ///         filter = filter != null ? filter : new FilterParams();
    ///         String name = filter.getAsNullableString("name");
    ///         return List<Func<MyData, bool>>() {
    ///         (item) => {
    ///         if (name != null && item.name != name)
    ///             return false;
    ///         return true;
    ///         }
    ///         };
    ///     }
    ///     
    ///     public DataPage<MyData> GetPageByFilter(IContext context, FilterParams filter, PagingParams paging)
    ///     {
    ///         base.GetPageByFilter(context, this.composeFilter(filter), paging, null, null);
    ///     }
    /// }
    /// 
    /// var persistence = new MyMemoryPersistence("./data/data.json");
    /// var item = persistence.Create("123", new MyData("1", "ABC"));
    /// var mydata = persistence.GetPageByFilter(
    /// "123",
    /// FilterParams.fromTuples("name", "ABC"),
    /// null, null, null);
    /// Console.Out.WriteLine(page.Data);          // Result: { id: "1", name: "ABC" }
    /// persistence.DeleteById("123", "1");
    /// ...
    /// </code>
    /// </example>
    public class IdentifiableMemoryPersistence<T, K> : MemoryPersistence<T>, IWriter<T, K>, IGetter<T, K>, ISetter<T>
        where T : IIdentifiable<K>
        where K : class
    {

        /// <summary>
        /// Creates a new instance of the persistence.
        /// </summary>
        public IdentifiableMemoryPersistence()
            : this(null, null)
        { }

        /// <summary>
        /// Creates a new instance of the persistence.
        /// </summary>
        /// <param name="loader">(optional) a loader to load items from external datasource.</param>
        /// <param name="saver">(optional) a saver to save items to external datasource.</param>
        protected IdentifiableMemoryPersistence(ILoader<T> loader, ISaver<T> saver)
            : base(loader, saver)
        { }

        /// <summary>
        /// Gets a list of data items retrieved by given unique ids.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="ids">ids of data items to be retrieved</param>
        /// <returns>a data list.</returns>
        public async Task<List<T>> GetListByIdsAsync(IContext context, K[] ids)
        {
            _lock.EnterReadLock();

            try
            {
                var filteredItems = _items.FindAll(x => ids.Contains(x.Id));

                _logger.Trace(context, $"Retrieved {filteredItems.Count} items");

                return await Task.FromResult(filteredItems);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets a data item by its unique id.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="id">an id of data item to be retrieved.</param>
        /// <returns>data item.</returns>
        public Task<T> GetOneByIdAsync(IContext context, K id)
        {
            _lock.EnterReadLock();

            try
            {
                var item = _items.FirstOrDefault(x => x.Id.Equals(id));

                if (item != null)
                    _logger.Trace(context, "Retrieved {0} by {1}", item, id);
                else
                    _logger.Trace(context, "Cannot find {0} by {1}", _typeName, id);

                return Task.FromResult(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Creates a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">an item to be created.</param>
        /// <returns>created item.</returns>
        public override async Task<T> CreateAsync(IContext context, T item)
        {
            var identifiable = item as IStringIdentifiable;
            if (identifiable != null && item.Id == null)
                ObjectWriter.SetProperty(item, nameof(item.Id), IdGenerator.NextLong());

            return await base.CreateAsync(context, item);
        }

        /// <summary>
        /// Sets a data item. If the data item exists it updates it, otherwise it create a new data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">a item to be set.</param>
        /// <returns>updated item.</returns>
        public async Task<T> SetAsync(IContext context, T item)
        {
            var identifiable = item as IStringIdentifiable;
            if (identifiable != null && item.Id == null)
                ObjectWriter.SetProperty(item, nameof(item.Id), IdGenerator.NextLong());

            _lock.EnterWriteLock();

            try
            {
                var index = _items.FindIndex(x => x.Id == item.Id);

                if (index < 0) _items.Add(item);
                else _items[index] = item;

                _logger.Trace(context, "Set {0}", item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            await SaveAsync(context);

            return item;
        }

        /// <summary>
        /// Updates a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">an item to be updated.</param>
        /// <returns>updated item.</returns>
        public async Task<T> UpdateAsync(IContext context, T item)
        {
            _lock.EnterWriteLock();

            try
            {
                var index = _items.FindIndex(x => x.Id.Equals(item.Id));

                if (index < 0)
                    return default(T);

                _items[index] = item;

                _logger.Trace(context, "Updated {0}", item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            await SaveAsync(context);

            return item;
        }

        /// <summary>
        /// Updates only few selected fields in a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="id">an id of data item to be updated.</param>
        /// <param name="data">a map with fields to be updated.</param>
        /// <returns>updated item.</returns>
        public async Task<T> UpdatePartiallyAsync(IContext context, K id, AnyValueMap data)
        {
            _lock.EnterWriteLock();

            object item;

            try
            {
                var index = _items.FindIndex(x => x.Id.Equals(id));

                if (index < 0)
                    return default(T);

                item = _items[index];

                var properties = ObjectReader.GetProperties(data.GetAsObject());
                ObjectWriter.SetProperties(item, properties);

                _items[index] = (T)item;

                _logger.Trace(context, "Partially updated item {0}", id);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            await SaveAsync(context);

            return (T)item;
        }

        /// <summary>
        /// Deleted a data item by it's unique id.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="id">an id of the item to be deleted</param>
        /// <returns>deleted item.</returns>
        public async Task<T> DeleteByIdAsync(IContext context, K id)
        {
            _lock.EnterWriteLock();

            T item = default(T);

            try
            {
                item = _items.Find(x => x.Id.Equals(id));

                if (item == null)
                    return default(T);

                _items.Remove(item);

                _logger.Trace(context, "Deleted {0}", item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            await SaveAsync(context);

            return await Task.FromResult(item);
        }

        /// <summary>
        /// Deletes multiple data items by their unique ids.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="ids">ids of data items to be deleted.</param>
        public async Task DeleteByIdsAsync(IContext context, K[] ids)
        {
            var deleted = false;

            _lock.EnterWriteLock();

            try
            {
                var filteredItems = _items.FindAll(x => ids.Contains(x.Id));
                deleted = filteredItems.Count > 0;

                foreach (var item in filteredItems)
                {
                    _items.Remove(item);
                }

                _logger.Trace(context, $"Deleted {filteredItems.Count} items");
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            if (deleted)
                await SaveAsync(context);
        }

    }
}