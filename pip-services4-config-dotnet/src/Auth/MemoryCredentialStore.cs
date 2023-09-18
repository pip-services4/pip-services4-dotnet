using PipServices4.Components.Config;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Config.Auth
{
	/// <summary>
	/// Credential store that keeps credentials in memory.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - [credential key 1]:
	/// - ...                          credential parameters for key 1
	/// - [credential key 2]:
	/// - ...                          credential parameters for key N
	/// </summary>
	/// <example>
	/// <code>
	/// var config = ConfigParams.FromTuples(
	/// "key1.user", "jdoe",
	/// "key1.pass", "pass123",
	/// "key2.user", "bsmith",
	/// "key2.pass", "mypass" );
	/// 
	/// var credentialStore = new MemoryCredentialStore();
	/// credentialStore.ReadCredentials(config);
	/// credentialStore.LookupAsync("123", "key1");
	/// </code>
	/// </example>
	/// See <see cref="ICredentialStore"/>, <see cref="CredentialParams"/>
	public class MemoryCredentialStore : ICredentialStore, IReconfigurable
    {
        private Dictionary<string, CredentialParams> _items = new Dictionary<string, CredentialParams>();
        private object _lock = new object();

        /// <summary>
        /// Creates a new instance of the credential store.
        /// </summary>
        public MemoryCredentialStore() { }

        /// <summary>
        /// Creates a new instance of the credential store.
        /// </summary>
        /// <param name="credentials">(optional) configuration with credential parameters.</param>
        public MemoryCredentialStore(ConfigParams credentials)
        {
            Configure(credentials);
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public virtual void Configure(ConfigParams config)
        {
            ReadCredentials(config);
        }

        /// <summary>
        /// Reads credentials from configuration parameters.
        /// Each section represents an individual CredentialParams
        /// </summary>
        /// <param name="config">configuration parameters to be read</param>
        public void ReadCredentials(ConfigParams config)
        {
            lock (_lock)
            {
                _items.Clear();
                var sections = config.GetSectionNames();
                foreach (var section in sections)
                {
                    var value = config.GetSection(section);
                    var creadentials = CredentialParams.FromConfig(value);
                    _items.Add(section, creadentials);
                }
            }
        }

        /// <summary>
        /// Stores credential parameters into the store.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the credential parameters.</param>
        /// <param name="credential">a credential parameters to be stored.</param>
        /// <returns></returns>
        public async Task StoreAsync(string correlationId, string key, CredentialParams credential)
        {
            lock (_lock)
            {
                if (credential != null)
                    _items[key] = credential;
                else
                    _items.Remove(key);
            }

            await Task.Delay(0);
        }

        /// <summary>
        /// Lookups credential parameters by its key.
        /// </summary>
        /// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the credential parameters.</param>
        /// <returns>resolved credential parameters or null if nothing was found.</returns>
        public async Task<CredentialParams> LookupAsync(string correlationId, string key)
        {
            CredentialParams credential = null;

            lock (_lock)
            {
                _items.TryGetValue(key, out credential);
            }

            return await Task.FromResult(credential);
        }

    }
}
