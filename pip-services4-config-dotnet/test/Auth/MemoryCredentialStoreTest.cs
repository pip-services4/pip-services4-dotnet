using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Config.Auth;
using Xunit;

namespace PipServices4.Config.test.Auth
{
	//[TestClass]
	public sealed class MemoryCredentialStoreTest
    {

        [Fact]
        public async void TestLookupAndStore()
        {
            var config = ConfigParams.FromTuples(
                "key1.username", "user1",
                "key1.password", "pass1",
                "key2.username", "user2",
                "key2.password", "pass2"
            );

            var credentialStore = new MemoryCredentialStore();
            credentialStore.ReadCredentials(config);

            var cred1 = await credentialStore.LookupAsync(Context.FromTraceId("123"), "key1");
            var cred2 = await credentialStore.LookupAsync(Context.FromTraceId("123"), "key2");

            Assert.Equal("user1", cred1.Username);
            Assert.Equal("pass1", cred1.Password);
            Assert.Equal("user2", cred2.Username);
            Assert.Equal("pass2", cred2.Password);

            var credConfig = new CredentialParams(
                ConfigParams.FromTuples(
                "username", "user3",
                "password", "pass3",
                "access_id", "123"
            ));

            await credentialStore.StoreAsync(null, "key3", credConfig);

            var cred3 = await credentialStore.LookupAsync(Context.FromTraceId("123"), "key3");

            Assert.Equal("user3", cred3.Username);
            Assert.Equal("pass3", cred3.Password);
            Assert.Equal("123", cred3.AccessId);
        }
    }
}
