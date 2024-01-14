using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Swagger.Services;

namespace PipServices4.Swagger.Build
{
    public class DefaultSwaggerFactory : Factory
    {
        public static Descriptor Descriptor = new Descriptor("pip-services", "factory", "swagger", "default", "1.0");
        public static Descriptor SwaggerControllerDescriptor = new Descriptor("pip-services", "swagger-controller", "*", "*", "1.0");

        public DefaultSwaggerFactory()
        {
            RegisterAsType(SwaggerControllerDescriptor, typeof(SwaggerController));
        }
    }
}
