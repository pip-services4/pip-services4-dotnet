using PipServices4.Components.Refer;

namespace PipServices4.Azure.Containers
{
    public class DummyCommandableAzureFunction: CommandableAzureFunction
    {
        public DummyCommandableAzureFunction() : base("dummy", "Dummy Azure function")
        {
            this._dependencyResolver.Put("service", new Descriptor("pip-services-dummies", "service", "default", "*", "*"));
            this._factories.Add(new DummyFactory());
        }
    }
}
