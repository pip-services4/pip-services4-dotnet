using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Redis.Cache;
using PipServices4.Redis.Lock;

namespace PipServices4.Redis.Build
{
    /// <summary>
    /// Creates Redis components by their descriptors.
    /// </summary>
    /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-redis-dotnet/class_pip_services_1_1_redis_1_1_cache_1_1_redis_cache.html">RedisCache</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-redis-dotnet/class_pip_services_1_1_redis_1_1_lock_1_1_redis_lock.html">RedisLock</a>
    public class DefaultRedisFactory: Factory
    {
        public static readonly Descriptor Descriptor = new Descriptor("pip-services", "factory", "redis", "default", "1.0");
        public static readonly Descriptor Descriptor3 = new Descriptor("pip-services4", "factory", "redis", "default", "1.0");
        public static readonly Descriptor RedisCacheDescriptor = new Descriptor("pip-services", "cache", "redis", "*", "1.0");
        public static readonly Descriptor RedisCache4Descriptor = new Descriptor("pip-services4", "cache", "redis", "*", "1.0");
        public static readonly Descriptor RedisLockDescriptor = new Descriptor("pip-services", "lock", "redis", "*", "1.0");
        public static readonly Descriptor RedisLock4Descriptor = new Descriptor("pip-services4", "lock", "redis", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultRedisFactory()
        {
            RegisterAsType(DefaultRedisFactory.RedisCacheDescriptor, typeof(RedisCache));
            RegisterAsType(DefaultRedisFactory.RedisCache4Descriptor, typeof(RedisCache));
            RegisterAsType(DefaultRedisFactory.RedisLockDescriptor, typeof(RedisLock));
            RegisterAsType(DefaultRedisFactory.RedisLock4Descriptor, typeof(RedisLock));
        }
    }
}
