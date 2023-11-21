using PipServices4.Components.Context;
using System.Threading.Tasks;

namespace PipServices4.Config.Auth
{
	/// <summary>
	/// Interface for credential stores which are used to store and lookup credentials
	/// to authenticate against external services.
	/// </summary>
	/// See <see cref="CredentialParams"/>, <see cref="ConnectionParams"/>
	public interface ICredentialStore
    {
        /// <summary>
        /// Stores credential parameters into the store.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the credential.</param>
        /// <param name="credential">a credential to be stored.</param>
        Task StoreAsync(IContext context, string key, CredentialParams credential);

        /// <summary>
        /// Lookups credential parameters by its key.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the credential.</param>
        /// <returns>found credential parameters or null if nothing was found</returns>
        Task<CredentialParams> LookupAsync(IContext context, string key);
    }
}
