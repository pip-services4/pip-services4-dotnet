using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;

namespace PipServices4.Swagger.Services
{
    public sealed class DummyCommandableHttpController : CommandableHttpController
    {
        public DummyCommandableHttpController() 
            : base("dummy")
        {
            _dependencyResolver.Put("service", new Descriptor("pip-services4-dummies", "service", "default", "*", "1.0"));
        }

		public override void Register()
		{
			if (!_swaggerAuto && _swaggerEnable)
			{
				RegisterOpenApiSpecFromResource("DummyRestControllerSwagger.yaml");
			}

			base.Register();
		}
	}
}
