using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using PipServices4.Commons.Data;
using PipServices4.Commons.Reflect;
using PipServices4.Components.Context;
using PipServices4.Data;
using PipServices4.Data.Data;
using PipServices4.Data.Keys;
using PipServices4.Persistence.Read;
using PipServices4.Persistence.Write;

namespace PipServices4.Sqlserver.Persistence
{
    /// <summary>
    /// Abstract persistence component that stores data in SqlServer
    /// and implements a number of CRUD operations over data items with unique ids.
    /// The data items must implement IIdentifiable interface.
    /// 
    /// In basic scenarios child classes shall only override <c>GetPageByFilter()</c>,
    /// <c>GetListByFilter()</c> or <c>DeleteByFilter()</c> operations with specific filter function.
    /// All other operations can be used out of the box.
    /// 
    /// In complex scenarios child classes can implement additional operations by
    /// accessing <c>this._collection</c> and <c>this._model</c> properties.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// - collection:                  (optional) SqlServer collection name
    /// 
    /// connection(s):
    /// - discovery_key:             (optional) a key to retrieve the connection from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a>
    /// - host:                      host name or IP address
    /// - port:                      port number (default: 27017)
    /// - uri:                       resource URI or connection string with all parameters in it
    /// 
    /// credential(s):
    /// - store_key:                 (optional) a key to retrieve the credentials from <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_auth_1_1_i_credential_store.html">ICredentialStore</a>
    /// - username:                  (optional) user name
    /// - password:                  (optional) user password
    /// 
    /// options:
    /// - max_pool_size:             (optional) maximum connection pool size (default: 2)
    /// - keep_alive:                (optional) enable connection keep alive (default: true)
    /// - connect_timeout:           (optional) connection timeout in milliseconds (default: 5 sec)
    /// - auto_reconnect:            (optional) enable auto reconnection (default: true)
    /// - max_page_size:             (optional) maximum page size (default: 100)
    /// - debug:                     (optional) enable debug output (default: false).
    /// 
    /// ### References ###
    /// 
    /// - *:logger:*:*:1.0           (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// - *:discovery:*:*:1.0        (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_connect_1_1_i_discovery.html">IDiscovery</a> services
    /// - *:credential-store:*:*:1.0 (optional) Credential stores to resolve credentials
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <example>
    /// <code>
    /// class MySqlServerPersistence: SqlServerPersistence<MyData, string> 
    /// {
    ///     public constructor()
    ///     {
    ///         base("mydata", MyData.class);
    ///     }
    /// 
    ///     private FilterDefinition<MyData> ComposeFilter(FilterParams filter)
    ///     {
    ///         filterParams = filterParams ?? new FilterParams();
    ///         var builder = Builders<BeaconV1>.Filter;
    ///         var filter = builder.Empty;
    ///         String name = filter.getAsNullableString('name');
    ///         if (name != null)
    ///             filter &= builder.Eq(b => b.Name, name);
    ///         return filter;
    ///     }
    ///     
    ///     public GetPageByFilter(IContext context, FilterParams filter, PagingParams paging)
    ///     {
    ///         base.GetPageByFilter(context, this.ComposeFilter(filter), paging, null, null);
    ///     }
    /// }
    /// 
    /// var persistence = new MySqlServerPersistence();
    /// persistence.Configure(ConfigParams.fromTuples(
    /// "host", "localhost",
    /// "port", 27017 ));
    /// 
    /// persitence.Open("123");
    /// 
    /// persistence.Create("123", new MyData("1", "ABC"));
    /// var mydata = persistence.GetPageByFilter(
    /// "123",
    /// FilterParams.FromTuples("name", "ABC"),
    /// Console.Out.WriteLine(mydata.Data);          // Result: { id: "1", name: "ABC" }
    /// 
    /// persistence.DeleteById("123", "1");
    /// ...
    /// </code>
    /// </example>
    public class IdentifiableSqlServerPersistence<T, K> : SqlServerPersistence<T>, IWriter<T, K>, IGetter<T, K>, ISetter<T>, IPartialUpdater<T, K>
        where T : IIdentifiable<K>, new()
        where K : class
    {
        /// <summary>
        /// Creates a new instance of the persistence component.
        /// </summary>
        /// <param name="tableName">(optional) a collection name.</param>
        public IdentifiableSqlServerPersistence(string tableName)
            : base(tableName)
        { }

        /// <summary>
        /// Gets a list of data items retrieved by given unique ids.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="ids">ids of data items to be retrieved</param>
        /// <returns>a data list of results by ids.</returns>
        public virtual async Task<List<T>> GetListByIdsAsync(IContext context, K[] ids)
        {
            var @params = GenerateParameters(ids);
            var query = "SELECT * FROM " + QuoteIdentifier(_tableName) + " WHERE [id] IN (" + @params + ")";

            var items = await ExecuteReaderAsync(query, ids);

            _logger.Trace(context, $"Retrieved {items.Count} from {_tableName}");

            return items.Select(item => ConvertToPublic(item)).ToList();
        }

        /// <summary>
        /// Gets a data item by its unique id.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="id">an id of data item to be retrieved.</param>
        /// <returns>a data item by id.</returns>
        public virtual async Task<T> GetOneByIdAsync(IContext context, K id)
        {
            var @params = new[] { id };
            var query = "SELECT * FROM " + QuoteIdentifier(_tableName) + " WHERE [id] = @Param1";

            var result = (await ExecuteReaderAsync(query, @params)).FirstOrDefault();

            if (result == null)
            {
                _logger.Trace(context, "Nothing found from {0} with id = {1}", _tableName, id);
                return default;
            }

            _logger.Trace(context, "Retrieved from {0} with id = {1}", _tableName, id);

            var item = ConvertToPublic(result);

            return item;
        }

        /// <summary>
        /// Creates a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">an item to be created.</param>
        /// <returns>created item.</returns>
        public override async Task<T> CreateAsync(IContext context, T item)
        {
            if (item is IStringIdentifiable && item.Id == null)
                ObjectWriter.SetProperty(item, nameof(item.Id), IdGenerator.NextLong());

            return await base.CreateAsync(context, item);
        }

        /// <summary>
        /// Sets a data item. If the data item exists it updates it, otherwise it create a new data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">an item to be set.</param>
        /// <returns>updated item.</returns>
        public virtual async Task<T> SetAsync(IContext context, T item)
        {
            if (item == null || item.Id == null)
                return default;

            var map = ConvertFromPublic(item);
            var columns = GenerateColumns(map);
            var @params = GenerateParameters(map);
            var setParams = GenerateSetParameters(map);
            var values = GenerateValues(map);

            var query = "INSERT INTO " + QuoteIdentifier(_tableName) + " (" + columns + ") OUTPUT INSERTED.* VALUES (" + @params +")";

            AnyValueMap result = null;

            var duplicate = false;
            try
            {
                result = (await ExecuteReaderAsync(query, map)).FirstOrDefault();
            }
            catch (SqlException ex)
            {
                duplicate = ex.Number == 2601 || ex.Number == 2627;
            }

            if (duplicate)
            {
                values.Add(item.Id);

                query = "UPDATE " + QuoteIdentifier(_tableName) + " SET " + setParams + " OUTPUT INSERTED.* WHERE [id]=@Param" + values.Count;
                result = (await ExecuteReaderAsync(query, values)).FirstOrDefault();
            }

            _logger.Trace(context, "Set in {0} with id = {1}", _tableName, item.Id);

            var newItem = ConvertToPublic(result);
            return newItem;
        }

        /// <summary>
        /// Updates a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="item">an item to be updated.</param>
        /// <returns>updated item.</returns>
        public virtual async Task<T> UpdateAsync(IContext context, T item)
        {
            if (item == null || item.Id == null)
                return default;

            var map = ConvertFromPublic(item);
            var @params = GenerateSetParameters(map);
            var values = GenerateValues(map);
            values.Add(item.Id);

            var query = "UPDATE " + QuoteIdentifier(_tableName)
                + " SET " + @params + " OUTPUT INSERTED.* WHERE [id]=@Param" + values.Count;

            var result = (await ExecuteReaderAsync(query, values)).FirstOrDefault();

            _logger.Trace(context, "Update in {0} with id = {1}", _tableName, item.Id);
            
            var newItem = ConvertToPublic(result);
            return newItem;
        }

        /// <summary>
        /// Updates only few selected fields in a data item.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="id">an id of data item to be updated.</param>
        /// <param name="data">a map with fields to be updated.</param>
        /// <returns>updated item</returns>
        public virtual async Task<T> UpdatePartially(IContext context, K id, AnyValueMap data)
        {
            if (data == null || id == null)
                return default;

            var map = data;
            var @params = GenerateSetParameters(map);
            var values = GenerateValues(map);
            values.Add(id);

            var query = "UPDATE " + QuoteIdentifier(_tableName)
                + " SET " + @params + " OUTPUT INSERTED.* WHERE [id]=@Param" + values.Count;

            var result = (await ExecuteReaderAsync(query, values)).FirstOrDefault();

            _logger.Trace(context, "Updated partially in {0} with id = {1}", _tableName, id);

            var newItem = ConvertToPublic(result);
            return newItem;
        }

        /// <summary>
        /// Deleted a data item by it's unique id.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="id">an id of the item to be deleted</param>
        /// <returns>deleted item.</returns>
        public virtual async Task<T> DeleteByIdAsync(IContext context, K id)
        {
            var values = new[] { id };

            var query = "DELETE FROM " + QuoteIdentifier(_tableName) + " OUTPUT DELETED.* WHERE [id] = @Param1";

            var result = (await ExecuteReaderAsync(query, values)).FirstOrDefault();

            _logger.Trace(context, "Deleted from {0} with id = {1}", _tableName, id);

            var newItem = ConvertToPublic(result);
            return newItem;
        }

        /// <summary>
        /// Deletes multiple data items by their unique ids.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="ids">ids of data items to be deleted.</param>
        public virtual async Task DeleteByIdsAsync(IContext context, K[] ids)
        {
            var @params = GenerateParameters(ids);
            var query = "DELETE FROM " + QuoteIdentifier(_tableName) + " WHERE [id] IN (" + @params +")";

            var result = await ExecuteNonQuery(query, ids);

            _logger.Trace(context, $"Deleted {result} from {_tableName}");
        }
    }
}