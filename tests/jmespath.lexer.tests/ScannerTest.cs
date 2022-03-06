using Xunit;

namespace jmespath.lexer.tests;

public class ScannerTest
{
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
    public void Scanner_GetNextToken_unambiguous_single_character_tokens()
    {
        var scanner = new Scanner(":,.*@{}]()");
        var results = new (TokenType Type, string Text)[] {
            (TokenType.T_COLON, ":"),
            (TokenType.T_COMMA, ","),
            (TokenType.T_DOT, "."),
            (TokenType.T_STAR, "*"),
            (TokenType.T_CURRENT, "@"),
            (TokenType.T_LBRACE, "{"),
            (TokenType.T_RBRACE, "}"),
            (TokenType.T_RBRACKET, "]"),
            (TokenType.T_LPAREN, "("),
            (TokenType.T_RPAREN, ")"),
        };

        AssertTokens(scanner, results);
        Assert.Equal(TokenType.EOF, scanner.GetNextToken().Type);
    }

    [Fact]
    public void Scanner_GetNextToken_ambiguous_one_or_two_character_tokens_short()
    {
        var scanner = new Scanner("& | < >  !");
        var results = new (TokenType Type, string Text)[] {
            (TokenType.T_ETYPE, "&"),
            (TokenType.T_PIPE, "|"),
            (TokenType.T_LT, "<"),
            (TokenType.T_GT, ">"),
            (TokenType.T_NOT, "!"),
        };

        AssertTokens(scanner, results);
        Assert.Equal(TokenType.EOF, scanner.GetNextToken().Type);
    }
    [Fact]
    public void Scanner_GetNextToken_ambiguous_one_or_two_character_tokens_long()
    {
        var scanner = new Scanner("&& || <= >= !=");
        var results = new (TokenType Type, string Text)[] {
            (TokenType.T_AND, "&&"),
            (TokenType.T_OR, "||"),
            (TokenType.T_LE, "<="),
            (TokenType.T_GE, ">="),
            (TokenType.T_NE, "!="),
        };

        AssertTokens(scanner, results);
        Assert.Equal(TokenType.EOF, scanner.GetNextToken().Type);
    }
    [Fact]
    public void Scanner_GetNextToken_two_character_tokens()
    {
        var scanner = new Scanner("== [ [] [? [");
        var results = new (TokenType Type, string Text)[] {
            (TokenType.T_EQ, "=="),
            (TokenType.T_LBRACKET, "["), // T_LBRACKET in the token stream
            (TokenType.T_FLATTEN, "[]"),
            (TokenType.T_FILTER, "[?"),
            (TokenType.T_LBRACKET, "["), // T_LBRACKET before EOF
        };

        AssertTokens(scanner, results);
        Assert.Equal(TokenType.EOF, scanner.GetNextToken().Type);
    }

    [Fact]
    public void Scanner_GetNextToken_LiteralString()
    {
        var scanner = new Scanner("`{\"foo\": \"bar\\`baz\"}`");
        var actual = scanner.GetNextToken();

        Assert.Equal(TokenType.T_LSTRING, actual.Type);
        Assert.Equal("`{\"foo\": \"bar\\`baz\"}`", actual.RawText);
        Assert.Equal("{\"foo\": \"bar`baz\"}", (string) actual.Value);

        Assert.Equal(TokenType.EOF, scanner.GetNextToken().Type);
    }

    [Fact]
    public void Scanner_GetNextToken_UnquotedString()
    {
        var scanner = new Scanner("foo");
        var actual = scanner.GetNextToken();

        Assert.Equal(TokenType.T_USTRING, actual.Type);
        Assert.Equal("foo", actual.RawText);

        Assert.Equal(TokenType.EOF, scanner.GetNextToken().Type);
    }

    private void AssertTokens(Scanner scanner, (TokenType Type, string Text)[] expectedResults)
    { 
        foreach (var expected in expectedResults)
            AssertToken(scanner, expected.Type, expected.Text);
    }
    private void AssertToken(Scanner scanner, TokenType expectedTokenType, string expectedText)
    {
        var token = scanner.GetNextToken();
        Assert.Equal(expectedTokenType, token.Type);
        Assert.Equal(expectedText, token.RawText);
    }
}
