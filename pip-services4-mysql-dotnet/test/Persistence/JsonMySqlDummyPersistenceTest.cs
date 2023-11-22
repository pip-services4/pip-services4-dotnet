using PipServices4.Components.Config;
using PipServices4.MySql.Test.Fixtures;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.MySql.Test.Persistence
{
    [Collection("Sequential")]
	public class JsonMySqlDummyPersistenceTest : IDisposable
	{
		private JsonMySqlDummyPersistence persistence;
		private DummyPersistenceFixture fixture;

		private string mysqlUri;
		private string mysqlHost;
		private string mysqlPort;
		private string mysqlDatabase;
		private string mysqlUsername;
		private string mysqlPassword;

		public JsonMySqlDummyPersistenceTest()
		{
			mysqlUri = Environment.GetEnvironmentVariable("MYSQL_URI");
			mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
			mysqlPort = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
			mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DB") ?? "test";
			mysqlUsername = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "user";
			mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password";

			if (mysqlUri == null && mysqlHost == null)
				return;

			var dbConfig = ConfigParams.FromTuples(
				"connection.uri", mysqlUri,
				"connection.host", mysqlHost,
				"connection.port", mysqlPort,
				"connection.database", mysqlDatabase,
				"credential.username", mysqlUsername,
				"credential.password", mysqlPassword
			);

			persistence = new JsonMySqlDummyPersistence();
			persistence.Configure(dbConfig);

			fixture = new DummyPersistenceFixture(persistence);

			persistence.OpenAsync(null).Wait();
			persistence.ClearAsync(null).Wait();
		}

		public void Dispose()
		{
			persistence.CloseAsync(null).Wait();
		}

		[Fact]
		public async Task TestCrudOperations()
		{
			await fixture.TestCrudOperationsAsync();
		}

		[Fact]
		public async Task TestBatchOperations()
		{
			await fixture.TestBatchOperationsAsync();
		}
	}
}
