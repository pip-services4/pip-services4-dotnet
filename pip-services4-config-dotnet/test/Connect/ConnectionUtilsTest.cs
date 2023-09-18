using PipServices4.Components.Config;
using PipServices4.Config.Connect;
using Xunit;

namespace PipServices4.Config.test.Connect
{
	public sealed class ConnectionUtilsTest
    {

        [Fact]
        public void TestConcatOptions()
        {
            var options1 = ConfigParams.FromTuples(
                "host", "server1",
                "port", "8080",
                "param1", "ABC"
            );

            var options2 = ConfigParams.FromTuples(
                "host", "server2",
                "port", "8080",
                "param2", "XYZ"
            );

            var options = ConnectionUtils.Concat(options1, options2);

            Assert.Equal(4, options.Count);
            Assert.Equal("server1,server2", options.GetAsNullableString("host"));
            Assert.Equal("8080,8080", options.GetAsNullableString("port"));
            Assert.Equal("ABC", options.GetAsNullableString("param1"));
            Assert.Equal("XYZ", options.GetAsNullableString("param2"));
        }

        [Fact]
        public void TestIncludeKeys()
        {
            var options1 = ConfigParams.FromTuples(
                "host", "server1",
                "port", "8080",
                "param1", "ABC"
            );

            var options = ConnectionUtils.Include(options1, "host", "port");

            Assert.Equal(2, options.Count);
            Assert.Equal("server1", options.GetAsNullableString("host"));
            Assert.Equal("8080", options.GetAsNullableString("port"));
            Assert.Null(options.GetAsNullableString("param1"));
        }

        [Fact]
        public void TestExcludeKeys()
        {
            var options1 = ConfigParams.FromTuples(
                "host", "server1",
                "port", "8080",
                "param1", "ABC"
            );

            var options = ConnectionUtils.Exclude(options1, "host", "port");

            Assert.Single(options);
            Assert.Null(options.GetAsNullableString("host"));
            Assert.Null(options.GetAsNullableString("port"));
            Assert.Equal("ABC", options.GetAsNullableString("param1"));
        }

        [Fact]
        public void TestParseURI1()
        {
            var options = ConnectionUtils.ParseUri("broker1", "kafka", 9092);
            Assert.Equal(4, options.Count);
            Assert.Equal("broker1:9092", options.GetAsNullableString("servers"));
            Assert.Equal("kafka", options.GetAsNullableString("protocol"));
            Assert.Equal("broker1", options.GetAsNullableString("host"));
            Assert.Equal("9092", options.GetAsNullableString("port"));

            options = ConnectionUtils.ParseUri("tcp://broker1:8082", "kafka", 9092);
            Assert.Equal(4, options.Count);
            Assert.Equal("broker1:8082", options.GetAsNullableString("servers"));
            Assert.Equal("tcp", options.GetAsNullableString("protocol"));
            Assert.Equal("broker1", options.GetAsNullableString("host"));
            Assert.Equal("8082", options.GetAsNullableString("port"));

            options = ConnectionUtils.ParseUri("tcp://user:pass123@broker1:8082", "kafka", 9092);
            Assert.Equal(6, options.Count);
            Assert.Equal("broker1:8082", options.GetAsNullableString("servers"));
            Assert.Equal("tcp", options.GetAsNullableString("protocol"));
            Assert.Equal("broker1", options.GetAsNullableString("host"));
            Assert.Equal("8082", options.GetAsNullableString("port"));
            Assert.Equal("user", options.GetAsNullableString("username"));
            Assert.Equal("pass123", options.GetAsNullableString("password"));

            options = ConnectionUtils.ParseUri("tcp://user:pass123@broker1,broker2:8082", "kafka", 9092);
            Assert.Equal(6, options.Count);
            Assert.Equal("broker1:9092,broker2:8082", options.GetAsNullableString("servers"));
            Assert.Equal("tcp", options.GetAsNullableString("protocol"));
            Assert.Equal("broker1,broker2", options.GetAsNullableString("host"));
            Assert.Equal("9092,8082", options.GetAsNullableString("port"));
            Assert.Equal("user", options.GetAsNullableString("username"));
            Assert.Equal("pass123", options.GetAsNullableString("password"));

            options = ConnectionUtils.ParseUri("tcp://user:pass123@broker1:8082,broker2:8082?param1=ABC&param2=XYZ", "kafka", 9092);
            Assert.Equal(8, options.Count);
            Assert.Equal("broker1:8082,broker2:8082", options.GetAsNullableString("servers"));
            Assert.Equal("tcp", options.GetAsNullableString("protocol"));
            Assert.Equal("broker1,broker2", options.GetAsNullableString("host"));
            Assert.Equal("8082,8082", options.GetAsNullableString("port"));
            Assert.Equal("user", options.GetAsNullableString("username"));
            Assert.Equal("pass123", options.GetAsNullableString("password"));
            Assert.Equal("ABC", options.GetAsNullableString("param1"));
            Assert.Equal("XYZ", options.GetAsNullableString("param2"));
        }

        [Fact]
        public void TestParseURI2()
        {
            var options = ConfigParams.FromTuples(
                "host", "broker1,broker2",
                "port", ",8082",
                "username", "user",
                "password", "pass123",
                "param1", "ABC",
                "param2", "XYZ",
                "param3", null
            );

            var uri = ConnectionUtils.ComposeUri(options, "tcp", 9092);
            Assert.Equal("tcp://user:pass123@broker1:9092,broker2:8082?param1=ABC&param2=XYZ&param3", uri);

            uri = ConnectionUtils.ComposeUri(options, null, 0);
            Assert.Equal("user:pass123@broker1,broker2:8082?param1=ABC&param2=XYZ&param3", uri);
        }
    }
}
