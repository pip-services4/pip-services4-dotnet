using PipServices4.Components.Refer;

namespace PipServices4.Azure.Controllers
{
    public class DummyCommandableAzureFunctionController: CommandableAzureFunctionController
    {
        public DummyCommandableAzureFunctionController() : base("dummies")
        {
            _dependencyResolver.Put("service", new Descriptor("pip-services-dummies", "service", "default", "*", "*"));
        }
    }
}
