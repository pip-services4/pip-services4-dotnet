using PipServices4.Expressions.IO;
using Xunit;

namespace PipServices4.Expressions.Test.Io
{
    public class StringScannerTest
    {
        string content;
        StringScanner scanner;
        ScannerFixture fixture;

        public StringScannerTest()
        {
            content = "Test String\nLine2\rLine3\r\n\r\nLine5";
            scanner = new StringScanner(content);
            fixture = new ScannerFixture(scanner, content);
        }

        [Fact]
        public void TestRead()
        {
            fixture.TestRead();
        }

        [Fact]
        public void TestUnread()
        {
            fixture.TestUnread();
        }

        [Fact]
        public void TestLineColumn()
        {
            fixture.TestLineColumn(3, 's', 1, 3);
            fixture.TestLineColumn(12, '\n', 2, 0);
            fixture.TestLineColumn(15, 'n', 2, 3);
            fixture.TestLineColumn(21, 'n', 3, 3);
            fixture.TestLineColumn(26, '\r', 4, 0);
            fixture.TestLineColumn(30, 'n', 5, 3);
        }
    }
}
