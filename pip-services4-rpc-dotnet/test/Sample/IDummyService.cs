﻿using PipServices4.Components.Context;
using PipServices4.Data.Query;
using System.Threading.Tasks;

namespace PipServices4.Rpc.Test.Sample
{
    public interface IDummyService
    {
        Task<DataPage<Dummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PagingParams paging);
        Task<Dummy> GetOneByIdAsync(IContext context, string id);
        Task<Dummy> CreateAsync(IContext context, Dummy entity);
        Task<Dummy> UpdateAsync(IContext context, Dummy entity);
        Task<Dummy> DeleteByIdAsync(IContext context, string id);
        Task RaiseExceptionAsync(IContext context);

        Task<string> CheckCorrelationId(IContext context);

        Task<bool> PingAsync();
    }
}
