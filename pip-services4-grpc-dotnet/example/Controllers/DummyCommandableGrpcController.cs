using PipServices4.Components.Refer;

namespace PipServices4.Grpc.Controllers
{
    public sealed class DummyCommandableGrpcController : CommandableGrpcController
    {
        public DummyCommandableGrpcController(string name = null) 
            : base(name)
        {
            _dependencyResolver.Put("service", new Descriptor("pip-services4-dummies", "service", "default", "*", "1.0"));
        }
    }
}
