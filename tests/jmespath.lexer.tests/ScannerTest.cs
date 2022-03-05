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

    [Fact]
    public void Scanner_Trim()
    {
        var scanner = new Scanner(" \b\f\r\n\tfoo");
        var token = scanner.GetNextToken();

        Assert.Equal(TokenType.T_USTRING, token.Type);
        Assert.Equal(1, token.Location!.EndLine);
        Assert.Equal(4, token.Location!.EndColumn);
    }

    [Fact]
    public void Scanner_GetNextToken_single_character_tokens()
    {
        var scanner = new Scanner("");
        var results = new (TokenType Type, string Text)[] { };

        foreach (var expected in results)
        {
            var token = scanner.GetNextToken();
            Assert.Equal(expected.Type, token.Type);
            Assert.Equal(expected.Text, token.RawText);
        }

        Assert.Equal(TokenType.EOF, scanner.GetNextToken().Type);
    }
}
