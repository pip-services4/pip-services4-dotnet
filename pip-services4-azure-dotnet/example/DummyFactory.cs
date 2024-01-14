using PipServices4.Azure.Controllers;
using PipServices4.Components.Build;
using PipServices4.Components.Refer;

namespace PipServices4.Azure
{
    public class DummyFactory: Factory
    {
        public static readonly Descriptor Descriptor = new Descriptor("pip-services-dummies", "factory", "default", "default", "1.0");
        public static readonly Descriptor ServiceDescriptor = new Descriptor("pip-services-dummies", "service", "default", "*", "1.0");
        public static readonly Descriptor CloudFunctionControllerDescriptor = new Descriptor("pip-services-dummies", "controller", "azure-function", "*", "1.0");
        public static readonly Descriptor CmdCloudFunctionControllerDescriptor = new Descriptor("pip-services-dummies", "controller", "commandable-azure-function", "*", "1.0");
    
        public DummyFactory(): base()
        {
            this.RegisterAsType(ServiceDescriptor, typeof(DummyService));
            this.RegisterAsType(CloudFunctionControllerDescriptor, typeof(DummyAzureFunctionController));
            this.RegisterAsType(CmdCloudFunctionControllerDescriptor, typeof(DummyCommandableAzureFunctionController));
        }
    }
}