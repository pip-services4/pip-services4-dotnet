using PipServices4.Components.Config;
using PipServices4.Postgres.Connect;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Postgres.Test.Connect
{
    public class PostgresConnectionResolverTest
    {
        [Fact]
        public async Task TestConnectionConfig()
        {
            var dbConfig = ConfigParams.FromTuples(
                "connection.host", "localhost",
                "connection.port", 5432,
                "connection.database", "test",
                "connection.ssl", true,
                "credential.username", "postgres",
                "credential.password", "postgres"
            );

            var resolver = new PostgresConnectionResolver();
            resolver.Configure(dbConfig);

            var connectionString = await resolver.ResolveAsync(null);
            Assert.Contains("Host=localhost", connectionString);
            Assert.Contains("Port=5432", connectionString);
            Assert.Contains("Database=test", connectionString);
            Assert.Contains("Username=postgres", connectionString);
            Assert.Contains("Password=postgres", connectionString);
            Assert.DoesNotContain("SSL Mode=", connectionString);
        }
    }
}
