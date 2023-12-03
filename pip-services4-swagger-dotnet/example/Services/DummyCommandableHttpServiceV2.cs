using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;

namespace PipServices4.Swagger.Services
{
    public sealed class DummyCommandableHttpServiceV2 : CommandableHttpService
    {
        private IDummyController _controller;

        public DummyCommandableHttpServiceV2() 
            : base("Dummy")
        {
            _dependencyResolver.Put("controller", new Descriptor("pip-services4-dummies", "controller", "default", "*", "1.0"));
        }

        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);

            _controller = references.GetOneRequired<IDummyController>(new Descriptor("pip-services4-dummies", "controller", "default", "*", "1.0"));
        }
    }
}
