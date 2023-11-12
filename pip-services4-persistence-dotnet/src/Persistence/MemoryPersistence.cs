using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Data.Query;
using PipServices4.Observability.Log;
using PipServices4.Persistence.Read;
using PipServices4.Persistence.Write;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PipServices4.Persistence.Persistence
{
    /// <summary>
    /// Abstract persistence component that stores data in memory.
    /// 
    /// This is the most basic persistence component that is only
    /// able to store data items of any type.Specific CRUD operations
    /// over the data items must be implemented in child classes by
    /// accessing <c>this._items</c> property and calling <c>Save()</c> method.
    /// 
    /// The component supports loading and saving items from another data source.
    /// That allows to use it as a base class for file and other types
    /// of persistence components that cache all data in memory.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// options:
    /// - max_page_size:       Maximum number of items returned in a single page (default: 100)
    /// 
    /// ### References ###
    /// 
    /// - *:logger:*:*:1.0         (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// </summary>
    /// <typeparam name="T">the class type</typeparam>
    /// <example>
    /// <code>
    /// class MyMemoryPersistence extends MemoryPersistence<MyData>
    /// {
    ///     public MyData GetByName(IContext context, String name)
    ///     {
    ///         MyData item = _items.Find((mydata) => { return mydata.Name == name; });
    ///         ...
    ///         return item;
    ///     } 
    ///     public MyData Set(String correlatonId, MyData item)
    ///     {
    ///         this._items = _items.Filter((mydata) => { return mydata.Name != name; });
    ///         ...
    ///         this._items.add(item);
    ///         this.save(context);
    ///     }
    ///     
    /// var persistence = new MyMemoryPersistence();
    /// 
    /// persistence.Set("123", new MyData("ABC"));
    /// Console.Out.WriteLine(persistence.getByName("123", "ABC")).toString(); // Result: { name: "ABC" }
    /// </code>
    /// </example>
    public class MemoryPersistence<T> : IReconfigurable, IReferenceable, IOpenable, ICleanable
    {
        protected int _maxPageSize = 100;

        protected readonly string _typeName;
        protected CompositeLogger _logger = new CompositeLogger();

        protected List<T> _items = new List<T>();
        protected ILoader<T> _loader;
        protected ISaver<T> _saver;
        protected readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected bool _opened = false;

        /// <summary>
        /// Creates a new instance of the persistence.
        /// </summary>
        public MemoryPersistence()
            : this(null, null)
        { }

        /// <summary>
        /// Creates a new instance of the persistence.
        /// </summary>
        /// <param name="loader">(optional) a loader to load items from external datasource.</param>
        /// <param name="saver">(optional) a saver to save items to external datasource.</param>
        protected MemoryPersistence(ILoader<T> loader, ISaver<T> saver)
        {
            _typeName = typeof(T).Name;
            _loader = loader;
            _saver = saver;
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public virtual void Configure(ConfigParams config)
        {
            // Todo: Use connection and auth components
            _maxPageSize = config.GetAsIntegerWithDefault("max_page_size", _maxPageSize);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public void SetReferences(IReferences references)
        {
            _logger.SetReferences(references);
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public bool IsOpen()
        {
            return _opened;
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async Task OpenAsync(IContext context)
        {
            await LoadAsync(context);
            _opened = true;
        }

        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public async Task CloseAsync(IContext context)
        {
            await SaveAsync(context);
            _opened = false;
        }

        private Task LoadAsync(IContext context)
        {
            if (_loader == null)
                return Task.Delay(0);

            _lock.EnterWriteLock();

            try
            {
                _items = _loader.LoadAsync(context).Result;
                _logger.Trace(context, "Loaded {0} of {1}", _items.Count, _typeName);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return Task.Delay(0);
        }

        /// <summary>
        /// Saves items to external data source using configured saver component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public Task SaveAsync(IContext context)
        {
            if (_saver == null)
                return Task.Delay(0);

            _lock.EnterWriteLock();

            try
            {
                var task = _saver.SaveAsync(context, _items);
                task.Wait();

                _logger.Trace(context, "Saved {0} of {1}", _items.Count, _typeName);

                return Task.Delay(0);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Clears component state.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public Task ClearAsync(IContext context)
        {
            _lock.EnterWriteLock();

            try
            {
                _items = new List<T>();

                _logger.Trace(context, "Cleared {0}", _typeName);

                return SaveAsync(context);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private List<T> Filter(IList<T> items, IList<Func<T, bool>> matchFunctions)
        {
            var result = new List<T>();

            foreach (var item in items)
            {
                var isMatched = true;

                foreach (var matchFunction in matchFunctions)
                {
                    isMatched &= matchFunction(item);
                }

                if (isMatched)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a page of data items retrieved by a given filter and sorted according to sort parameters.
        /// 
        /// This method shall be called by a public getPageByFilter method from child class that
        /// receives FilterParams and converts them into a filter function.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="matchFunctions">(optional) a filter function to filter items</param>
        /// <param name="paging">(optional) paging parameters</param>
        /// <returns>a data page of result by filter.</returns>
        public async Task<DataPage<T>> GetPageByFilterAsync(IContext context,
            IList<Func<T, bool>> matchFunctions, PagingParams paging)
        {
            _lock.EnterReadLock();

            try
            {
                var filteredItems = Filter(_items, matchFunctions);

                paging = paging ?? new PagingParams();
                var skip = paging.GetSkip(0);
                var take = paging.GetTake(_maxPageSize);

                _logger.Trace(context, $"Retrieved {filteredItems.Count} items");

                return await Task.FromResult(new DataPage<T>()
                {
                    Data = filteredItems.Take((int)take).Skip((int)skip).ToList(),
                    Total = paging.Total ? filteredItems.Count : (long?)null
                });
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets a list of data items retrieved by a given filter and sorted according to sort parameters.
        /// 
        /// This method shall be called by a public getListByFilter method from child
        /// class that receives FilterParams and converts them into a filter function.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="matchFunctions">(optional) a filter function to filter items</param>
        /// <returns>a data list of results by filter.</returns>
        public async Task<List<T>> GetListByFilterAsync(IContext context,
            IList<Func<T, bool>> matchFunctions)
        {
            _lock.EnterReadLock();

            try
            {
                var filteredItems = Filter(_items, matchFunctions);

                _logger.Trace(context, $"Retrieved {filteredItems.Count} items");

                return await Task.FromResult(filteredItems);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets a random item from items that match to a given filter.
        /// 
        /// This method shall be called by a public getOneRandom method from child class
        /// that receives FilterParams and converts them into a filter function.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="matchFunctions">(optional) a filter function to filter items.</param>
        /// <returns>a random item.</returns>
        public async Task<T> GetOneRandomAsync(IContext context, IList<Func<T, bool>> matchFunctions)
        {
            var filteredItems = Filter(_items, matchFunctions);

            if (filteredItems.Count > 0)
            {
                var randomIndex = new Random().Next(0, filteredItems.Count - 1);
                var result = filteredItems[randomIndex];

                _logger.Trace(context, "Retrieved randomly {0}", result);

                return await Task.FromResult(result);
            }

            _logger.Trace(context, "Cannot randomnly find item by filter");

            return default(T);
        }

        /// <summary>
        /// Creates a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">an item to be created.</param>
        /// <returns>created item.</returns>
        public virtual async Task<T> CreateAsync(IContext context, T item)
        {
            _lock.EnterWriteLock();

            try
            {
                _items.Add(item);

                _logger.Trace(context, "Created {0}", item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            await SaveAsync(context);

            return item;
        }

        /// <summary>
        /// Deletes data items that match to a given filter.
        /// 
        /// This method shall be called by a public deleteByFilter method from child
        /// class that receives FilterParams and converts them into a filter function.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="matchFunctions">(optional) a filter function to filter items.</param>
        public async Task DeleteByFilterAsync(IContext context, IList<Func<T, bool>> matchFunctions)
        {
            var deleted = false;

            _lock.EnterWriteLock();

            try
            {
                var filteredItems = Filter(_items, matchFunctions);
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