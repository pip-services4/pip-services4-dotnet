using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Azure.Controllers
{
    public class DummyAzureFunctionControllerTest
    {
        private DummyAzureFunctionFixture fixture;

        public DummyAzureFunctionControllerTest()
        {
            fixture = new DummyAzureFunctionFixture(Function.Run);
        }

        [Fact]
        public async Task TestCrudOperations()
        {
            await fixture.TestCrudOperations();
        }
    }
}
