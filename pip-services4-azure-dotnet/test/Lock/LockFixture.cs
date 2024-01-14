using PipServices4.Components.Context;
using PipServices4.Data.Keys;
using PipServices4.Logic.Lock;
using System;
using Xunit;

namespace PipServices4.Azure.Lock
{
    public class LockFixture
    {
        public static string Lock1 = "lock_1";
        public static string Lock2 = "lock_2";
        public static string Lock3 = "lock_3";

        private ILock _lock;
        private IContext _context = Context.FromTraceId(IdGenerator.NextLong());

        public LockFixture(ILock @lock)
        {
            _lock = @lock;
        }

        public void TestTryAcquireLock()
        {
            // Try to acquire lock for the first time
            var result = _lock.TryAcquireLock(_context, Lock1, 3000);
            Assert.True(result);

            // Try to acquire lock for the second time
            result = _lock.TryAcquireLock(_context, Lock1, 3000);
            Assert.False(result);

            // Release the lock
            _lock.ReleaseLock(_context, Lock1);

            // Try to acquire lock for the third time
            result = _lock.TryAcquireLock(_context, Lock1, 3000);
            Assert.True(result);

            _lock.ReleaseLock(_context, Lock1);
        }

        public void TestAcquireLock()
        {
            // Acquire lock for the first time
            _lock.AcquireLock(_context, Lock2, 3000, 1000);

            // Acquire lock for the second time
            var result = false;
            try
            {
                _lock.AcquireLock(_context, Lock2, 3000, 1000);
            }
            catch (Exception)
            {
                result = true;
            }
            Assert.True(result);

            // Release the lock
            _lock.ReleaseLock(_context, Lock2);

            // Acquire lock for the third time
            _lock.AcquireLock(_context, Lock2, 3000, 1000);
            _lock.ReleaseLock(_context, Lock2);
        }

        public void TestReleaseLock()
        {
            // Acquire lock for the first time
            var result = _lock.TryAcquireLock(_context, Lock3, 3000);
            Assert.True(result);

            // Release the lock for the first time
            _lock.ReleaseLock(_context, Lock3);

            // Release the lock for the second time
            _lock.ReleaseLock(_context, Lock3);
        }

    }
}
