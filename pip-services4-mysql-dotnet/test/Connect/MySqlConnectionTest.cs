using PipServices4.Components.Config;
using PipServices4.Mysql.Connect;
using System;
using Xunit;

namespace PipServices4.MySql.Test.Connect
{
    /// <summary>
    /// Unit tests for the <c>MySqlConnectionTest</c> class
    /// </summary>
    [Collection("Sequential")]
	public class MySqlConnectionTest : IDisposable
	{
		private MySqlConnection connection { get; }

		private string mysqlUri;
		private string mysqlHost;
		private string mysqlPort;
		private string mysqlDatabase;
		private string mysqlUsername;
		private string mysqlPassword;

		public MySqlConnectionTest()
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

			connection.OpenAsync(null).Wait();

			Assert.True(connection.IsOpen());
		}

		public void Dispose()
		{
			connection.CloseAsync(null).Wait();
		}

		[Fact]
		public void TestOpenAsync_Success()
		{
			Assert.True(connection.GetConnection() != null);
			Assert.True(connection.GetDatabaseName() != "");
		}
	}
}
