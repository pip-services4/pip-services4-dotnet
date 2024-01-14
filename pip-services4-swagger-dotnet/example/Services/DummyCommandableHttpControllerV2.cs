using PipServices4.Components.Refer;
using PipServices4.Http.Controllers;

namespace PipServices4.Swagger.Services
{
    public sealed class DummyCommandableHttpControllerV2 : CommandableHttpController
    {
        private IDummyService _service;

        public DummyCommandableHttpControllerV2() 
            : base("Dummy")
        {
            _dependencyResolver.Put("service", new Descriptor("pip-services4-dummies", "service", "default", "*", "1.0"));
        }

        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);

            _service = references.GetOneRequired<IDummyService>(new Descriptor("pip-services4-dummies", "service", "default", "*", "1.0"));
        }
    }
}
