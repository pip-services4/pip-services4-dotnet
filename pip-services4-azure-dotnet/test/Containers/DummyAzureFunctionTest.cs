using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Azure.Containers
{
    public class DummyAzureFunctionTest
    {
        private DummyAzureFunctionFixture fixture;

        public DummyAzureFunctionTest()
        {
            fixture = new DummyAzureFunctionFixture(FunctionContainer.Run);
        }

        [Fact]
        public async Task TestCrudOperations()
        {
            await fixture.TestCrudOperations();
        }
    }
}
