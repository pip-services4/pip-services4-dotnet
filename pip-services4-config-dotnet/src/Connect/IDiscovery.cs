using PipServices4.Components.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Config.Connect
{
	/// <summary>
	/// Interface for discovery services which are used to store and resolve connection parameters
	/// to connect to external services.
	/// </summary>
	/// See <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_connect_1_1_connection_params.html">ConnectionParams</a>, 
	/// <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_auth_1_1_credential_params.html">CredentialParams</a>
	public interface IDiscovery
    {
        /// <summary>
        /// Registers connection parameters into the discovery service.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the connection parameters.</param>
        /// <param name="connection">a connection to be registered.</param>
        Task RegisterAsync(IContext context, string key, ConnectionParams connection);

        /// <summary>
        /// Resolves a single connection parameters by its key.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the connection.</param>
        /// <returns>a resolved connection.</returns>
        Task<ConnectionParams> ResolveOneAsync(IContext context, string key);

        /// <summary>
        /// Resolves all connection parameters by their key.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="key">a key to uniquely identify the connection.</param>
        /// <returns>a list with resolved connections.</returns>
        Task<List<ConnectionParams>> ResolveAllAsync(IContext context, string key);
    }
}
