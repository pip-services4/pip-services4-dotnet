using PipServices4.Azure.Auth;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Config.Auth;
using PipServices4.Config.Config;
using PipServices4.Config.Connect;
using System;

namespace PipServices4.Azure.Config
{
    /// <summary>
    /// Reads configuration from Azure KeyVault secrets. Secret key becomes a parameter name
    /// </summary>
    public class KeyVaultConfigReader : CachedConfigReader, IReferenceable, IConfigurable
    {
        private ConnectionResolver _connectionResolver = new ConnectionResolver();
        private CredentialResolver _credentialResolver = new CredentialResolver();

        public KeyVaultConfigReader() { }

        public KeyVaultConfigReader(ConfigParams config)
        {
            if (config != null) Configure(config);
        }

        public virtual void SetReferences(IReferences references)
        {
            _connectionResolver.SetReferences(references);
            _credentialResolver.SetReferences(references);
        }

        public override void Configure(ConfigParams config)
        {
            base.Configure(config);
            _connectionResolver.Configure(config, true);
            _credentialResolver.Configure(config, true);
        }

        protected ConfigParams PerformReadConfig(IContext context)
        {
            try
            {
                var connection = _connectionResolver.ResolveAsync(context).Result;
                var credential = _credentialResolver.LookupAsync(context).Result;
                KeyVaultClient _client = new KeyVaultClient(connection, credential);

                var secrets = _client.GetSecretsAsync().Result;
                var result = new ConfigParams();

                foreach (var entry in secrets)
                {
                    var key = entry.Key.Replace('-', '.');
                    var value = entry.Value;
                    result[key] = value;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to load config from KeyVault", ex);
            }
        }

        public static new ConfigParams ReadConfig(IContext context, ConfigParams config)
        {
            return new KeyVaultConfigReader(config).PerformReadConfig(context);
        }

        public static ConfigParams ReadConfig(IContext context, string connectionString)
        {
            var config = ConfigParams.FromString(connectionString);
            return new KeyVaultConfigReader(config).PerformReadConfig(context);
        }

        protected override ConfigParams PerformReadConfig(IContext context, ConfigParams parameters)
        {
            return PerformReadConfig(context);
        }
    }
}
