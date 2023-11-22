using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;

namespace PipServices4.Http.Services
{
    public sealed class DummyCommandableHttpService : CommandableHttpService
    {
        public DummyCommandableHttpService() 
            : base("dummy")
        {
            _dependencyResolver.Put("controller", new Descriptor("pip-services4-dummies", "controller", "default", "*", "1.0"));
        }

		public override void Register()
		{
			if (!_swaggerAuto && _swaggerEnable)
			{
				RegisterOpenApiSpecFromResource("DummyRestServiceSwagger.yaml");
			}

			base.Register();
		}
	}
}
