using PipServices4.Expressions.IO;
using System.IO;
using Xunit;

namespace PipServices4.Expressions.Test.Io
{
    public class TextPushbackReaderTest
    {
        private string _content;
        private TextPushbackReader _reader;
        private PushbackReaderFixture _fixture;

        public TextPushbackReaderTest()
        {
            _content = "Test String";
            _reader = new TextPushbackReader(new StringReader(_content));
            _fixture = new PushbackReaderFixture(_reader, _content);
        }

        [Fact]
        public void TestOperations()
        {
            _fixture.TestOperations();
        }
    }
}
