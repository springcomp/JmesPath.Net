using jmespath.lexer.Tokens;
namespace jmespath.lexer;
public class Token
{
    public static readonly Token EOF = new Token(TokenType.EOF, "");

    public Token(TokenType type, string rawText)
    {
        Type = type;
        RawText = rawText;
    }

    public TokenType Type { get; }
    public string RawText { get; }
    public virtual object Value => RawText;
    public LexLocation? Location { get; set; }

    internal Token SetPosition(int line, int column, int endColumn)
    {
        var location = new LexLocation(line, column, line, endColumn);
        var token = new Token(Type, RawText) { Location = location, };

        return token;
    }
    internal Token SetRawText(string rawText)
        => new Token(Type, rawText) { Location = Location, };

    public static Token Create(TokenType tokenType, string yytext)
    {
        switch (tokenType)
        {
            case TokenType.error:
            case TokenType.EOF:
                return new Token(tokenType, String.Empty);

            case TokenType.T_NUMBER:
                return new NumberToken(yytext);

            case TokenType.T_LSTRING:
                return new LiteralStringToken(yytext);

            case TokenType.T_QSTRING:
                return new QuotedStringToken(yytext);

            case TokenType.T_RSTRING:
                return new RawStringToken(yytext);

            case TokenType.T_LBRACKET:
            case TokenType.T_RBRACKET:
            case TokenType.T_USTRING:
            default:
                return new Token(tokenType, yytext);
        }
    }

    public override string ToString()
    {
        return $"{RawText} ({Type})";
    }
}

