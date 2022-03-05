using Xunit;

namespace jmespath.lexer.tests;

public class ScannerTest
{
    [Fact]
    public void Scanner_GetNextToken_Identifier()
    {
        var scanner = new Scanner("foo");
        var actual = scanner.GetNextToken();

        Assert.Equal(TokenType.T_USTRING, actual.Type);
        Assert.Equal("foo", actual.RawText);

        Assert.Equal(TokenType.EOF, scanner.GetNextToken().Type);
    }
}
