using PipServices4.Expressions.IO;
using Xunit;

namespace PipServices4.Expressions.Test.Io
{
    public class ScannerFixture
    {
        private IScanner _scanner;
        private string _content;

        public ScannerFixture(IScanner scanner, string content)
        {
            _scanner = scanner;
            _content = content;
        }

        public void TestRead()
        {
            int chr;
            _scanner.Reset();
            for (int i = 0; i < _content.Length; i++)
            {
                chr = _scanner.Read();
                Assert.Equal((int)_content[i], chr);
            }

            chr = _scanner.Read();
            Assert.Equal('\xffff', chr);

            chr = _scanner.Read();
            Assert.Equal('\xffff', chr);
        }

        public void TestUnread()
        {
            _scanner.Reset();

            int chr = _scanner.Peek();
            Assert.Equal((int)_content[0], chr);

            chr = this._scanner.Read();
            Assert.Equal((int)_content[0], chr);

            chr = this._scanner.Read();
            Assert.Equal(this._content[1], chr);

            this._scanner.Unread();
            chr = this._scanner.Read();
            Assert.Equal(this._content[1], chr);

            this._scanner.UnreadMany(2);
            chr = this._scanner.Read();
            Assert.Equal(this._content[0], chr);
            chr = this._scanner.Read();
            Assert.Equal(this._content[1], chr);
        }

        public void TestLineColumn(int position, int charAt, int line, int column)
        {
            _scanner.Reset();

            // Get in position
            while (position > 1)
            {
                this._scanner.Read();
                position--;
            }

            // Test forward scanning
            int chr = _scanner.Read();
            Assert.Equal(charAt, chr);
            int ln = this._scanner.Line();
            Assert.Equal(line, ln);
            int col = this._scanner.Column();
            Assert.Equal(column, col);

            // Moving backward
            chr = this._scanner.Read();
            if (chr != '\xffff')
            {
                this._scanner.Unread();
            }
            this._scanner.Unread();

            // Test backward scanning
            chr = this._scanner.Read();
            Assert.Equal(charAt, chr);
            ln = this._scanner.Line();
            Assert.Equal(line, ln);
            col = this._scanner.Column();
            Assert.Equal(column, col);
        }
    }
}