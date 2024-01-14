
namespace PipServices4.Http.Controllers
{
    /// <summary>
    /// Interface to perform Swagger registrations.
    /// </summary>
    public interface ISwaggerController
    {
        /// <summary>
        /// Perform required Swagger registration steps.
        /// </summary>
        void RegisterOpenApiSpec(string baseRoute, string swaggerRoute);
    }
}