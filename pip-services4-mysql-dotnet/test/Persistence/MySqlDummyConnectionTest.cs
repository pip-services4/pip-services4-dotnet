using PipServices4.Components.Config;
using PipServices4.Components.Refer;
using PipServices4.Mysql.Connect;
using PipServices4.MySql.Test.Fixtures;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.MySql.Test.Persistence
{
    /// <summary>
    /// Unit tests for the <c>MySqlDummyConnectionTest</c> class
    /// </summary>
    [Collection("Sequential")]
	public class MySqlDummyConnectionTest : IDisposable
	{
		private MySqlConnection connection { get; }
		private MySqlDummyPersistence persistence { get; }
		private DummyPersistenceFixture fixture { get;  }

		private string mysqlUri;
		private string mysqlHost;
		private string mysqlPort;
		private string mysqlDatabase;
		private string mysqlUsername;
		private string mysqlPassword;

		public MySqlDummyConnectionTest()
		{
			connection = new MySqlConnection();

			mysqlUri = Environment.GetEnvironmentVariable("MYSQL_URI");
			mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
			mysqlPort = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
			mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DB") ?? "test";
			mysqlUsername = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "user";
			mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password";
			if (mysqlUri == null && mysqlHost == null)
				return;

			if (connection == null) return;

			connection.Configure(ConfigParams.FromTuples(
				"connection.uri", mysqlUri,
				"connection.host", mysqlHost,
				"connection.port", mysqlPort,
				"connection.database", mysqlDatabase,
				"credential.username", mysqlUsername,
				"credential.password", mysqlPassword
			));

			persistence = new MySqlDummyPersistence();

			persistence.SetReferences(References.FromTuples(
				new Descriptor("pip-services", "connection", "mysql", "default", "1.0"), connection
			));

			fixture = new DummyPersistenceFixture(persistence);

			connection.OpenAsync(null).Wait();
			persistence.OpenAsync(null).Wait();
			persistence.ClearAsync(null).Wait();
		}

		public void Dispose()
		{
			persistence.CloseAsync(null).Wait();
			connection.CloseAsync(null).Wait();
		}

		[Fact]
		public void TestConnection()
		{
			Assert.True(connection.GetConnection() != null);
			Assert.True(connection.GetDatabaseName() != "");
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
