using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PipServices4.Http.Controllers
{
    public interface IInitializable
    {
        void ConfigureServices(IServiceCollection services);
        void ConfigureApplication(IApplicationBuilder applicationBuilder);
    }
}
