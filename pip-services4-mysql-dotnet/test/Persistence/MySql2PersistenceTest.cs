//using PipServices3.Commons.Config;
//using PipServices3.MySql.Fixtures;
//using System;
//using System.Threading.Tasks;
//using Xunit;

//namespace PipServices4.MySql.Test.Persistence
//{
//	/// <summary>
//	/// Unit tests for the <c>MySql2PersistenceTest</c> class
//	/// </summary>
//	[Collection("Sequential")]
//	public class MySql2PersistenceTest : IDisposable
//	{
//		private MySql2DummyPersistence persistence;
//		private Dummy2PersistenceFixture fixture;

//		private string mysqlUri;
//		private string mysqlHost;
//		private string mysqlPort;
//		private string mysqlDatabase;
//		private string mysqlUsername;
//		private string mysqlPassword;

//		public MySql2PersistenceTest()
//		{
//			mysqlUri = Environment.GetEnvironmentVariable("MYSQL_URI");
//			mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
//			mysqlPort = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
//			mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DB") ?? "test";
//			mysqlUsername = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "user";
//			mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password";

//			if (mysqlUri == null && mysqlHost == null)
//				return;

//			var dbConfig = ConfigParams.FromTuples(
//				"connection.uri", mysqlUri,
//				"connection.host", mysqlHost,
//				"connection.port", mysqlPort,
//				"connection.database", mysqlDatabase,
//				"credential.username", mysqlUsername,
//				"credential.password", mysqlPassword
//			);

//			persistence = new MySql2DummyPersistence();
//			persistence.Configure(dbConfig);

//			fixture = new Dummy2PersistenceFixture(persistence);

//			persistence.OpenAsync(null).Wait();
//			persistence.ClearAsync(null).Wait();
//		}

//		public void Dispose()
//		{
//			persistence.CloseAsync(null).Wait();
//		}

//		[Fact]
//		public async Task TestCrudOperations()
//		{
//			await fixture.TestCrudOperationsAsync();
//		}

//		[Fact]
//		public async Task TestBatchOperations()
//		{
//			await fixture.TestBatchOperationsAsync();
//		}
//	}
//}
