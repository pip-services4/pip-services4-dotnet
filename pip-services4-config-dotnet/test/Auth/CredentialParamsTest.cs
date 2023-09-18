using PipServices4.Config.Auth;
using Xunit;

namespace PipServices4.Config.test.Auth
{
    //[TestClass]
    public sealed class CredentialParamsTest
    {
        [Fact]
        public void TestStoreKey()
        {
            var сredential = new CredentialParams();
            сredential.StoreKey = null;
            Assert.Null(сredential.StoreKey);

            сredential.StoreKey = "Store key";
            Assert.Equal("Store key", сredential.StoreKey);
            Assert.True(сredential.UseCredentialStore);
        }

        [Fact]
        public void TestUsername()
        {
            var сredential = new CredentialParams();
            сredential.Username = null;
            Assert.Null(сredential.Username);

            сredential.Username = "Kate Negrienko";
            Assert.Equal("Kate Negrienko", сredential.Username);
        }

        [Fact]
        public void TestPassword()
        {
            CredentialParams сredential = new CredentialParams();
            сredential.Password = null;
            Assert.Null(сredential.Password);

            сredential.Password = "qwerty";
            Assert.Equal("qwerty", сredential.Password);
        }

        [Fact]
        public void TestAccessKey()
        {
            var сredential = new CredentialParams();
            сredential.AccessKey = null;
            Assert.Null(сredential.AccessKey);

            сredential.AccessKey = "key";
            Assert.Equal("key", сredential.AccessKey);
        }
        
        [Fact]
        public void TestAccessKeyWithAnotherParamName()
        {
            var сredential = new CredentialParams();
            сredential.Add("client_key", "client key");

            Assert.Equal("client key", сredential.AccessKey);
            
            сredential.Add("secret_key", "secret key");
            сredential.Set("client_key", null);
            Assert.Equal("secret key", сredential.AccessKey);
        }
    }
}
