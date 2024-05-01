using PipServices4.Components.Config;
using PipServices4.Mongodb.Connect;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Mongodb.Test.Connect
{
    /// <summary>
    /// Unit tests for the <c>MongoDbConnectionTest</c> class
    /// </summary>
    [Collection("Sequential")]
    public class MongoDbConnectionTest
    {
        private MongoDbConnection Db { get; }

        private string mongoUri;
        private string mongoHost;
        private string mongoPort;
        private string mongoDatabase;

        public MongoDbConnectionTest()
        {
            Db = new MongoDbConnection();

            mongoUri = Environment.GetEnvironmentVariable("MONGO_URI");
            mongoHost = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "localhost";
            mongoPort = Environment.GetEnvironmentVariable("MONGO_PORT") ?? "27017";
            mongoDatabase = Environment.GetEnvironmentVariable("MONGO_DB") ?? "test";

            if (mongoUri == null && mongoHost == null)
                return;

            if (Db == null) return;
        }

        [Fact]
        public async Task TestOpenAsync_Success()
        {
            Db.Configure(ConfigParams.FromTuples(
                "connection.uri", mongoUri,
                "connection.host", mongoHost,
                "connection.port", mongoPort,
                "connection.database", mongoDatabase
            ));

            await Db.OpenAsync(null);

            var actual = Db.IsOpen();

            Assert.True(actual);
        }

        //[Fact]
        //public void TestOpenAsync_Failure()
        //{
        //    Db.CloseAsync(null).Wait();

        //    Db.Configure(ConfigParams.FromTuples(
        //        "connection.uri", mongoUri,
        //        "connection.host", mongoHost,
        //        "connection.port", "1234",
        //        "connection.database", mongoDatabase
        //    ));

        //    var ex = Assert.Throws<AggregateException>(() => Db.OpenAsync(null).Wait());
        //    Assert.Equal("Connection to mongodb failed", ex.InnerException.Message);
        //}
    }
}
