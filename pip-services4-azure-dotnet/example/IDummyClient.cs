using PipServices4.Components.Context;
using PipServices4.Data.Query;
using System.Threading.Tasks;

namespace PipServices4.Azure
{
    public interface IDummyClient
    {
        Task<DataPage<Dummy>>  GetDummiesAsync(IContext context, FilterParams filter, PagingParams paging);
        Task<Dummy>  GetDummyByIdAsync(IContext context, string dummyId);
        Task<Dummy>  CreateDummyAsync(IContext context, Dummy dummy);
        Task<Dummy>  UpdateDummyAsync(IContext context, Dummy dummy);
        Task<Dummy>  DeleteDummyAsync(IContext context, string dummyId);
    }
}