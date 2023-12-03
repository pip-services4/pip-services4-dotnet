using PipServices4.Commons.Convert;
using PipServices4.Components.Config;
using PipServices4.Memcached.Lock;
using System;
using Xunit;

namespace PipServices4.Memcached.Test.Lock
{
    [Collection("Sequential")]
    public class MemcachedLockTest : IDisposable
    {
        private readonly bool _enabled;
        private readonly MemcachedLock _lock;
        private readonly LockFixture _fixture;

        public MemcachedLockTest()
        {
            var MEMCACHED_ENABLED = Environment.GetEnvironmentVariable("MEMCACHED_ENABLED") ?? "true";
            var MEMCACHED_SERVICE_HOST = Environment.GetEnvironmentVariable("MEMCACHED_SERVICE_HOST") ?? "localhost";
            var MEMCACHED_SERVICE_PORT = Environment.GetEnvironmentVariable("MEMCACHED_SERVICE_PORT") ?? "11211";

            _enabled = BooleanConverter.ToBoolean(MEMCACHED_ENABLED);
            if (_enabled)
            {
                _lock = new MemcachedLock();
                _lock.Configure(ConfigParams.FromTuples(
                    "connection.host", MEMCACHED_SERVICE_HOST,
                    "connection.port", MEMCACHED_SERVICE_PORT
                ));

                _fixture = new LockFixture(_lock);

                _lock.OpenAsync(null).Wait();
            }
        }

        public void Dispose()
        {
            if (_lock != null)
            {
                _lock.CloseAsync(null).Wait();
            }
        }

        [Fact]
        public void TestAcquireLock()
        {
            _fixture.TestAcquireLock();
        }

        [Fact]
        public void TestTryAcquireLock()
        {
            _fixture.TestTryAcquireLock();
        }

        [Fact]
        public void TestReleaseLock()
        {
            _fixture.TestReleaseLock();
        }
    }
}
