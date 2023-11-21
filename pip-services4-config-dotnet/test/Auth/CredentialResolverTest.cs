using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Config.Auth;
using System;
using System.Linq;
using Xunit;

namespace PipServices4.Config.test.Auth
{
	//[TestClass]
	public sealed class CredentialResolverTest
    {
        private static readonly ConfigParams RestConfig = ConfigParams.FromTuples(
            "credential.username", "Negrienko",
            "credential.password", "qwerty",
            "credential.access_key", "key",
            "credential.store_key", "store key"
        );

        [Fact]
        public void TestConfigure()
        {
            var credentialResolver = new CredentialResolver(RestConfig);
            var config = credentialResolver.GetAll().FirstOrDefault();

            Assert.Equal("Negrienko", config["username"]);
            Assert.Equal("qwerty", config["password"]);
            Assert.Equal("key", config["access_key"]);
            Assert.Equal("store key", config["store_key"]);
        }

        [Fact]
        public void TestLookup()
        {
            var credentialResolver = new CredentialResolver();
            var credential = credentialResolver.LookupAsync(Context.FromTraceId("context")).Result;
            Assert.Null(credential);

            var restConfigWithoutStoreKey = ConfigParams.FromTuples(
                "credential.username", "Negrienko",
                "credential.password", "qwerty",
                "credential.access_key", "key"
            );
            credentialResolver = new CredentialResolver(restConfigWithoutStoreKey);
            credential = credentialResolver.LookupAsync(Context.FromTraceId("context")).Result;

            Assert.Equal("Negrienko", credential.Get("username"));
            Assert.Equal("qwerty", credential.Get("password"));
            Assert.Equal("key", credential.Get("access_key"));
            Assert.Null(credential.Get("store_key"));

            credentialResolver = new CredentialResolver(RestConfig);
            credentialResolver.SetReferences(new References());
            try
            {
                credential = credentialResolver.LookupAsync(Context.FromTraceId("context")).Result;
            }
            catch (Exception)
            {
                //Assert.IsType<ReferenceException>(ex);
            }
        }
    }
}
