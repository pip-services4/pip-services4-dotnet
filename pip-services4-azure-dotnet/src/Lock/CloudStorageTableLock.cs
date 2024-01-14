using Azure;
using Azure.Data.Tables;
using PipServices4.Commons.Convert;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Observability.Log;
using System;

namespace PipServices4.Azure.Lock
{
    public class CloudStorageTableLock : Logic.Lock.Lock, IConfigurable, IReferenceable
    {
        private TableServiceClient _serviceClient;
        private TableClient _tableClient;
        private ConfigParams _connectionConfigParams;

        protected CompositeLogger _logger = new CompositeLogger();

        public override void Configure(ConfigParams config)
        {
            _connectionConfigParams = ConfigParams.FromTuples(
                "Table", config.GetAsNullableString("table"),
                "DefaultEndpointsProtocol", config.GetAsNullableString("connection.protocol"),
                "AccountName", config.GetAsNullableString("credential.access_id"),
                "AccountKey", config.GetAsNullableString("credential.access_key"));
        }

        public void SetReferences(IReferences references)
        {
            _logger.SetReferences(references);

            InitializeConnection();
        }

        private void InitializeConnection()
        {
            var context = Context.FromTraceId("CloudStorageTableLock");
            try
            {
                var connection = _connectionConfigParams;

                var tableName = connection.Get("Table") ?? "locks";
                connection.Remove("Table");

                var newConnectionString = connection.ToString();

                _serviceClient = new TableServiceClient(newConnectionString);

                _tableClient = _serviceClient.GetTableClient(tableName);

                _logger.Trace(context, $"Connected to lock storage table '{tableName}'.");
            }
            catch (Exception exception)
            {
                _logger.Error(context, exception,
                    $"Failed to configure the connection to lock storage table with parameters '{_connectionConfigParams}'.");
            }
        }

        public override bool TryAcquireLock(IContext context, string key, long timeToLive)
        {
            try
            {
                if (_tableClient == null)
                {
                    _logger.Error(context, $"TryAcquireLock: Lock storage table is not initialized.");
                    return false;
                }

                var result = _tableClient.GetEntityIfExists<TableLock>(context != null ? ContextResolver.GetTraceId(context) : null, key);

                if (result.HasValue)
                {
                    var tableLock = result.Value;

                    if (tableLock.Expired > DateTime.UtcNow)
                    {
                        _logger.Trace(context, $"TryAcquireLock: Key = '{key}' has been already locked and not expired.");
                        return false;
                    }

                    _logger.Trace(context, $"TryAcquireLock: Locked key = '{key}' expired.");
                }

                var lockRecord = new TableLock(context, key, timeToLive);

                _tableClient.UpsertEntity(lockRecord);

                _logger.Trace(context, $"TryAcquireLock: Set Key = '{key}' to 'lock' state; it will be expired at {lockRecord.Expired.ToString()} UTC.");
                return true;
            }
            catch (Exception exception)
            {
                _logger.Error(context, exception, $"TryAcquireLock: Failed to acquire lock for key = '{key}'.");
                return false;
            }
        }

        public override void ReleaseLock(IContext context, string key)
        {
            try
            {
                if (_tableClient == null)
                {
                    _logger.Error(context, $"TryAcquireLock: Lock storage table is not initialized.");
                    return;
                }

                var traceId = context != null ? ContextResolver.GetTraceId(context) : null;
                var result = _tableClient.GetEntityIfExists<TableLock>(traceId, key);

                if (result.HasValue)
                {
                    try
                    {
                        _tableClient.DeleteEntity(traceId, key);
                        _logger.Trace(context, $"ReleaseLock: Key = '{key}' is released from 'lock' state.");
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(context, exception, $"ReleaseLock: Failed to release lock for key = '{key}'.");
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error(context, exception, $"ReleaseLock: Failed to release lock for key = '{key}'.");
            }
        }

        public class TableLock : ITableEntity
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
            public DateTime Expired { get; set; }

            public TableLock() { }

            public TableLock(IContext context, string key, long timeToLive)
            {
                PartitionKey = context != null ? ContextResolver.GetTraceId(context) : null;
                RowKey = key;
                Timestamp = DateTimeConverter.ToDateTime(DateTime.UtcNow);
                Expired = DateTimeConverter.ToDateTime(DateTime.UtcNow.AddMilliseconds(timeToLive));
            }
        }
    }
}
