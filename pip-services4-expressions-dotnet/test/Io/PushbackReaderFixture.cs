using System;
using Xunit;
using PipServices4.Expressions.Tokenizers.Utilities;
using PipServices4.Expressions.IO;

namespace PipServices4.Expressions.Test.Io
{
    public class PushbackReaderFixture
    {
        private IPushbackReader _reader;
        private string _content;

        public PushbackReaderFixture(IPushbackReader reader, string content)
        {
            _reader = reader;
            _content = content;
        }

        public void TestOperations()
        {
            char chr = _reader.Peek();
            Assert.Equal(_content[0], chr);

            chr = _reader.Read();
            Assert.Equal(_content[0], chr);

            chr = _reader.Read();
            Assert.Equal(_content[1], chr);

            _reader.Pushback('#');
            chr = _reader.Read();
            Assert.Equal('#', chr);

            _reader.PushbackString("@$");
            chr = _reader.Read();
            Assert.Equal('@', chr);
            chr = _reader.Read();
            Assert.Equal('$', chr);

            for (var i = 2; i < _content.Length; i++)
            {
                chr = _reader.Read();
                Assert.Equal(_content[i], chr);
            }

            chr = _reader.Read();
            Assert.Equal('\xffff', chr);

            chr = _reader.Read();
            Assert.Equal('\xffff', chr);
        }

        public void TestPushback() {
            char chr;
            char lastChr = '\0';
            for (chr = _reader.Read(); !CharValidator.IsEof(chr); chr = _reader.Read()) {
                lastChr = chr;           
            }

            _reader.Pushback(lastChr);
            _reader.Pushback(chr);

            var chr1 = _reader.Peek();
            Assert.Equal(lastChr, chr1);

            chr1 = _reader.Read();
            Assert.Equal(lastChr, chr1);
        }

    }
}