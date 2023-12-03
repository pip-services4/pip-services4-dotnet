using PipServices4.Components.Refer;
using PipServices4.Grpc.Services;

namespace PipServices4.Grpc.Services
{
    public sealed class DummyCommandableGrpcService : CommandableGrpcService
    {
        public DummyCommandableGrpcService(string name = null) 
            : base(name)
        {
            _dependencyResolver.Put("controller", new Descriptor("pip-services4-dummies", "controller", "default", "*", "1.0"));
        }
    }
}
