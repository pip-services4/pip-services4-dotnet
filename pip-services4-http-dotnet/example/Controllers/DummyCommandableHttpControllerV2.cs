using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;

namespace PipServices4.Http.Services
{
    public sealed class DummyCommandableHttpControllerV2 : CommandableHttpController
    {
        public DummyCommandableHttpControllerV2() 
            : base("dummy")
        {
            _dependencyResolver.Put("service", new Descriptor("pip-services4-dummies", "service", "default", "*", "1.0"));
        }

        public override void Register()
        {
            if (!_swaggerAuto && _swaggerEnable)
                RegisterOpenApiSpec("swagger yaml content");

            base.Register();
        }
    }
}
