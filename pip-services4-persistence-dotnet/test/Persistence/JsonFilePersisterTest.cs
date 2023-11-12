using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Persistence.Persistence;
using PipServices4.Persistence.Test.Sample;
using Xunit;

namespace PipServices4.Persistence.Test.Persistence
{
    public sealed class JsonFilePersisterTest
    {
        private readonly JsonFilePersister<Dummy> _persister;

        public JsonFilePersisterTest()
        {
            _persister = new JsonFilePersister<Dummy>();
        }

        [Fact]
        public void Configure_IfNoPathKey_Fails()
        {
            Assert.Throws<ConfigException>(() => _persister.Configure(new ConfigParams()));
        }

        [Fact]
        public void Configure_IfPathKeyCheckProperty_IsOk()
        {
            const string fileName = nameof(JsonFilePersisterTest);

            _persister.Configure(ConfigParams.FromTuples("path", fileName));

            Assert.Equal(fileName, _persister.Path);
        }
    }
}
