using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Config.Auth
{
	/// <summary>
	/// Helper class to retrieve component credentials.
	/// 
	/// If credentials are configured to be retrieved from <see cref="ICredentialStore"/>,
	/// it automatically locates <see cref="ICredentialStore"/>in component references
	/// and retrieve credentials from there using <c>store_key</c> parameter.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// credential:
	/// - store_key:                   (optional) a key to retrieve the credentials from <see cref="ICredentialStore"/>
	/// - ...                          other credential parameters
	/// 
	/// credentials:                   alternative to credential
	/// - [credential params 1]:       first credential parameters
	/// - ...
	/// - [credential params N]:       Nth credential parameters
	/// - ...
	/// 
	/// ### References ###
	/// - *:credential-store:*:*:1.0     (optional) Credential stores to resolve credentials
	/// </summary>
	/// <example>
	/// <code>
	/// var config = ConfigParams.FromTuples(
	/// "credential.user", "jdoe",
	/// "credential.pass",  "pass123" );
	/// 
	/// var credentialResolver = new CredentialResolver();
	/// credentialResolver.Configure(config);
	/// credentialResolver.SetReferences(references);
	/// 
	/// credentialResolver.LookupAsync("123");
	/// </code>
	/// </example>
	/// See <see cref="CredentialParams"/>, <see cref="ICredentialStore"/>
	public sealed class CredentialResolver
    {
        private readonly List<CredentialParams> _credentials = new List<CredentialParams>();
        private IReferences _references = null;

        /// <summary>
        /// Creates a new instance of credentials resolver.
        /// </summary>
        /// <param name="config">(optional) component configuration parameters</param>
        /// <param name="references">(optional) component references</param>
        public CredentialResolver(ConfigParams config = null, IReferences references = null)
        {
            if (config != null) Configure(config);
            if (references != null) SetReferences(references);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public void SetReferences(IReferences references)
        {
            _references = references;
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        /// <param name="configAsDefault">boolean parameter for default configuration. If "true"
        /// the default value will be added to the result.</param>
        public void Configure(ConfigParams config, bool configAsDefault = true)
        {
            _credentials.AddRange(CredentialParams.ManyFromConfig(config, configAsDefault));
        }

        /// <summary>
        /// Gets all credentials configured in component configuration.
        /// Redirect to CredentialStores is not done at this point.If you need fully
        /// fleshed credential use lookup() method instead.
        /// </summary>
        /// <returns>a list with credential parameters</returns>
        public List<CredentialParams> GetAll()
        {
            return _credentials;
        }

        /// <summary>
        /// Adds a new credential to component credentials
        /// </summary>
        /// <param name="connection">new credential parameters to be added</param>
        public void Add(CredentialParams connection)
        {
            _credentials.Add(connection);
        }

        private async Task<CredentialParams> LookupInStoresAsync(IContext context, CredentialParams credential)
        {
            if (credential.UseCredentialStore == false) return null;

            var key = credential.StoreKey;
            if (_references == null) return null;

            var traceId = context != null ? ContextResolver.GetTraceId(context) : null;

            var components = _references.GetOptional(new Descriptor("*", "credential_store", "*", "*", "*"));
            if (components.Count == 0)
                throw new ReferenceException(traceId, "Credential store wasn't found to make lookup");

            foreach (var component in components)
            {
                var store = component as ICredentialStore;
                if (store != null)
                {
                    var resolvedCredential = await store.LookupAsync(context, key);
                    if (resolvedCredential != null)
                        return resolvedCredential;
                }
            }

            return null;
        }

        /// <summary>
        /// Looks up component credential parameters. If credentials are configured to be
        /// retrieved from Credential store it finds a ICredentialStore and lookups
        /// credentials there.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns>resolved credential parameters or null if nothing was found.</returns>
        public async Task<CredentialParams> LookupAsync(IContext context)
        {
            if (_credentials.Count == 0) return null;

            // Return connection that doesn't require discovery
            foreach (var credential in _credentials)
            {
                if (!credential.UseCredentialStore)
                    return credential;
            }

            // Return connection that require discovery
            foreach (var credential in _credentials)
            {
                if (credential.UseCredentialStore)
                {
                    var resolvedConnection = await LookupInStoresAsync(context, credential);
                    if (resolvedConnection != null)
                        return resolvedConnection;
                }
            }

            return null;
        }
    }
}
