using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Exec;
using System;

namespace PipServices4.Config.Config
{
	public abstract class CachedConfigReader: IConfigReader, IReconfigurable
    {
        private long _lastRead = 0;
        private ConfigParams _config;

        public CachedConfigReader()
        {
            Timeout = 60000;
        }

        public long Timeout { get; set; }

        public virtual void Configure(ConfigParams config)
        {
            Timeout = config.GetAsLongWithDefault("timeout", Timeout);
        }

        protected abstract ConfigParams PerformReadConfig(IContext context, ConfigParams parameters);

        public ConfigParams ReadConfig(IContext context, ConfigParams parameters)
        {
            if (_config != null && DateTime.UtcNow.Ticks < _lastRead + TimeSpan.FromMilliseconds(Timeout).Ticks)
            {
                return _config;
            }

            _config = PerformReadConfig(context, parameters);
            _lastRead = DateTime.UtcNow.Ticks;

            return _config;
        }

        public ConfigParams ReadConfigSection(IContext context, ConfigParams parameters, string section)
        {
            var config = ReadConfig(context, parameters);
            return config != null ? config.GetSection(section) : null;
        }

        /// <summary>
        /// Adds a listener that will be notified when configuration is changed
        /// </summary>
        /// <param name="listener">a listener to be added.</param>
        public virtual void AddChangeListener(INotifiable listener)
        {
            // Do nothing...
        }

        /// <summary>
        /// Remove a previously added change listener.
        /// </summary>
        /// <param name="listener">a listener to be removed.</param>
        public virtual void RemoveChangeListener(INotifiable listener)
        {
            // Do nothing...
        }
    }
}
