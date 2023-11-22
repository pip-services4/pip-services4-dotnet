using PipServices4.Commons.Data;
using PipServices4.Components.Context;
using PipServices4.Data.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PipServices4.Mysql.Persistence
{
    public class IdentifiableJsonMySqlPersistence<T, K>: IdentifiableMySqlPersistence<T, K>
        where T : IIdentifiable<K>, new()
        where K : class
    {
        public IdentifiableJsonMySqlPersistence(string tableName)
            : base(tableName)
        { }

        /// <summary>
        /// Adds DML statement to automatically create JSON(B) table
        /// </summary>
        /// <param name="idType">type of the id column (default: VARCHAR(32))</param>
        /// <param name="dataType">type of the data column (default: NVARCHAR(MAX))</param>
        protected void EnsureTable(string idType = "VARCHAR(32)", string dataType = "JSON")
        { 
            var query = "CREATE TABLE IF NOT EXISTS " + QuoteIdentifier(_tableName)
            + " (`id` " + idType + " PRIMARY KEY, `data` " + dataType + ")";

            EnsureSchema(query);
        }

        /// <summary>
        /// Converts object value from internal to public format.
        /// </summary>
        /// <param name="value">an object in internal format to convert.</param>
        /// <returns>converted object in public format.</returns>
        protected override T ConvertToPublic(AnyValueMap map)
        {
            if (map != null && map.TryGetValue("data", out object value) && value != null)
            {
                return base.ConvertToPublic(value as AnyValueMap);
            }

            return default;
        }

        /// <summary>
        /// Convert object value from public to internal format.
        /// </summary>
        /// <param name="value">an object in public format to convert.</param>
        /// <returns>converted object in internal format.</returns>
        protected override AnyValueMap ConvertFromPublic(T value)
        {
            if (value == null) return null;
            return AnyValueMap.FromTuples("id", value.Id, "data", base.ConvertFromPublic(value));
        }

        ///// <summary>
        ///// Updates only few selected fields in a data item.
        ///// </summary>
        ///// <param name="context">(optional) execution context to trace execution through call chain.</param>
        ///// <param name="id">an id of data item to be updated.</param>
        ///// <param name="data">a map with fields to be updated.</param>
        ///// <returns>updated item</returns>
        public override async Task<T> UpdatePartially(IContext context, K id, AnyValueMap data)
        {
            if (data == null || id == null)
                return default;

            var values = new object[] { data, id };

            var query = "UPDATE " + QuoteIdentifier(_tableName) + " SET `data`=JSON_MERGE_PATCH(data,@Param1) WHERE id=@Param" + values.Length;
            query += "; SELECT * FROM " + QuoteIdentifier(_tableName) + " WHERE id=@Param" + values.Length;

            var result = (await ExecuteReaderAsync(query, values)).FirstOrDefault();

            _logger.Trace(context, "Updated partially in {0} with id = {1}", _tableName, id);

            var newItem = ConvertToPublic(result);
            return newItem;
        }
    }
}
