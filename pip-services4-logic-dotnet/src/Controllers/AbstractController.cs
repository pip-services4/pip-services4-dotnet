using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Logic.Cache;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using System;
using System.Threading.Tasks;

namespace PipServices4.Logic.Controllers
{
    public abstract class AbstractController : IConfigurable, IReferenceable
    {
        protected DependencyResolver _dependencyResolver = new DependencyResolver();
        protected CompositeLogger _logger = new CompositeLogger();
        protected CompositeCounters _counters = new CompositeCounters();
        protected ICache _cache = new NullCache();

        public abstract string Component { get; }

        public virtual void Configure(ConfigParams config)
        {
            _dependencyResolver.Configure(config);
        }

        public virtual void SetReferences(IReferences references)
        {
            _dependencyResolver.SetReferences(references);
            _logger.SetReferences(references);
            _counters.SetReferences(references);
        }

        #region Instrumentation Methods

        protected virtual CounterTiming Instrument(IContext context, string methodName, string message = "")
        {
            _logger.Trace(context, "Executed {0}.{1} {2}", Component, methodName, message);
            return _counters.BeginTiming(Component + "." + methodName + ".exec_time");
        }

        protected virtual void HandleError(IContext context, string methodName, Exception ex)
        {
            _logger.Error(context, ex, "Failed to execute {0}.{1}", Component, methodName);
        }

        protected async Task<T> SafeInvokeAsync<T>(IContext context, string methodName, Func<Task<T>> invokeFunc, bool throwException = false)
        {
            return await SafeInvokeAsync<T>(context, methodName, invokeFunc, null, throwException);
        }

        protected async Task<T> SafeInvokeAsync<T>(IContext context, string methodName, Func<Task<T>> invokeFunc, Func<Task<T>> errorHandlerFunc, bool throwException = false)
        {
            using (var timing = Instrument(context, methodName))
            {
                try
                {
                    return await invokeFunc();
                }
                catch (Exception ex)
                {
                    HandleError(context, methodName, ex);

                    if (errorHandlerFunc != null)
                    {
                        return await errorHandlerFunc();
                    }

                    if (throwException)
                    {
                        throw ex;
                    }
                }

                return await Task.FromResult(default(T));
            }
        }

        #endregion

        #region Cache Methods

        protected virtual async Task<T> RetrieveFromCacheAsync<T>(IContext context, string cacheKey)
        {
            return await _cache.RetrieveAsync<T>(context, cacheKey);
        }

        protected virtual async Task<T> StoreInCacheAsync<T>(IContext context, string cacheKey, T result)
        {
            return await _cache.StoreAsync(context, cacheKey, result, 0);
        }

        protected virtual async Task RemoveFromCacheAsync(IContext context, string id)
        {
            var cacheKey = GetCacheKey(id);
            await _cache.RemoveAsync(context, cacheKey);

            cacheKey = GetProjectionCacheKey(id);
            await _cache.RemoveAsync(context, cacheKey);
        }

        protected virtual string GetProjectionCacheKey(string id)
        {
            return $"{Component}.{id}.Projection";
        }

        protected virtual string GetCacheKey(string id)
        {
            return $"{Component}.{id}";
        }

        #endregion

        #region Audit Methods

        protected virtual async Task AuditCreateAsync<T>(IContext context, string collectionName, object createdObject, Func<Task<T>> auditFunc)
        {
            if (createdObject == null)
            {
                _logger.Error(context, $"Unable to audit create null object for collection '{collectionName}'.");
                return;
            }

            await SafeAuditAsync(context, "AuditCreateAsync", auditFunc);
        }

        protected virtual async Task AuditUpdateAsync<T>(IContext context, string collectionName, object oldObject, object updatedObject, Func<Task<T>> auditFunc)
        {
            if (oldObject == null || updatedObject == null)
            {
                _logger.Error(context, $"Unable to audit update of null object for collection '{collectionName}'.");
                return;
            }

            await SafeAuditAsync(context, "AuditUpdateAsync", auditFunc);
        }

        protected virtual async Task AuditDeleteAsync<T>(IContext context, string collectionName, object deletedObject, Func<Task<T>> auditFunc)
        {
            if (deletedObject == null)
            {
                _logger.Error(context, $"Unable to audit delete of null object for collection '{collectionName}'.");
                return;
            }

            await SafeAuditAsync(context, "AuditDeleteAsync", auditFunc);
        }

        private async Task<T> SafeAuditAsync<T>(IContext context, string methodName, Func<Task<T>> auditFunc)
        {
            using (var timing = Instrument(context, methodName))
            {
                try
                {
                    return await auditFunc();
                }
                catch (Exception ex)
                {
                    HandleError(context, methodName, ex);
                }

                return await Task.FromResult(default(T));
            }
        }

        #endregion

    }
}
