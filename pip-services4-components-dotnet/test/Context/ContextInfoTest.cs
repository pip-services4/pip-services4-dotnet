using System;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using Xunit;

namespace PipServices4.Components.test.Context
{
    public sealed class ContextInfoTest
    {
        [Fact]
        public void TestName()
        {
            var contextInfo = new ContextInfo();

            Assert.Equal("unknown", contextInfo.Name);

            contextInfo.Name = "new name";

            Assert.Equal("new name", contextInfo.Name);
        }

        [Fact]
        public void TestDescription()
        {
            var contextInfo = new ContextInfo();
            Assert.Null(contextInfo.Description);

            contextInfo.Description = "new description";
            Assert.Equal("new description", contextInfo.Description);
        }

        [Fact]
        public void TestContextId()
        {
            var contextInfo = new ContextInfo();

            contextInfo.ContextId = "new context id";

            Assert.Equal("new context id", contextInfo.ContextId);
        }

        [Fact]
        public void TestStartTime()
        {
            var contextInfo = new ContextInfo();
            Assert.Equal(contextInfo.StartTime.Year, DateTimeOffset.UtcNow.Year);
            Assert.Equal(contextInfo.StartTime.Month, DateTimeOffset.UtcNow.Month);

            contextInfo.StartTime = new DateTime(1975, 4, 8, 0, 0, 0, 0);
            Assert.Equal(contextInfo.StartTime, new DateTime(1975, 4, 8, 0, 0, 0, 0));
        }

        [Fact]
        public void TestFromConfigs()
        {
            var config = ConfigParams.FromTuples(
                "info.name", "new name",
                "info.description", "new description",
                "properties.access_key", "key",
                "properties.store_key", "store key"
                );

            var contextInfo = ContextInfo.FromConfig(config);
            Assert.Equal("new name", contextInfo.Name);
            Assert.Equal("new description", contextInfo.Description);
        }
    }
}
