using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Azure.Controllers
{
    public class DummyCommandableAzureFunctionControllerTest
    {
        private DummyAzureFunctionFixture fixture;

        public DummyCommandableAzureFunctionControllerTest()
        {
            fixture = new DummyAzureFunctionFixture(CommandableFunction.Run);
        }

        [Fact]
        public async Task TestCrudOperations()
        {
            await fixture.TestCrudOperations();
        }
    }
}
