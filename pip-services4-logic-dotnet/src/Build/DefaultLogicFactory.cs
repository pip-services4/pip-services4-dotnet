using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Logic.Cache;
using PipServices4.Logic.Lock;
using PipServices4.Logic.State;

namespace PipServices4.Logic.Build
{
	/// <summary>
	/// Creates business logic components by their descriptors.
	/// <see cref="Factory"/>
	/// <see cref="ICache"/>
	/// <see cref="MemoryCache"/>
	/// <see cref="NullCache"/>
	/// </summary>
	public class DefaultLogicFactory : Factory
	{
		public static readonly Descriptor Descriptor = new Descriptor("pip-services", "factory", "logic", "default", "1.0");
		public static readonly Descriptor NullCacheDescriptor = new Descriptor("pip-services", "cache", "null", "*", "1.0");
		public static readonly Descriptor MemoryCacheDescriptor = new Descriptor("pip-services", "cache", "memory", "*", "1.0");
		public static readonly Descriptor NullLockDescriptor = new Descriptor("pip-services", "lock", "null", "*", "1.0");
		public static readonly Descriptor MemoryLockDescriptor = new Descriptor("pip-services", "lock", "memory", "*", "1.0");
		public static readonly Descriptor NullStateStoreDescriptor = new Descriptor("pip-services", "state-store", "null", "*", "1.0");
		public static readonly Descriptor MemoryStateStoreDescriptor = new Descriptor("pip-services", "state-store", "memory", "*", "1.0");

		/// <summary>
		/// Create a new instance of the factory.
		/// </summary>
		public DefaultLogicFactory()
		{
			RegisterAsType(MemoryCacheDescriptor, typeof(MemoryCache));
			RegisterAsType(NullCacheDescriptor, typeof(NullCache));
			RegisterAsType(NullLockDescriptor, typeof(NullLock));
			RegisterAsType(MemoryLockDescriptor, typeof(MemoryLock));
			RegisterAsType(MemoryStateStoreDescriptor, typeof(MemoryStateStore));
			RegisterAsType(NullStateStoreDescriptor, typeof(NullStateStore));
		}
	}
}
