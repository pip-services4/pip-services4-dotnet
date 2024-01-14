using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Config.Auth;
using PipServices4.Config.Connect;
using System.Threading.Tasks;

namespace PipServices4.Azure.Connect
{
    public class AzureStorageConnectionResolver : IConfigurable, IReferenceable
    {
        protected ConnectionResolver _connectionResolver = new ConnectionResolver();
        
        protected CredentialResolver _credentialResolver = new CredentialResolver();
        
        public void Configure(ConfigParams config)
        {
            _connectionResolver.Configure(config);
            _credentialResolver.Configure(config);
        }
        
        public void SetReferences(IReferences references)
        {
            _connectionResolver.SetReferences(references);
            _credentialResolver.SetReferences(references);
        }
        
        public async Task<AzureStorageConnectionParams> ResolveAsync(IContext context)
        {
            var result = new AzureStorageConnectionParams();

            var connection = await _connectionResolver.ResolveAsync(context);
            result.Append(connection);

            var credential = await _credentialResolver.LookupAsync(context);
            result.Append(credential);

            // Perform validation
            var err = result.Validate(context);
            if (err != null)
            {
                throw err;
            }

            return result;
        }
    }
}