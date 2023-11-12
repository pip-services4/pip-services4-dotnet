using PipServices4.Components.Config;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Persistence.Test.Persistence
{
    public class DummyMemoryPersistenceTest : IDisposable
    {
        private DummyMemoryPersistence persistence;
        private PersistenceFixture2 fixture;

        public DummyMemoryPersistenceTest()
        {
            persistence = new DummyMemoryPersistence();
            persistence.Configure(new ConfigParams());

            fixture = new PersistenceFixture2(persistence);

            persistence.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            persistence.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestCrudOperationsAsync()
        {
            await fixture.TestCrudOperationsAsync();
        }

        //[Fact]
        //public async Task TestGetWithFiltersAsync()
        //{
        //    await fixture.TestGetWithFiltersAsync();
        //}
    }
}
